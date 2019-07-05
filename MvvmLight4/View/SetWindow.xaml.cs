using MvvmLight4.Model;
using System.Windows;

namespace MvvmLight4.View
{
    /// <summary>
    /// SetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetWindow
    {
        public SetWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AbnormalTypeModel abnormalTypeModel = (AbnormalTypeModel)MarkSetDataGrid.SelectedItem;
            MessageBox.Show(abnormalTypeModel.Name);
        }
    }
}
