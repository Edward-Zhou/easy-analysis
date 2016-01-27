using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyAnalysis.Models;

namespace EasyAnalysis.Repository
{
    public class ScopeRepository : IScopeRepository
    {
        private readonly DefaultDbConext _context;

        public ScopeRepository(DefaultDbConext context = null)
        {
            if (context == null)
            {
                _context = new DefaultDbConext();
            }
            else
            {
                _context = context;
            }
        }

        public IEnumerable<ScopeModel> List()
        {
            return _context.Scopes.ToList();
        }

        public void Add(ScopeModel scope)
        {
            _context.Scopes.Add(scope);

            _context.SaveChanges();
        }
    }
}
