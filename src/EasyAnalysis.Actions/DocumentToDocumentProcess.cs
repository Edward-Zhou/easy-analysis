using EasyAnalysis.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class DocumentToDocumentProcess
    {
        public IMongoCollection<BsonDocument> In { get; set; }

        public FilterDefinition<BsonDocument> Filter { get; set; }

        public IMongoCollection<BsonDocument> Out { get; set; }

        private const string KeyName = "_id";

        protected async Task UpdateAsync(object key, IDictionary<string, object> values)
        {
            var builder = Builders<BsonDocument>.Update;

            UpdateDefinition<BsonDocument> updateDefination = null;

            foreach(var kv in values)
            {
                if(updateDefination == null)
                {
                    updateDefination = builder.Set(kv.Key, kv.Value);
                }
                else
                {
                    updateDefination = updateDefination.Set(kv.Key, kv.Value);
                }
            }

            var identity = Builders<BsonDocument>.Filter.Eq("_id", key);

            await Out.UpdateOneAsync(
                  identity,
                  updateDefination,
                  new UpdateOptions { IsUpsert = true });
        }

        protected void Map(IDictionary<string, object> values)
        {
            var key = values[KeyName];

            values.Remove(KeyName);

            var task = UpdateAsync(key, values);

            task.Wait();
        }

        public async Task ProcessAsync(Action<IList<IDictionary<string, object>>, BsonDocument> processor)
        {
            await In.Find(Filter)
              .ForEachAsync((item) => {
                  IList<IDictionary<string, object>> list = new List<IDictionary<string, object>>();

                  processor(list, item);

                  foreach(var record in list)
                  {
                      Map(record);
                  }
              });

        }
    }
}
