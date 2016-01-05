using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Data
{
    public class TimeFrameAwareCollection : BsonDocumentCollection, Framework.Data.IReadOnlyCollection<BsonDocument>
    {
        public TimeFrameAwareCollection(string source, string timeframe) : base (source)
        {


        }
    }
}
