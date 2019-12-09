using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Threading.TaskTree
{
    public abstract class TaskContainer<T> : TaskNode<T>, ITaskContainer
    {
        #region Properties
        public List<ITaskNode> Children { get; private set; }

        IEnumerable<ITaskNode> ITaskContainer.Children
        {
            get { return Children; }
        }
        #endregion

        #region Methods
        protected async override Task<IEnumerable<ITaskNode>> CreateTask(ITaskNode item)
        {
            var taskList = new List<Task<IEnumerable<ITaskNode>>>();
            var task = default(Task<IEnumerable<ITaskNode>>);
            var result = new List<ITaskNode>();
            var resultCount = 0;

            #region Cargando dependencias
            State = TaskState.LOADING_DEPENDENCIES;

            foreach (var dependency in Dependencies)
            {
                if ((task = dependency.GetTask(this)) != null)
                {
                    taskList.Add(task);
                }
            }

            if (taskList.Count > 0)
            {
                while (taskList.Count > 0)
                {
                    task = await Task.WhenAny(taskList);
                    result.AddRange(await task);
                    taskList.Remove(task);
                }
            }

            if (result.Any(x => x.State == TaskState.ERROR))
            {
                State = TaskState.ERROR;
                return result;
            }
            #endregion
            
            #region Cargando
            State = TaskState.LOADING;

            resultCount = result.Count;

            OnStart();

            if ((task = GetTask()) != null) result.AddRange(await task);

            if (result.Skip(resultCount).Any(x => x.State == TaskState.ERROR))
            {
                State = TaskState.ERROR;
                return result;
            }
            #endregion

            #region Cargando hijos
            State = TaskState.LOADING_CHILDREN;

            resultCount = result.Count;

            foreach (var child in Children)
            {
                if ((task = child.GetTask()) != null)
                {
                    taskList.Add(task);
                }
            }

            if (taskList.Count > 0)
            {
                while (taskList.Count > 0)
                {
                    task = await Task.WhenAny(taskList);
                    result.AddRange(await task);
                    taskList.Remove(task);
                }
            }

            if (result.Skip(resultCount).Any(x => x.State == TaskState.ERROR))
            {
                State = TaskState.ERROR;
                return result;
            }

            State = TaskState.LOADED;

            OnFinish();
            #endregion

            CurrentTask = null;

            return result;
        }

        public async override Task<IEnumerable<ITaskNode>> GetTask(ITaskNode item)
        {
            if (CurrentTask == null)
            {
                switch (State)
                {
                    case TaskState.LOADED_SELF:
                        if (item == null) CurrentTask = CreateTask(null);
                        break;
                    case TaskState.NONE:
                        CurrentTask = CreateTask(item);
                        break;
                }

                if (CurrentTask == null) return null;
                else return await CurrentTask;
            }
            else return await CurrentTask;
        }
        public void AddChildren(ITaskNode item)
        {
            Children.Add(item);
        }
        #endregion
    }
}
