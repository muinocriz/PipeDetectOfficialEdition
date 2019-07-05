using GalaSoft.MvvmLight.Messaging;
using System.Windows.Controls;

namespace MvvmLight4.View
{
    /// <summary>
    /// DetectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DetectWindow
    {
        public DetectWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<bool>(this, "CloseDectWindow", CloseWin);
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }
        /// <summary>
        /// 接受到关闭指令执行关闭窗口函数
        /// </summary>
        /// <param name="b"></param>
        private void CloseWin(bool b)
        {
            if (b == true)
                this.Close();
        }

        private void LogTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            LogTB.ScrollToEnd();
        }
    }
}
