using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyAnalysis.Models;

namespace EasyAnalysis.Repository
{
    public class DropDownFieldRepository : IDropDownFieldRepository
    {
        private readonly DefaultDbConext _context;

        public DropDownFieldRepository(DefaultDbConext context = null)
        {
            if(context == null)
            {
                _context = new DefaultDbConext();
            }else
            {
                _context = context;
            }
        }

        public void Change(int id, Action<DropDownField> model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DropDownField> ListByRepository(string repository)
        {
            return _context.DropDownFields
                    .Include("Options")
                    .Where(m => m.Repository == repository)
                    .ToList();
        }

        public int New(DropDownField model)
        {
            var field = _context.DropDownFields.Add(model);

            _context.SaveChanges();

            return field.Id;
        }

        public void Remove(DropDownField model)
        {
            _context.DropDownFields.Remove(model);

            _context.SaveChanges();
        }
    }
}
