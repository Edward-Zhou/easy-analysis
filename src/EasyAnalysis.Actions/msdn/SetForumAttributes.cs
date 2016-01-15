using EasyAnalysis.Data;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.Data;
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
    public class SetForumAttributes : IAction
    {
        public string Description
        {
            get
            {
                return "Set forum attributes for questions";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// [0-datasource-in  (required), e.g. landing.threads]
        /// [1-timeframe      (optional), e.g. 2015-11-16T00:00:00Z&2015-11-18T00:00:00Z]
        /// </param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            TimeFrameRange timeFrameRange = null;

            string inputCollectionName = args[0];

            if (args.Length > 1)
            {
                timeFrameRange = TimeFrameRange.Parse(args[1]);
            }

            IReadOnlyCollection dsInput = MongoDataCollection.Parse(inputCollectionName);

            var fb = Builders<BsonDocument>.Filter;

            var filter = fb.Exists("views", true);

            if(timeFrameRange != null)
            {

                filter = filter & MongoFilterHelper
                    .CreateBetween("createdOn", timeFrameRange.Start, timeFrameRange.End);
            }

            var collection = dsInput.GetData() as IMongoCollection<BsonDocument>;

            using (var connection =
                new SqlConnection(new UniversalConnectionStringProvider()
                    .GetConnectionString("EasIndexConnection")))
            {
                var factory = SqlQueryFactory.Instance;

                var sql = factory.Get("set_forum_attributes");

                await collection.Find(filter)
                      .Project("{_id: 1, views: 1, users: 1, replies: 1, answered: 1}")
                      .ForEachAsync((item) =>
                      {
                          connection.Execute(sql, new {
                              Id = item.GetValue("_id").AsString,
                              Views = item.GetValue("views").AsInt32,
                              Users = item.GetValue("users").AsInt32,
                              Replies = item.GetValue("replies").AsInt32,
                              Answered = item.GetValue("answered").AsBoolean,
                              Timestamp = DateTime.UtcNow
                          });
                      });
            }
        }
    }
}
