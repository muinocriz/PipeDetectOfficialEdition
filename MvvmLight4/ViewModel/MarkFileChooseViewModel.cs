﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MvvmLight4.Model;
using MvvmLight4.Service;
using MvvmLight4.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmLight4.ViewModel
{
    public class MarkFileChooseViewModel : ViewModelBase
    {
        public MarkFileChooseViewModel()
        {
            InitData();
        }

        #region 属性
        public string[] StringMessage;
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
        private string savePath;
        public string SavePath { get => savePath; set { savePath = value; RaisePropertyChanged(() => SavePath); } }
        #endregion

        #region 命令
        private RelayCommand<string> folderBrowserCmd;
        public RelayCommand<string> FolderBrowserCmd
        {
            get
            {
                if (folderBrowserCmd == null)
                    return new RelayCommand<string>((p) => ExecuteFolderBrowserCmd(p));
                return folderBrowserCmd;
            }
            set
            {
                FolderBrowserCmd = value;
            }
        }

        private void ExecuteFolderBrowserCmd(string p)
        {
            string path = FileDialogService.GetService().OpenFolderBrowserDialog();
            if (!string.IsNullOrEmpty(path))
                SavePath = path;
        }

        private RelayCommand submitCmd;
        /// <summary>
        /// 点击确定
        /// </summary>
        public RelayCommand SubmitCmd
        { 
            get
            {
                if (submitCmd == null)
                    return new RelayCommand(() => ExecuteSubmitCmd(),CanExecuteSubmitCmd);
                return submitCmd;
            }
            set
            {
                SubmitCmd = value;
            }
        }

        private bool CanExecuteSubmitCmd()
        {
            return CombboxItem != null && !string.IsNullOrEmpty(SavePath);
        }

        private void ExecuteSubmitCmd()
        {
            string framePath = MetaService.GetService().QueryFramePathById(Convert.ToInt32(CombboxItem.Key));
            StringMessage[0] = framePath;
            StringMessage[1] = SavePath;
            ViewModelLocator.FolderPath = StringMessage[0];
            ViewModelLocator.SavePath = StringMessage[1];
            MarkWindow mark = new MarkWindow();
            
            mark.Show();
            //Messenger.Default.Send<String[]>(StringMessage, "MFSVM2MVM");


        }
        #endregion

        #region 辅助方法
        private void InitData()
        {
            CombboxList = MetaService.GetService().QueryVideoFramed();
            if (CombboxList.Count > 0)
            {
                CombboxItem = CombboxList[0];
            }

            StringMessage = new string[2];
            //默认保存路径为桌面
            SavePath= Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); 
        }
        #endregion
    }
}