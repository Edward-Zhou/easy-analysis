using System.Threading.Tasks;

namespace EasyAnalysis.Framework.Analysis
{
    public interface IAction
    {
        string Description { get; }

        Task RunAsync(string[] args);
    }
}
