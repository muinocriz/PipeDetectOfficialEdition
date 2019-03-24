﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmLight4.Model;
using MvvmLight4.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmLight4.ViewModel
{
    public class ImportViewModel : ViewModelBase
    {
        public ImportViewModel()
        {
            AssignCommands();

            Meta = new MetaModel();
        }

        private MetaModel meta;
        /// <summary>
        /// 元数据
        /// </summary>
        public MetaModel Meta
        {
            get
            {
                return meta;
            }
            set
            {
                meta = value;
                RaisePropertyChanged(() => Meta);
            }
        }

        public RelayCommand SubmitCmd { get; private set; }

        private bool CanExecuteSubmitCmd()
        {
            return !(string.IsNullOrEmpty(Meta.VideoPath) || string.IsNullOrEmpty(Meta.PipeCode) || string.IsNullOrEmpty(Meta.TaskCode) || string.IsNullOrEmpty(Meta.Addr) || string.IsNullOrEmpty(Meta.Charge));
        }

        private void ExecuteSubmitCmd()
        {
            string tag = Meta.PipeCode;
            if(tag.Split('-').Length!=2)
            {
                MessageBox.Show(@"管线编号不正确,应为“起始井号-终止井号”");
                return;
            }
            int result = MetaService.GetService().InsertData(Meta);
            if (result == 1)
            {
                MessageBox.Show("导入成功");
            }
        }

        public RelayCommand OpenFileDialogCmd { get; private set; }

        private void ExecuteOpenFileDialogCmd()
        {
            string filter = @"视频文件|*.avi;*.mp4;*.wmv;*.mpeg|所有文件|*.*";
            Meta.VideoPath = FileDialogService.GetService().OpenFileDialog(filter);
        }

        #region helper function
        private void AssignCommands()
        {
            OpenFileDialogCmd = new RelayCommand(() => ExecuteOpenFileDialogCmd());
            SubmitCmd = new RelayCommand(() => ExecuteSubmitCmd(), CanExecuteSubmitCmd);
        }
        #endregion
    }

}
