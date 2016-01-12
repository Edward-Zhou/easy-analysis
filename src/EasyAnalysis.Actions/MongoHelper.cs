using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace EasyAnalysis.Actions
{
    internal class MongoFilterHelper
    {
        public static FilterDefinition<BsonDocument> CreateBetween(string name, object left, object right)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;

            return filterBuilder.Gte(name, left) & filterBuilder.Lte(name, right);
        }

        public static FilterDefinition<BsonDocument> CreateTimeFrameFilter(TimeFrameRange timeFrameRange)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;

            if (timeFrameRange == null)
            {
                throw new ArgumentNullException("timeFrameRange");
            }

            return filterBuilder.Gte("timestamp", timeFrameRange.Start) & filterBuilder.Lte("timestamp", timeFrameRange.End);
        }
    }
}
