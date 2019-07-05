using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;

namespace MvvmLight4.View
{
    /// <summary>
    /// MarkWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MarkWindow
    {
        public MarkWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<String>(this, "ButtonVisibility", ChangeVisibility);
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        private void ChangeVisibility(string msg)
        {
            if(msg.Equals("showPause"))
            {
                this.Dispatcher.BeginInvoke(new Action(delegate
                {
                    Start.Visibility = Visibility.Collapsed;
                    Pause.Visibility = Visibility.Visible;
                }));
            }
            else
            {
                this.Dispatcher.BeginInvoke(new Action(delegate
                {
                    Pause.Visibility = Visibility.Collapsed;
                    Start.Visibility = Visibility.Visible;
                }));
            }
        }
    }
}
