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
using EasyAnalysis.Framework.Data;
using EasyAnalysis.Data;

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
        /// [1-repository (required), e.g. SOUWP]
        /// [2-datasource-out (required), e.g. souwp.thread_profiles]
        /// [3-timeframe      (optional), e.g. 2015-11-16T00:00:00Z&2015-11-18T00:00:00Z]
        /// </param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            var repository = args[0].ToUpper();

            var timeFrameRange = TimeFrameRange.Parse(args[2]);

            IReadOnlyCollection outputDs = MongoDataCollection.Parse(args[1]);

            using (var connection = new SqlConnection(_connectionStringProvider.GetConnectionString()))
            {
                var inputDs = new NamedQueryReadOnlyCollection("get_thread_profile", connection);

                inputDs.SetParameters(new
                {
                    repository = repository.ToUpper(),
                    start = timeFrameRange.Start,
                    end = timeFrameRange.End
                });

                var prifiles = inputDs.GetData() as IEnumerable<dynamic>;

                var collection = outputDs.GetData() as IMongoCollection<BsonDocument>;

                foreach (dynamic profile in prifiles)
                {
                    var key = Builders<BsonDocument>.Filter.Eq("_id", (profile.Id as string).Trim());

                    var updateAction = Builders<BsonDocument>.Update
                     .Set("title", profile.Title as string)
                     .Set("url", string.Format("http://stackoverflow.com/questions/{0}", (profile.Id as string).ToLower().Replace("so_", "").Trim()))
                     .Set("createdOn", (DateTime)(profile.CreatedOn))
                     .Set("category", profile.Category as string)
                     .Set("type", profile.Type as string);

                    var tags = inputDs.Connection.Query<string>(SqlQueryFactory.Instance.Get("get_thread_tags"), new { ThreadId = profile.Id });

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
