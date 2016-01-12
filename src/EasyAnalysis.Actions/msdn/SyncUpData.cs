using EasyAnalysis.Data;
using EasyAnalysis.Framework;
using EasyAnalysis.Framework.Analysis;
using EasyAnalysis.Framework.ConnectionStringProviders;
using EasyAnalysis.Repository;
using System;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class SyncUpData : IAction
    {
        private IConnectionStringProvider _connectionStringProvider;

        public string Description
        {
            get
            {
                return "Sync up data between Mongo DB and SQL Server";
            }
        }

        public SyncUpData()
        {
            _connectionStringProvider = new UniversalConnectionStringProvider();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// thre mongo document collection to sync with
        /// date range: sync-up date range
        /// </param>
        /// <returns></returns>
        public async Task RunAsync(string[] args)
        {
            var collection = new BsonDocumentCollection(args[0]);

            var timeFrameRange = TimeFrameRange.Parse(args[1]);

            collection.Filter((buider) => {
                return MongoFilterHelper.CreateBetween("createdOn", timeFrameRange.Start, timeFrameRange.End);
            });

            using (var context = new DefaultDbConext())
            {
                await collection.ForEachAsync((item) =>
                  {
                      try
                      {
                          var id = item.GetValue("_id").AsString;

                          var forumId = item.GetValue("forumId").AsString;

                          var question = context.Threads.Find(id);

                          var originalForumId = question.ForumId.TrimEnd();

                          if (originalForumId != forumId)
                          {
                              question.ForumId = forumId;

                              context.SaveChanges();

                              Logger.Current.Info(string.Format("changed {0} to {1}", originalForumId, forumId));
                          }
                      }
                      catch (Exception ex)
                      {
                          Logger.Current.Error(ex.Message);
                      }
                  });
            }
        }
    }
}
