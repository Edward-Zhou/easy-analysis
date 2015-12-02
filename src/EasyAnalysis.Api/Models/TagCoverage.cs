using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyAnalysis.Api.Models
{
    public class TagCoverage
    {
        public double daily { get; set; }
        public double weekly { get; set; }
        public double monthly { get; set; }
    }
}