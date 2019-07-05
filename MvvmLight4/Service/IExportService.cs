using MvvmLight4.ViewModel;
using System.Collections.Generic;

namespace MvvmLight4.Service
{
    interface IExportService
    {
        List<ExportData> GetExportListData(List<ExportMeta> l);
    }
}
