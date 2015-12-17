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
    public class AddMetadataToThreadProfile : IAction
    {
        private IConnectionStringProvider _connectionStringProvider;

        public string Description
        {
            get
            {
                return "Add thread meatadata (type & tags) to thread profiles";
            }
        }

        public AddMetadataToThreadProfile(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// [0-repository     (required), e.g. uwp]
        /// [1-datasource-out (required), e.g. uwp.thread_profiles]
        /// [2-timeframe      (required), e.g. 2015-10-01T00:00:00&2015-11-30T00:00:00]
        /// </param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            if (args == null || args.Length < 3)
            {
                throw new ArgumentException("Invalid action auguments for clean up data");
            }

            var repository = args[0];

            IReadOnlyCollection outputDs = MongoDataCollection.Parse(args[1]);

            var timeFrameRange = TimeFrameRange.Parse(args[2]);

            using (var connection = new SqlConnection(_connectionStringProvider.GetConnectionString()))
            {
                var collection = outputDs.GetData() as IMongoCollection<BsonDocument>;

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
                     .Set("category", profile.Category as string)
                     .Set("type", profile.Type as string);

                    var tags = connection.Query<string>(SqlQueryFactory.Instance.Get("get_thread_tags"), new { ThreadId = profile.Id });

                    if (tags != null)
                    {
                        var tagArray = new BsonArray(tags.Select(m => m.ToLower()).ToList());

                        updateAction = updateAction.Set("tags", tagArray);
                    }

                    await collection.UpdateOneAsync(key, updateAction);
                }
            }
        }
    }
}
