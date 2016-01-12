using EasyAnalysis.Actions;
using EasyAnalysis.Framework;
using EasyAnalysis.Infrastructure.Discovery;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace EasyAnalysis.Backend
{
    public class MsdnRefreshWebStreamFlow : IWebStreamFlow
    {
        public void Run(string[] parameters)
        {
            string inputCollectionName = parameters[0];

            string outputCollectionName = parameters[1];

            var dateRange = TimeFrameRange.Parse(parameters[2]);

            var collection = Data.MongoDataCollection.Parse(inputCollectionName);

            var data = collection.GetData() as IMongoCollection<BsonDocument>;

            var fb = Builders<BsonDocument>.Filter;

            var filter = fb.Gt("createdOn", dateRange.Start) & fb.Lt("createdOn", dateRange.End) & fb.Exists("del", false);

            var list = new List<string>();

            var task = data.Find(filter)
                .Project("{url: 1}")
                .ForEachAsync((item) =>
                {
                    list.Add(item.GetValue("url", BsonValue.Create("")).AsString + "&outputAs=xml");
                });

            task.Wait();

            IResourceDiscovery discovery = new ListDiscovery(list);

            using (var client = new Message.MessageClient("import-new-question"))
            {
                discovery.OnDiscovered += (url) =>
                {
                    var cmd = new Message.Command.ImportQuestionCommand
                    {
                        Url = url,
                        Collection = outputCollectionName
                    };

                    client.Send(cmd);

                    Logger.Current.Info(string.Format("Discovered [{0}]", url));
                };

                discovery.Start();
            }
        }
    }
}
