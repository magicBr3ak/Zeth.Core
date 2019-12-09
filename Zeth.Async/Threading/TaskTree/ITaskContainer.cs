using System.Collections.Generic;

namespace System.Threading.TaskTree
{
    public interface ITaskContainer : ITaskNode
    {
        IEnumerable<ITaskNode> Children { get; }

        void AddChildren(ITaskNode item);
    }
}
