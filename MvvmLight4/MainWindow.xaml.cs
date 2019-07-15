using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using MvvmLight4.Common;
using MvvmLight4.Service;
using MvvmLight4.View;
using MvvmLight4.ViewModel;

namespace MvvmLight4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<int>(this, "videoNotWatched", ReceiveMsg);
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void ReceiveMsg(int videoNotWatched)
        {
            Debug.WriteLine("main window xaml get videoNotWatched count :" + videoNotWatched);
            string text = "您有" + videoNotWatched + "条异常未查看，是否现在查看？\n\n该消息将在下次启动时重新提示";
            List<int> list = new List<int>();
            int taskId = TaskService.GetService().GetLastTaskId();
            Debug.WriteLine("main window xaml get task id :" + taskId);
            if (taskId >= 0)
            {
                MessageBoxResult result = MessageBox.Show(text, "回溯未完成", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    list.Add(taskId);
                    BackTrackWindow sender = new BackTrackWindow();
                    Messenger.Default.Send(list, "DVM2BTVM");
                    sender.ShowDialog();
                }
                else
                {
                    return;
                }

            }
            list = null;
        }

        private Assembly _assembly = Assembly.GetExecutingAssembly();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string targetDayBase64 = RegeditHelper.GetRegedit("sd");
            string targetDayString = Base64Helper.DecodeBase64("utf-8", targetDayBase64);
            DateTime dt = DateTime.Parse(targetDayString);
            int result = DateTime.Compare(DateTime.Now, dt);

            if (result > 0)
            {
                return;
            }
            else
            {
                string winName = ((Button)e.OriginalSource).Tag.ToString();
                Window win = ((Window)_assembly.CreateInstance(string.Format("MvvmLight4.View.{0}", winName)));
                win.Owner = this;
                win.ShowDialog();
            }
        }
    }
}