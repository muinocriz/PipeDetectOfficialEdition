using MvvmLight4.Model;
using MvvmLight4.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MvvmLight4.Service
{
    public interface IAbnormalService
    {
        ObservableCollection<AbnormalViewModel> SelectAllWithoutWatch(List<int> TaskIds);
        int UpdateAbnormalType(int id, int type);
        int AddAbnormal(List<AbnormalModel> abnormalModels);
        ObservableCollection<AbnormalTypeModel> GetAbnormalTypeModels();
        int DeleteItem(int id);
        void ChangeState(int abnormalId, int state);
        int SearchLastTaskById(int taskId);
        int AddItem(int id);
    }
}
