using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace EasyAnalysis.Actions
{
    public class BuildStackoverflowQuestionProfile : IAction
    {
        private IConnectionStringProvider _connectionStringProvider;

        public string Description
        {
            get
            {
                return "Build the stackoverflow threads profile";
            }
        }

        public BuildStackoverflowQuestionProfile()
        {
            _connectionStringProvider = new UniversalConnectionStringProvider();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// [1-datasource-out (required), e.g. souwp.thread_profiles]
        /// [2-timeframe      (optional), e.g. 2015-11-16T00:00:00Z&2015-11-18T00:00:00Z]
        /// </param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            var repository = "SOUWP";

            var outputDs = MongoDatasource.Parse(args[0]);

            var timeFrameRange = TimeFrameRange.Parse(args[1]);

            using (var connection = new SqlConnection(_connectionStringProvider.GetConnectionString()))
            {
                var mongoClient = new MongoClient(_connectionStringProvider.GetConnectionString("mongo:" + outputDs.DatabaseName));

                var database = mongoClient.GetDatabase(outputDs.DatabaseName);

                var collection = database.GetCollection<BsonDocument>(outputDs.CollectionName);

                var prifiles = connection.Query(SqlQueryFactory.Instance.Get("get_thread_profile"),
                    new
                    {
                        repository = repository.ToUpper(),
                        start = timeFrameRange.Start,
                        end = timeFrameRange.End
                    });

                foreach (dynamic profile in prifiles)
                {
                    var key = Builders<BsonDocument>.Filter.Eq("_id", (profile.Id as string).Trim());

                    var updateAction = Builders<BsonDocument>.Update
                     .Set("title", profile.Title as string)
                     .Set("url", string.Format("http://stackoverflow.com/questions/{0}", (profile.Id as string).ToLower().Replace("so_", "").Trim()))
                     .Set("createdOn", (DateTime)(profile.CreatedOn))
                     .Set("category", profile.Category as string)
                     .Set("type", profile.Type as string);

                    var tags = connection.Query<string>(SqlQueryFactory.Instance.Get("get_thread_tags"), new { ThreadId = profile.Id });

                    if (tags != null)
                    {
                        var tagArray = new BsonArray(tags.Select(m => m.ToLower()).ToList());

                        updateAction = updateAction.Set("tags", tagArray);
                    }

                    await collection.UpdateOneAsync(key, updateAction, new UpdateOptions { IsUpsert = true });
                }
            }
        }
    }
}
