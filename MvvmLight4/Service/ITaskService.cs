using MvvmLight4.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmLight4.Service
{
    public interface ITaskService
    {
        int GetLastTaskId();
        void AddTask(TaskModel Task);
    }
}
