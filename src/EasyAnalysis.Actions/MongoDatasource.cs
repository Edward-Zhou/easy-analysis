using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class MongoDatasource
    {
        public string DatabaseName { get; set; }

        public string CollectionName { get; set; }

        public static MongoDatasource Parse(string text)
        {
            var temp = text.Split('.');

            var databaseName = temp[0];

            var collectionName = temp[1];

            return new MongoDatasource
            {
                DatabaseName = databaseName,
                CollectionName = collectionName
            };
        }
    }
}
