using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;

namespace MvvmLight4.View
{
    /// <summary>
    /// FrameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FrameWindow
    {
        public FrameWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<String>(this, "FVM2FV", GetMsg);
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        private void GetMsg(string msg)
        {
            if(!string.IsNullOrEmpty(msg))
            {
                switch (msg)
                {
                    case "frameIsRunning":
                        SubmitBtn.IsEnabled = false;
                        break;
                    case "frameIsFinished":
                        SubmitBtn.IsEnabled = true;
                        OpenBtn.Content = "请点击";
                        break;
                    case "frameClosing":
                        SubmitBtn.IsEnabled = true;
                        FrameProg.Visibility = Visibility.Hidden;
                        OpenBtn.Content = "请点击";
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
