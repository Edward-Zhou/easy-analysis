using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Data
{
    public class FieldDefination
    {
        public string Name { get; set; }

        public Type DataType { get; set; }

        public string SqlDataType { get; set;}
    } 

    public class BsonToDictionaryConverter
    {
        private IEnumerable<FieldDefination> _fields;

        public BsonToDictionaryConverter(IEnumerable<FieldDefination> fields)
        {
            _fields = fields;
        }

        IDictionary<string, object> Convert(BsonDocument document)
        {
            throw new NotImplementedException();
        }
    }
}
