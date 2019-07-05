using MvvmLight4.Model;

namespace MvvmLight4.Service
{
    public interface ITaskService
    {
        int GetLastTaskId();
        void AddTask(TaskModel Task);
    }
}
