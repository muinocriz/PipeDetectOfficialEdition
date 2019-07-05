using MvvmLight4.Model;
using MvvmLight4.ViewModel;
using System.Collections.ObjectModel;

namespace MvvmLight4.Service
{
    interface IModelService
    {
        ObservableCollection<ModelViewModel> LoadData();
        int UpdateModel(ModelViewModel modelViewModel);
        int DeleteModel(ModelViewModel modelViewModel);
        int AddModel(ModelModel modelModel);
    }
}
