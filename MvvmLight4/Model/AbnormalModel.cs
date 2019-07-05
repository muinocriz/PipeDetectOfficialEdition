using GalaSoft.MvvmLight;

namespace MvvmLight4.Model
{
    /// <summary>
    /// 异常具体信息
    /// </summary>
    public class AbnormalModel : ObservableObject
    {
        public AbnormalModel() { }
        public AbnormalModel(int _videoId, string _position, int _type)
        {
            VideoId = _videoId;
            Position = _position;
            Type = _type;
        }

        public AbnormalModel(int _videoId, string _position, int _type, double _qxwz)
        {
            VideoId = _videoId;
            Position = _position;
            Type = _type;
            QXWZ = _qxwz;
        }
        public AbnormalModel(int _videoId, string _position, int _type, double _qxwz, int _taskId)
        {
            VideoId = _videoId;
            Position = _position;
            Type = _type;
            QXWZ = _qxwz;
            TaskId = _taskId;
            State = 1000;
        }
        private int videoId;
        /// <summary>
        /// 异常所属于的视频的Id
        /// </summary>
        public int VideoId { get => videoId; set { videoId = value; RaisePropertyChanged(() => VideoId); } }

        private int type;
        /// <summary>
        /// 异常类型
        /// 0 - 无异常
        /// 其他 - 其他异常
        /// </summary>
        public int Type { get => type; set { type = value; RaisePropertyChanged(() => Type); } }

        private string qpwz;
        /// <summary>
        /// 切片位置
        /// </summary>
        public string QPWZ
        {
            get { return qpwz; }
            set { qpwz = value; }
        }

        private string position;
        /// <summary>
        /// 异常所在帧号
        /// </summary>
        public string Position { get => position; set { position = value; RaisePropertyChanged(() => Position); } }

        private double qxwz;
        /// <summary>
        /// 缺陷位置
        /// 单位：m
        /// </summary>
        public double QXWZ
        {
            get { return qxwz; }
            set { qxwz = value; }
        }

        private int dj;
        /// <summary>
        /// 等级
        /// </summary>
        public int DJ
        {
            get { return dj; }
            set { dj = value; }
        }

        private string szbs;
        /// <summary>
        /// 时钟表示
        /// </summary>
        public string SZBS
        {
            get { return szbs; }
            set { szbs = value; }
        }

        private string bz;
        /// <summary>
        /// 备注
        /// </summary>
        public string BZ
        {
            get { return bz; }
            set { bz = value; }
        }

        private string sfxf;
        /// <summary>
        /// 是否修复
        /// </summary>
        public string SFXF
        {
            get { return sfxf; }
            set { sfxf = value; }
        }

        private string ms;
        /// <summary>
        /// 描述
        /// </summary>
        public string MS
        {
            get { return ms; }
            set { ms = value; }
        }

        private string tpmc;
        /// <summary>
        /// 图片名称
        /// </summary>
        public string TPMC
        {
            get { return tpmc; }
            set { tpmc = value; }
        }


        private int taskId;
        /// <summary>
        /// 本次异常所属的任务
        /// </summary>
        public int TaskId { get => taskId; set => taskId = value; }

        private int state;
        /// <summary>
        /// 异常当前的状态
        /// 1000：默认状态，被检测但未被查看
        /// 2000：已被查看
        /// 3000：已被修改
        /// </summary>
        public int State { get => state; set => state = value; }
    }
}
