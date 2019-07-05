using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;

namespace MvvmLight4.View
{
    /// <summary>
    /// ExportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExportWindow
    {
        public ExportWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<String>(this, "EVM2EV", GetMsg);
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        private void GetMsg(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                switch (msg)
                {
                    case "exportIsRunning":
                        ExportBtn.IsEnabled = false;
                        ExportProg.Visibility = Visibility.Visible;
                        break;
                    case "exportIsFinished":
                        ExportBtn.IsEnabled = true;
                        ExportProg.Visibility = Visibility.Hidden;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
