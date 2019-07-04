using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MvvmLight4.Common;
using MvvmLight4.Model;
using MvvmLight4.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLight4.ViewModel
{
    public class ExportViewModel : ViewModelBase
    {
        public ExportViewModel()
        {
            AssignCommands();
            InitData();
            InitWorker();
            DispatcherHelper.Initialize();
        }

        #region property
        private BackgroundWorker worker;
        private List<ComplexInfoModel> combboxList;
        private Dictionary<string, string> dict;
        private List<ExportModel> exportModelsForExcel;
        private List<AbnormalViewModel> list;
        private Dictionary<int, AbnormalTypeModel> typeDict;
        private List<ExportData> exportDatas;

        private ObservableCollection<ExportMeta> exportList;
        /// <summary>
        /// 可供输出的任务列表
        /// </summary>
        public ObservableCollection<ExportMeta> ExportList
        {
            get { return exportList; }
            set { exportList = value; RaisePropertyChanged(() => ExportList); }
        }



        /// <summary>
        /// 下拉框列表
        /// </summary>
        public List<ComplexInfoModel> CombboxList
        {
            get { return combboxList; }
            set { combboxList = value; RaisePropertyChanged(() => CombboxList); }
        }
        private ComplexInfoModel combboxItem;
        /// <summary>
        /// 下拉框选中信息
        /// </summary>
        public ComplexInfoModel CombboxItem
        {
            get { return combboxItem; }
            set { combboxItem = value; RaisePropertyChanged(() => CombboxItem); }
        }
        private int way;
        /// <summary>
        /// 输出方式
        /// </summary>
        public int Way
        {
            get { return way; }
            set
            {
                way = value;
                RaisePropertyChanged(() => Way);
            }
        }
        private string targetSource;
        /// <summary>
        /// 导出文件存放位置
        /// </summary>
        public string TargetSource
        {
            get { return targetSource; }
            set
            {
                targetSource = value;
                RaisePropertyChanged(() => TargetSource);
            }
        }

        private bool? selectAll;
        /// <summary>
        /// 全选
        /// </summary>
        public bool? SelectAll
        {
            get
            {
                return selectAll;
            }
            set
            {
                selectAll = value;
                RaisePropertyChanged(() => SelectAll);
            }
        }

        private ObservableCollection<ExportModel> exports;
        /// <summary>
        /// 要输出的属性
        /// </summary>
        public ObservableCollection<ExportModel> Exports
        {
            get { return exports; }
            set
            {
                exports = value;
                RaisePropertyChanged(() => Exports);
            }
        }

        private Visibility proVisiable;
        /// <summary>
        /// 进度条可视状态
        /// </summary>
        public Visibility ProVisiable
        {
            get { return proVisiable; }
            set
            {
                proVisiable = value;
                RaisePropertyChanged(() => ProVisiable);
            }
        }
        #endregion

        public RelayCommand LoadedCmd { get; private set; }
        public RelayCommand CloingCmd { get; private set; }
        private void ExecuteCloingCmd()
        {
            if (worker != null && worker.IsBusy)
            {
                Debug.WriteLine("worker.CancelAsync");
                worker.CancelAsync();
            }
        }

        /// <summary>
        /// 判断批量导出命令
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanExecuteExportListCmd(object arg)
        {
            return !string.IsNullOrEmpty(TargetSource);
        }

        /// <summary>
        /// 批量导出命令
        /// </summary>
        public RelayCommand<object> ExportListCmd { get; private set; }
        private void ExecuteExportListCmd(object obj)
        {
            DataGrid d = (DataGrid)obj;
            List<ExportMeta> l = new List<ExportMeta>();

            if (d.SelectedItems == null || d.SelectedItems.Count == 0)
            {
                MessageBox.Show("您还未选择导出任务");
                return;
            }

            foreach (var item in d.SelectedItems)
            {
                l.Add(item as ExportMeta);
            }

            foreach (var item in l)
            {
                Debug.WriteLine("选择任务：{0}，任务编号：{1}", item.TaskCode, item.VideoId);
            }
            Debug.WriteLine("输出位置:{0}", TargetSource);

            //send Message
            //按钮禁止点击
            //显示进度条
            Messenger.Default.Send("exportIsRunning", "EVM2EV");
            //load metadata abnormaldata abnormaltype from database
            exportDatas = ExportService.GetService().GetExportListData(l);
            typeDict = AbnormalService.GetService().GetAbnormalTypeModelsToDict();
            worker.RunWorkerAsync();
        }

        public RelayCommand FolderBrowserCmd { get; private set; }

        private void ExecuteFolderBrowserCmd()
        {
            TargetSource = FileDialogService.GetService().OpenSaveFileDialog();
        }

        #region helper function
        private void ExecuteLoadedCmd()
        {
            ExportList = MetaService.GetService().SelectAllDetected();
            Way = 0;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Messenger.Default.Send("exportIsFinished", "EVM2EV");
            MessageBox.Show("导出 " + TargetSource + " 已完成");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            switch (Way)
            {
                case 0:
                    SaveService.GetService().SaveXlsxFileBatch(TargetSource, exportDatas, typeDict);
                    break;
                case 1:
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 初始化worker
        /// </summary>
        private void InitWorker()
        {
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
        }


        private void AssignCommands()
        {
            LoadedCmd = new RelayCommand(() => ExecuteLoadedCmd());
            FolderBrowserCmd = new RelayCommand(() => ExecuteFolderBrowserCmd());
            ExportListCmd = new RelayCommand<object>((obj) => ExecuteExportListCmd(obj), CanExecuteExportListCmd);
            CloingCmd = new RelayCommand(() => ExecuteCloingCmd());
        }

        private void InitData()
        {
            Way = 0;
            ExportList = new ObservableCollection<ExportMeta>();
        }
        #endregion
    }

    public class ExportMeta
    {
        public ExportMeta() { }
        public int VideoId { get; set; }
        public string TaskCode { get; set; }
    }

    public class ExportData
    {
        public MetaModel Meta;
        public List<AbnormalModel> AbnormalModels;
    }
}
