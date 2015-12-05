using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using EasyAnalysis.Framework.Data;
using EasyAnalysis.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class SyncWithWebDatabase : IAction
    {
        private IConnectionStringProvider _connectionStringProvider;

        public string Description
        {
            get
            {
                return "Import new threads to web system (database)";
            }
        }

        public SyncWithWebDatabase(IConnectionStringProvider connectionStringProvider = null)
        {
            if(connectionStringProvider == null)
            {
                _connectionStringProvider = new UniversalConnectionStringProvider();
            }
            else
            {
                _connectionStringProvider = connectionStringProvider;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// [0-datasource (required), e.g. landing.threads] 
        /// [1-timeframe  (optional), e.g. 2015-11-16T00:00:00&2015-11-18T00:00:00]</param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            IReadOnlyCollection ds = MongoDataCollection.Parse(args[0]);

            TimeFrameRange timeFrameRange = null;

            if (args.Length > 1)
            {
                timeFrameRange = TimeFrameRange.Parse(args[1]);
            }

            IMongoCollection<BsonDocument> sourceCollection = ds.GetData() as IMongoCollection<BsonDocument>;

            FilterDefinition<BsonDocument> filter = "{}";

            var filterBuilder = Builders<BsonDocument>.Filter;

            if (timeFrameRange != null)
            {
                filter = filter & MongoHelper.CreateTimeFrameFilter(timeFrameRange);
            }

            using (var context = new DefaultDbConext())
            {
                var repository = new ThreadRepository(context);

                await sourceCollection
                  .Find(filter)
                  .ForEachAsync((item) =>
                  {
                      try
                      {
                          var id = item.GetValue("_id").AsString;

                          if (!repository.Exists(id))
                          {
                              var model = new Models.ThreadModel
                              {
                                  Id = id,
                                  Title = item.GetValue("title").AsString,
                                  ForumId = item.GetValue("forumId").AsString,
                                  CreateOn = item.GetValue("createdOn").ToUniversalTime(),
                                  AuthorId = item.GetValue("authorId").AsString
                              };

                              repository.Create(model);

                              Logger.Current.Info("[New]-" + model.Title);
                          }
                      }catch(Exception ex)
                      {
                          Logger.Current.Error(ex.Message);
                      }

                  });
            }
        }
    }
}
