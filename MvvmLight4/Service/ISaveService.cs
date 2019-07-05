﻿using MvvmLight4.Model;
using MvvmLight4.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmLight4.Service
{
    interface ISaveService
    {
        void SaveDocxFile(string filePath, Object contain);
        void SaveXlsxFileBatch(string targetSource, List<ExportData> exportDatas, Dictionary<int, AbnormalTypeModel> typeDict);
    }
}
