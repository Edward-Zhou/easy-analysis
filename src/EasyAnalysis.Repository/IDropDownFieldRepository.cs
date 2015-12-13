using EasyAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Repository
{
    public interface IDropDownFieldRepository
    {
        int New(DropDownField model);

        IEnumerable<DropDownField> ListByRepository(string repository);

        void Change(int id, Action<DropDownField> model);

        void Remove(DropDownField model);
    }
}
