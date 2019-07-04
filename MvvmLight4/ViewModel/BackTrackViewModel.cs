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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

            DispatcherHelper.Initialize();

            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
        }

        #region property
        private int taskId;
        private CancellationTokenSource tokenSource;
        private CancellationToken token;
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
        private bool stop = false;

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

            Messenger.Default.Send("disEnableDeleteBtn", "BTVM2BTV");
        }

        #region 选择了DataGrid的某一项
        /// <summary>
        /// 选择了DataGrid的某一项
        /// </summary>
        public RelayCommand<AbnormalViewModel> SelectCommand { get; private set; }

        private bool CanExecuteSelectCommand(AbnormalViewModel p)
        {
            return !string.IsNullOrEmpty(p.Meta.FramePath);
        }

        private void ExecuteSelectCommand(AbnormalViewModel p)
        {
            ///修改
            ///将该异常的STATE修改为2000
            AbnormalService.GetService().ChangeState(p.AbnormalId, 2000);

            Messenger.Default.Send("enableDeleteBtn", "BTVM2BTV");
            stop = true;
            //更新左下角显示区域
            SelectedAVM = p;
            CombboxItem = CombboxList[p.Abnormal.Type];

            //播放视频
            //找到文件
            string folderPath = p.Meta.FramePath;

            player = new PlayerModel
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
                player.Calculate(imagePath.Count, 120);

                //播放函数
                var t = new Task(() =>
                {
                    int nowPic = player.StartNum;
                    int end = player.EndNum;
                    stop = false;

                    while (!stop && nowPic <= end)
                    {
                        token.ThrowIfCancellationRequested();
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            ImgSource = imagePath[nowPic];
                        });
                        nowPic++;
                        Thread.Sleep(player.Speed);
                    }
                    stop = true;
                });
                t.Start();
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
        // 删除列表项函数
        public RelayCommand<int> DeleteCmd { get; private set; }

        private bool CanExecuteDeleteCmd(int arg)
        {
            return arg >= 0;
        }

        private void ExecuteDeleteCmd(int p)
        {
            Debug.WriteLine("get abnornal id :" + p);
            Messenger.Default.Send("disEnableDeleteBtn", "BTVM2BTV");
            //修改列表显示
            for (int i = 0; i < AbnormalVMs.Count; i++)
            {
                if (AbnormalVMs[i].AbnormalId == p)
                {
                    Debug.WriteLine("get item in AbnormalVMs :" + p);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        AbnormalVMs.RemoveAt(i);
                    });
                }
            }
            //修改右下角展示区显示
            //重新计算现有的总异常
            ErrorNum = 0;
            foreach (var item in abnormalVMs)
            {
                if (item.Abnormal.Type != 0 && item.Abnormal.Type != 6)
                {
                    ErrorNum++;
                }
            }
            //总数由abnormalVMs.Count直接生成切无需更改
            //删除数据库
            AbnormalService.GetService().DeleteItem(p);
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
            TypeChangedCmd = new RelayCommand(() => ExecuteTypeChangedCmd());
            DeleteCmd = new RelayCommand<int>((p) => ExecuteDeleteCmd(p), CanExecuteDeleteCmd);
        }
        #endregion
    }
}
