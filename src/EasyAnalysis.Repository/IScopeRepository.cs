using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Repository
{
    public interface IScopeRepository
    {
        IEnumerable<Models.ScopeModel> List();
    }
}
