using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Threading.TaskTree
{
    public abstract class TaskNode<T> : ITaskNode
    {
        #region Properties
        public T Content { get; set; }
        public TaskState State { get; set; }
        public List<ITaskNode> Dependencies { get; private set; }
        public Task<IEnumerable<ITaskNode>> CurrentTask { get; protected set; }

        object ITaskNode.Content
        {
            get { return Content; }
        }
        IEnumerable<ITaskNode> ITaskNode.Dependencies
        {
            get { return Dependencies; }
        }
        #endregion

        #region Methods
        protected abstract void OnStart();
        protected abstract void OnFinish();
        protected abstract Task<IEnumerable<ITaskNode>> GetTask();
        protected async virtual Task<IEnumerable<ITaskNode>> CreateTask(ITaskNode item)
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

            State = TaskState.LOADED;

            OnFinish();
            #endregion

            CurrentTask = null;

            return result;
        }

        public async virtual Task<IEnumerable<ITaskNode>> GetTask(ITaskNode item)
        {
            if (CurrentTask == null)
            {
                switch (State)
                {
                    case TaskState.NONE:
                        CurrentTask = CreateTask(item);
                        break;
                }

                if (CurrentTask == null) return null;
                else return await CurrentTask;
            }
            else return await CurrentTask;
        }
        public void AddDependency(ITaskNode item)
        {
            Dependencies.Add(item);
        }
        #endregion

        #region Constructors
        public TaskNode()
        {
            State = TaskState.NONE;
            Dependencies = new List<ITaskNode>();
        }
        #endregion
    }
}
