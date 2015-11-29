using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    internal class MongoHelper
    {
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
