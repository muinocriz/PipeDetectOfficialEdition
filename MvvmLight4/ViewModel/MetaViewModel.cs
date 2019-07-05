using GalaSoft.MvvmLight;
using MvvmLight4.Model;

namespace MvvmLight4.ViewModel
{
    public class MetaViewModel : ViewModelBase
    {
        public MetaViewModel()
        {
            IsChoose = 0;
        }
        private int? isChoose;
        public int? IsChoose { get => isChoose; set { isChoose = value; RaisePropertyChanged(() => IsChoose); } }
        private int? id;
        public int? Id { get { return id; } set { id = value; } }

        private MetaModel meta;
        /// <summary>
        /// 所有模型列表
        /// </summary>
        public MetaModel Meta
        {
            get { return meta; }
            set
            {
                meta = value;
                RaisePropertyChanged(() => Meta);
            }
        }
    }
}
