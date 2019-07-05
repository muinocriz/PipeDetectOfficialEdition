using Dapper;
using MvvmLight4.Common;
using MvvmLight4.Model;
using System;
using System.Data;
using System.Linq;

namespace MvvmLight4.Service
{
    public class TaskService : ITaskService
    {
        private static TaskService taskService = new TaskService();
        private TaskService() { }
        public static TaskService GetService() { return taskService; }

        /// <summary>
        /// 检测界面
        /// 任务完成之后，向数据库添加一条检测任务
        /// </summary>
        /// <param name="Task"></param>
        public void AddTask(TaskModel Task)
        {
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                var sql = @"INSERT INTO TB_TASK VALUES(@Id,@VListJSON,@DTime,@Count);";
                conn.Execute(sql, new { Task.Id, Task.VListJSON, Task.DTime, Task.Count });
            }
        }

        /// <summary>
        /// 检测界面
        /// 获得最后的任务ID
        /// </summary>
        /// <returns></returns>
        public int GetLastTaskId()
        {
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT MAX(ID) AS RECENTID FROM TB_TASK;";
                var result = conn.Query(sql).SingleOrDefault();
                int id;
                if (result == null || result.RECENTID == null)
                {
                    id = 0;
                }
                else
                {
                    id = Convert.ToInt32(result.RECENTID);
                }
                return id;
            }
        }
    }
}
