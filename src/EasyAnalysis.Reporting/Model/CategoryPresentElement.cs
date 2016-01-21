using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Reporting.Model
{
    public class TypeNodeEelment
    {
        public string TypeName { get; set; }

        public int Total { get; set; }

        public int Answered { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }

    public class CategoryNodeElement
    {
        public string CategoryName { get; set; }

        public IEnumerable<TypeNodeEelment> SubElements { get; set; }
    }
}
