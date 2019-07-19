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
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace MvvmLight4.ViewModel
{
    public class BackTrackViewModel : ViewModelBase
    {
        public BackTrackViewModel()
        {
            AssignCommands();

            Messenger.Default.Register<List<int>>(this, "DVM2BTVM", msg =>
            {
                Debug.WriteLine("backtrack ViewModel get taskID: " + msg.ToString());
                TaskIds = msg;
            });
            InitCombobox();
            InitWorker();
            DispatcherHelper.Initialize();
        }

        #region property
        /// <summary>
        /// 后台进程
        /// </summary>
        public BackgroundWorker worker;
        /// <summary>
        /// 图像列表
        /// </summary>
        public List<string> imagePath = new List<string>();
        /// <summary>
        /// 视频列表
        /// </summary>
        public List<int> VideoIds = new List<int>();
        /// <summary>
        /// 任务列表
        /// </summary>
        public List<int> TaskIds = new List<int>();
        private PlayerModel player;
        /// <summary>
        /// 播放类
        /// </summary>
        public PlayerModel Player { get { return player; } set { player = value; RaisePropertyChanged(() => Player); } }
        private string imgSource;
        /// <summary>
        /// 播放文件的位置
        /// </summary>
        public string ImgSource { get { return imgSource; } set { imgSource = value; RaisePropertyChanged(() => ImgSource); } }
        #region 右下展示板
        //总数由abnormalVMs.Count直接生成切无需更改
        private int errorNum;
        /// <summary>
        /// 异常数目
        /// </summary>
        public int ErrorNum { get { return errorNum; } set { errorNum = value; RaisePropertyChanged(() => ErrorNum); } }
        private int changeNum;
        /// <summary>
        /// 修改数目
        /// </summary>
        public int ChangeNum { get { return changeNum; } set { changeNum = value; RaisePropertyChanged(() => ChangeNum); } }
        #endregion
        #region 列表区域
        private AbnormalViewModel selectedAVM;
        /// <summary>
        /// 被选中的项
        /// 用于显示视频下方的详细信息和对异常类型进行修改
        /// </summary>
        public AbnormalViewModel SelectedAVM
        {
            get
            {
                return selectedAVM;
            }
            set
            {
                selectedAVM = value;
                RaisePropertyChanged(() => SelectedAVM);
            }
        }
        private ObservableCollection<AbnormalViewModel> abnormalVMs;
        /// <summary>
        /// 带有元信息的异常列表
        /// </summary>
        public ObservableCollection<AbnormalViewModel> AbnormalVMs
        {
            get
            {
                return abnormalVMs;
            }
            set
            {
                abnormalVMs = value;
                RaisePropertyChanged(() => AbnormalVMs);
            }
        }
        #endregion
        #region 下拉框相关
        private List<ComplexInfoModel> combboxList;
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
        #endregion
        #endregion

        /// <summary>
        /// 界面加载时函数
        /// </summary>
        public RelayCommand LoadedCmd { get; private set; }
        private void ExecuteLoadedCmd()
        {
            SelectAllWithoutWatch();
            CheckWorkerState();
            Messenger.Default.Send("disEnableDeleteBtn", "BTVM2BTV");
        }

        private void CheckWorkerState()
        {
            if (worker != null && worker.IsBusy)
            {
                worker.CancelAsync();
            }
        }

        void SelectAllWithoutWatch()
        {
            ///修改
            ///为了考虑以后可能传递多个taskId
            ///将它存入List<int>中，方便修改
            AbnormalVMs = AbnormalService.GetService().SelectAllWithoutWatch(TaskIds);
            ErrorNum = 0;

            foreach (var item in AbnormalVMs)
            {
                if (item.Abnormal.Type != 0 && item.Abnormal.Type != 6)
                {
                    ErrorNum++;
                }
            }
        }

        public RelayCommand ClosedCmd { get; private set; }
        public void ExecuteClosedCmd()
        {
            CheckWorkerState();
        }

        #region 选择了DataGrid的某一项
        /// <summary>
        /// 选择了DataGrid的某一项
        /// </summary>
        public RelayCommand<AbnormalViewModel> SelectCommand { get; private set; }

        private bool CanExecuteSelectCommand(AbnormalViewModel p)
        {
            CheckWorkerState();

            return !string.IsNullOrEmpty(p.Meta.FramePath);
        }

        private void ExecuteSelectCommand(AbnormalViewModel p)
        {
            Debug.WriteLine("worker.isBusy:" + worker.IsBusy);
            if (worker.IsBusy)
            {
                return;
            }

            //将该异常的STATE修改为2000
            AbnormalService.GetService().ChangeState(p.AbnormalId, 2000);

            Messenger.Default.Send("enableDeleteBtn", "BTVM2BTV");

            //更新左下角显示区域
            SelectedAVM = p;
            CombboxItem = CombboxList[p.Abnormal.Type];

            //播放视频
            //找到文件
            InitPlayer(p);
            worker.RunWorkerAsync(Player);
        }
        void InitPlayer(AbnormalViewModel p)
        {
            string folderPath = p.Meta.FramePath;

            Player = new PlayerModel
            {
                Target = Convert.ToInt32(p.Abnormal.Position)//目标帧号
            };
            try
            {
                DirectoryInfo root = new DirectoryInfo(folderPath);
                FileInfo[] files = root.GetFiles("*.jpg");
                files = files.OrderBy(y => y.Name, new FileComparer()).ToArray();
                foreach (var item in files)
                {
                    string name = item.FullName;
                    imagePath.Add(name);
                }
                Player.Calculate(imagePath.Count);
            }
            catch (PathTooLongException)
            {
                MessageBox.Show("文件路径过长");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("该路径下文件错误");
            }
            catch (Exception e)
            {
                MessageBox.Show("发生异常：" + e.ToString());
            }
        }

        public RelayCommand<int> DeleteCmd { get; private set; }

        private bool CanExecuteDeleteCmd(int arg)
        {
            return SelectedAVM != null && arg >= 0;
        }
        /// <summary>
        /// 删除选择项
        /// </summary>
        /// <param name="p"></param>
        private void ExecuteDeleteCmd(int p)
        {
            CheckWorkerState();
            Debug.WriteLine("get abnornal id :" + p);
            Messenger.Default.Send("disEnableDeleteBtn", "BTVM2BTV");

            bool isAbnormal = true;

            //修改列表显示
            for (int i = 0; i < AbnormalVMs.Count; i++)
            {
                if (AbnormalVMs[i].AbnormalId == p)
                {
                    Debug.WriteLine("get item in AbnormalVMs :" + p);
                    if (AbnormalVMs[i].Abnormal.Type == 0 || AbnormalVMs[i].Abnormal.Type == 6)
                    {
                        isAbnormal = false;
                    }
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        AbnormalVMs.RemoveAt(i);
                    });
                }
            }
            //修改右下角展示区显示
            //重新计算现有的总异常
            if (isAbnormal)
            {
                ErrorNum--;
            }
            //总数由abnormalVMs.Count直接生成切无需更改
            //删除数据库
            AbnormalService.GetService().DeleteItem(p);
        }
        #endregion
        #region 添加一项异常
        public RelayCommand<int> AddCmd { get; private set; }
        public bool CanExecuteAddCmd(int arg)
        {
            return SelectedAVM != null && arg >= 0;
        }
        public void ExecuteAddCmd(int p)
        {
            CheckWorkerState();
            Debug.WriteLine("get abnornal id :" + p);

            //修改数据库
            int newId = AbnormalService.GetService().AddItem(p);

            bool isAbnormal = true;



            //修改列表显示
            for (int i = 0; i < AbnormalVMs.Count; i++)
            {
                if (AbnormalVMs[i].AbnormalId == p)
                {
                    Debug.WriteLine("get item in AbnormalVMs :" + p);

                    if (AbnormalVMs[i].Abnormal.Type == 0 || AbnormalVMs[i].Abnormal.Type == 6)
                    {
                        isAbnormal = false;
                    }

                    AbnormalViewModel abnormalViewModel = AbnormalVMs[i];
                    

                    MetaModel mm = new MetaModel();
                    AbnormalModel am = new AbnormalModel();
                    AbnormalViewModel avm = new AbnormalViewModel();

                    mm.Addr = abnormalViewModel.Meta.Addr;
                    mm.PipeCode = abnormalViewModel.Meta.PipeCode;
                    mm.PipeType = (int)abnormalViewModel.Meta.PipeType;
                    mm.FramePath = abnormalViewModel.Meta.FramePath;
                    if (!string.IsNullOrEmpty(abnormalViewModel.Meta.StartTime))
                        mm.StartTime = abnormalViewModel.Meta.StartTime;
                    else
                        mm.StartTime = "未填写";

                    am.VideoId = (int)abnormalViewModel.Abnormal.VideoId;
                    am.Type = (int)abnormalViewModel.Abnormal.Type;
                    am.Position = abnormalViewModel.Abnormal.Position;
                    //新加的状态和任务编号
                    am.State = 1000;
                    am.TaskId = (int)abnormalViewModel.Abnormal.TaskId;

                    avm.AbnormalId = newId;
                    avm.Meta = mm;
                    avm.Abnormal = am;

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        AbnormalVMs.Insert(i, avm);
                    });
                    break;
                }
            }


            //修改右下角展示区显示
            //重新计算现有的总异常
            if (isAbnormal)
            {
                ErrorNum++;
            }

        }

        #endregion
        #region ComboxItem被更改
        /// <summary>
        /// ComboxItem被更改
        /// </summary>
        public RelayCommand TypeChangedCmd { get; private set; }

        private void ExecuteTypeChangedCmd()
        {
            ///修改
            ///将该异常的STATE修改为2000
            AbnormalService.GetService().ChangeState(SelectedAVM.AbnormalId, 3000);

            //修改数据库
            int type = Convert.ToInt32(CombboxItem.Key);
            int result = AbnormalService.GetService().UpdateAbnormalType(SelectedAVM.AbnormalId, type);
            //更改右下角显示
            if (result == 1)
            {
                ChangeNum++;

                SelectedAVM.Abnormal.Type = type;

                //重新计算现有的总异常
                ErrorNum = 0;
                foreach (var item in abnormalVMs)
                {
                    if (item.Abnormal.Type != 0 && item.Abnormal.Type != 6)
                    {
                        ErrorNum++;
                    }
                }
            }
        }

        public bool CanExecuteTypeChangedCmd()
        {
            return SelectedAVM != null;
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 初始化下拉列表
        /// </summary>
        private void InitCombobox()
        {
            CombboxList = new List<ComplexInfoModel>();

            IEnumerable<AbnormalTypeModel> dynamics = AbnormalService.GetService().GetAbnormalTypeModels();
            foreach (var item in dynamics)
            {
                CombboxList.Add(new ComplexInfoModel() { Key = Convert.ToString(item.Type), Text = item.Name });
            }
        }

        private void AssignCommands()
        {
            LoadedCmd = new RelayCommand(() => ExecuteLoadedCmd());
            SelectCommand = new RelayCommand<AbnormalViewModel>((p) => ExecuteSelectCommand(p), CanExecuteSelectCommand);
            TypeChangedCmd = new RelayCommand(() => ExecuteTypeChangedCmd(), CanExecuteTypeChangedCmd);
            DeleteCmd = new RelayCommand<int>((p) => ExecuteDeleteCmd(p), CanExecuteDeleteCmd);
            ClosedCmd = new RelayCommand(() => ExecuteClosedCmd());
            AddCmd = new RelayCommand<int>((arg) => ExecuteAddCmd(arg), CanExecuteAddCmd);
        }

        public void InitWorker()
        {
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            PlayerModel p = (PlayerModel)e.Argument;
            int nowPic = p.StartNum;
            int end = p.EndNum;

            while (nowPic <= end)
            {
                if (worker.CancellationPending)
                {
                    Debug.WriteLine("收到取消信号");
                    e.Cancel = true;
                    return;
                }
                worker.ReportProgress(nowPic++);
                Thread.Sleep(p.Speed);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ImgSource = imagePath[e.ProgressPercentage];
            });
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                Debug.WriteLine("本条目回溯中断");
            else
                Debug.WriteLine("本条目回溯完成");
        }
        #endregion
    }
}
