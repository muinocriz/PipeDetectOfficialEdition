using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmLight4.Model;
using MvvmLight4.Service;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Windows;

namespace MvvmLight4.ViewModel
{
    public class SetViewModel : ViewModelBase
    {
        public SetViewModel()
        {
            AssignCommands();
            string frameTimeString = ConfigurationManager.AppSettings["FrameTime"];
            SelFrameTime = Convert.ToInt32(frameTimeString);
            AbnormalTypes = AbnormalService.GetService().GetAbnormalTypeModels();
        }
        private int selFrameTime;
        public int SelFrameTime
        {
            get => selFrameTime; set { selFrameTime = value; RaisePropertyChanged(() => SelFrameTime); }
        }
        private ObservableCollection<AbnormalTypeModel> abnormalTypes;
        public ObservableCollection<AbnormalTypeModel> AbnormalTypes
        {
            get => abnormalTypes; set { abnormalTypes = value; RaisePropertyChanged(() => AbnormalTypes); }
        }
        public ObservableCollection<AbnormalTypeModel> SelectedAbnormalTypes { get; private set; }

        public RelayCommand SelectAbnormal { get; private set; }
        private void ExecuteSelect()
        {
            SelectedAbnormalTypes = new ObservableCollection<AbnormalTypeModel>();
            foreach (var item in AbnormalTypes)
            {
                if (item.IsSelected)
                    SelectedAbnormalTypes.Add(item);
            }

            string json = JsonConvert.SerializeObject(SelectedAbnormalTypes);

            try
            {
                File.WriteAllText("test.txt", json);
            }
            catch (Exception e)
            {
                MessageBox.Show("存储异常！\n" + e.ToString());
            }

            MessageBox.Show("已保存");
        }

        public RelayCommand UpdateFTCmd { get; private set; }
        private void ExecuteUpdateFTCmd()
        {
            Console.WriteLine(SelFrameTime);

            // Open App.Config of executable
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // You need to remove the old settings object before you can replace it
            config.AppSettings.Settings.Remove("FrameTime");
            // Add an Application Setting.
            config.AppSettings.Settings.Add("FrameTime", Convert.ToString(SelFrameTime));
            // Save the changes in App.config file.
            config.Save(ConfigurationSaveMode.Modified);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
        }

        #region helper function
        private void AssignCommands()
        {
            SelectAbnormal = new RelayCommand(() => ExecuteSelect());
            UpdateFTCmd = new RelayCommand(() => ExecuteUpdateFTCmd());
        }
        #endregion
    }
}
