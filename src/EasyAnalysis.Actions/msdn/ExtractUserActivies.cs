﻿using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Data;
using EasyAnalysis.Data;

namespace EasyAnalysis.Actions
{
    public class ExtractUserActivies : IAction
    {
        private IConnectionStringProvider _connectionStringProvider;

        private class EmitScope : IDisposable
        {
            private SqlConnection _sqlConnection;

            public EmitScope(SqlConnection sqlConnection)
            {
                _sqlConnection = sqlConnection;
            }

            public void Emit(string userId, string action, DateTime time, string effectOn)
            {
                try
                {
                    var signature = string.Format("{0}-{1}-{2:MM/dd/yy H:mm:ss}-{3}", userId, action.ToLower(), time, effectOn);

                    var md5Hash = Utils.ComputeStringMD5Hash(signature);

                    var match = _sqlConnection.Query(SqlQueryFactory.Instance.Get("find_user_activity_by_hash"), new { Hash = md5Hash });

                    if (match.Count() == 0)
                    {
                        _sqlConnection.Execute(
                            SqlQueryFactory.Instance.Get("insert_user_activity"),
                            new
                            {
                                Hash = md5Hash,
                                UserId = userId,
                                Action = action,
                                Time = time,
                                EffectOn = effectOn,
                                Timestamp = DateTime.Now
                            });
                    }

                    PrintProgress();
                } catch (Exception ex) {
                    Logger.Current.Error(ex.Message);
                }
            }

            public void Dispose()
            {
                _sqlConnection.Dispose();
            }

            private void PrintProgress()
            {
                Console.Write(".");
            }
        }

        public ExtractUserActivies(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public string Description
        {
            get
            {
                return "Run asker analysis to extract the actions of askers";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// [0-datasource (required), e.g. landing.threads]
        /// [1-timeframe  (optional), e.g. 2015-11-16T00:00:00&2015-11-18T00:00:00]
        /// </param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            Logger.Current.Info(Description);

            IReadOnlyCollection ds = MongoDataCollection.Parse(args[0]);

            TimeFrameRange timeFrameRange = null;

            if (args.Length > 1)
            {
                timeFrameRange = TimeFrameRange.Parse(args[1]);
            }

            var threadCollection = ds.GetData() as IMongoCollection<BsonDocument>;

            FilterDefinition<BsonDocument> filter = "{}";

            var filterBuilder = Builders<BsonDocument>.Filter;

            if (timeFrameRange != null)
            {
                filter = filter & MongoFilterHelper.CreateTimeFrameFilter(timeFrameRange);
            }

            try
            {
                var list = threadCollection.Find(filter);

                var count = await list.CountAsync();

                using (var scope = new EmitScope(new SqlConnection(_connectionStringProvider.GetConnectionString("EasIndexConnection"))))
                {
                    await list.ForEachAsync(item =>
                    {
                        ExtractInThread(item, scope);
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Current.Error(ex.Message);
            }

        }

        private static void ExtractInThread(BsonDocument item, EmitScope scope)
        {
            try
            {
                var threadId = item.GetValue("_id").AsString;

                var authorId = item.GetValue("authorId").AsString;

                var createdOn = item.GetValue("createdOn").ToUniversalTime();

                scope.Emit(authorId, "Ask", createdOn, threadId);

                var messages = item.GetElement("messages").Value.AsBsonArray;

                foreach (BsonDocument message in messages.Skip(1))
                {
                    ExtractInMessage(scope, threadId, message);
                }
            }catch(Exception ex)
            {
                Logger.Current.Error(ex.Message);
            }
        }

        private static void ExtractInMessage(EmitScope scope, string threadId, BsonDocument message)
        {
            try
            {
                var replyAuthorId = message.GetElement("authorId").Value.AsString;

                var replyOn = message.GetElement("createdOn").Value.AsString;

                scope.Emit(replyAuthorId, "Reply", DateTime.Parse(replyOn), threadId);

                BsonArray histories = message.GetElement("histories").Value.AsBsonArray;

                foreach (BsonDocument hisotry in histories)
                {
                    ExtractInHistory(scope, threadId, hisotry);
                }
            }
            catch (Exception ex)
            {
                Logger.Current.Error(ex.Message);
            }
        }

        private static void ExtractInHistory(EmitScope insert, string threadId, BsonDocument hisotry)
        {
            try
            {
                var user = hisotry.GetValue("user").AsString;

                var date = hisotry.GetValue("date").AsString;

                var type = hisotry.GetValue("type").AsString;

                insert.Emit(user, type, DateTime.Parse(date), threadId);
            }
            catch (Exception ex) {
                Logger.Current.Error(ex.Message);
            }

        }
    }
}
