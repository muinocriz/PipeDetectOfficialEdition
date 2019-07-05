using MvvmLight4.Model;
using MvvmLight4.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmLight4.Service
{
    interface IMetaService
    {
        int InsertData(MetaModel meta);
        ObservableCollection<MetaViewModel> SelectAllFramed();
        List<ComplexInfoModel> QueryVideoFramed();
        string QueryFramePathById(int id);
        int UpdateFramePathByVideoPath(string FramePath,string VideoPath);
        ObservableCollection<MetaModel> SelectNotFrame();
        ObservableCollection<ExportMeta> SelectAllDetected();
    }
}
