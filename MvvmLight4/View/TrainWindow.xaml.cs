using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLight4.View
{
    /// <summary>
    /// TrainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TrainWindow
    {
        public TrainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<String>(this, "trainMessage", StopButtonVis);
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        private void StopButtonVis(string msg)
        {
            if ("showStopButton".Equals(msg))
                StopButton.Visibility = Visibility.Visible;
            else if ("hideStopButton".Equals(msg))
                StopButton.Visibility = Visibility.Hidden;
            else if ("closeTrainWindow".Equals(msg))
                this.Close();
        }

        /// <summary>
        /// 加载已有模型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModelCB_Checked(object sender, RoutedEventArgs e)
        {
            SP.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 不加载已有模型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModelCB_Unchecked(object sender, RoutedEventArgs e)
        {
            SP.Visibility = Visibility.Hidden;
        }

        private void OutputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OutputTextBox.ScrollToEnd();
        }
    }
}
