using System;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MvvmLight4.Common;
using MvvmLight4.Service;

namespace MvvmLight4.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            AssignCommands();
        }

        public RelayCommand LoadedCmd { get; private set; }
        private void AssignCommands()
        {
            LoadedCmd = new RelayCommand(() => ExecuteLoadedCmd());
        }

        private void ExecuteLoadedCmd()
        {
            //写注册表
            RegeditHelper.SetRegedit("sd", "MjAxOS0wOC0zMQ==");

            string dbName = "data.sqlite";
            if (!File.Exists(dbName))
            {
                Console.WriteLine("数据库不存在");
                int result = InitService.GetService().InitDatabase(dbName);
                if (result < 0)
                {
                    Console.WriteLine("创建出错");
                }
                else
                {
                    Console.WriteLine("创建成功：{0}", result);
                }
            }

            //判断最后一次任务是否有未查看的异常项目
            int lastTaskId = TaskService.GetService().GetLastTaskId();
            if (lastTaskId >= 0)
            {
                //判断本次任务是否有未查看的任务
                int videoNotWatched = AbnormalService.GetService().SearchLastTaskById(lastTaskId);
                if (videoNotWatched > 0)
                {
                    //存在未观看项目
                    //通过消息机制弹出提示
                    Messenger.Default.Send(videoNotWatched, "videoNotWatched");
                }
            }
        }
    }
}