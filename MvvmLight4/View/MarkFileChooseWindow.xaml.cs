using GalaSoft.MvvmLight.Messaging;

namespace MvvmLight4.View
{
    /// <summary>
    /// MarkFileChooseWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MarkFileChooseWindow
    {
        public MarkFileChooseWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<bool>(this, "CloseMarkFileChooseWindow", CloseWin);
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        private void CloseWin(bool b)
        {
            if (b == true)
                this.Close();
        }
    }
}
