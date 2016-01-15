using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Reporting.Model
{
    public class CategoryAggregationModel
    {
        public string CategoryName { get; set; }

        public string TypeName { get; set; }

        public int Total { get; set;}

        public int Answered { get; set; }
    }
}
