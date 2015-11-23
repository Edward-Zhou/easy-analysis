using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Actions
{
    public class MSSQLDatasource
    {
        public string DatabaseName { get; set; }

        public string ViewName { get; set; }

        public static MSSQLDatasource Parse(string text)
        {
            var temp = text.Split('.');

            var databaseName = temp[0];

            var viewName = temp[1];

            return new MSSQLDatasource
            {
                DatabaseName = databaseName,
                ViewName = viewName
            };
        }
    }
}
