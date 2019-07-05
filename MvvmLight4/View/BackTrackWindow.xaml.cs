using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Input;

namespace MvvmLight4.View
{
    /// <summary>
    /// BackTrackWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BackTrackWindow
    {
        public BackTrackWindow()
        {
            InitializeComponent();
            IsNormal = 0;
            Messenger.Default.Register<String>(this, "BTVM2BTV", GetMsg);
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        private void GetMsg(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                switch (msg)
                {
                    case "enableDeleteBtn":
                        DeleteBtn.IsEnabled = true;
                        break;
                    case "disEnableDeleteBtn":
                        DeleteBtn.IsEnabled = false;
                        break;
                    default:
                        break;
                }
            }
        }
        #region 全屏处理
        /// <summary>
        /// 是否全屏
        /// </summary>
        public int IsNormal { get; set; }

        /// <summary>
        /// 点击全屏按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            ChangeScreen();
        }

        /// <summary>
        /// 全屏
        /// </summary>
        private void ChangeScreen()
        {
            ScreenCtlSP.Visibility = Visibility.Collapsed;
            DataGrid.Visibility = Visibility.Collapsed;
            DetailSP.Visibility = Visibility.Collapsed;

            IsNormal = 1;
        }

        private void MainWin_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BackToNormal();
        }

        /// <summary>
        /// 全屏->默认
        /// </summary>
        private void BackToNormal()
        {
            ScreenCtlSP.Visibility = Visibility.Visible;
            DataGrid.Visibility = Visibility.Visible;
            DetailSP.Visibility = Visibility.Visible;
            IsNormal = 0;
        }

        private void MainWin_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsNormal == 1 && e.Key == Key.Escape)
                BackToNormal();
        }
        #endregion

    }
}
