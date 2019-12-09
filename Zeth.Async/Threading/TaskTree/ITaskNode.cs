using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Threading.TaskTree
{
    public interface ITaskNode
    {
        object Content { get; }
        TaskState State { get; }

        IEnumerable<ITaskNode> Dependencies { get; }

        Task<IEnumerable<ITaskNode>> GetTask(ITaskNode item = null);
        void AddDependency(ITaskNode item);
    }
}
