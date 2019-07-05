using GalaSoft.MvvmLight;

namespace MvvmLight4.Model
{
    public class ComplexInfoModel : ObservableObject
    {
        private string key;
        /// <summary>
        /// 键
        /// </summary>
        public string Key
        {
            get { return key; }
            set { key = value; RaisePropertyChanged(() => Key); }

        }
        private string text;
        /// <summary>
        /// 值
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; RaisePropertyChanged(() => Text); }
        }
    }
}
