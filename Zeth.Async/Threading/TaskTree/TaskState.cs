namespace System.Threading.TaskTree
{
    public enum TaskState
    {
        NONE = 0,
        LOADING = 10,
        LOADING_DEPENDENCIES = 11,
        LOADING_CHILDREN = 12,
        WAITING = 20,
        LOADED = 30,
        LOADED_SELF = 31,
        ERROR = 40
    }
}
