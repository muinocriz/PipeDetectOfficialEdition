using GalaSoft.MvvmLight.Messaging;
using System;

namespace MvvmLight4.View
{
    /// <summary>
    /// FrameFileChooseWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FrameFileChooseWindow
    {
        public FrameFileChooseWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<String>(this, "FFCVM2FFCW", GetMsg);
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }



        private void GetMsg(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                switch (msg)
                {
                    case "closeWindow":
                        this.Close();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
