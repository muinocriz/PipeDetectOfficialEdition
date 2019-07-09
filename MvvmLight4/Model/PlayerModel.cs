using GalaSoft.MvvmLight;
using System;
using System.Configuration;

namespace MvvmLight4.Model
{
    public class PlayerModel : ObservableObject
    {
        private int target;
        /// <summary>
        /// 目标帧号
        /// </summary>
        public int Target { get => target; set { target = value; RaisePropertyChanged(() => Target); } }
        private int speed;
        /// <summary>
        /// 模仿速度
        /// </summary>
        public int Speed { get => speed; set { speed = value; RaisePropertyChanged(() => Speed); } }
        private string folder;
        /// <summary>
        /// 该帧所在的文件夹绝对路径
        /// </summary>
        public string Folder { get => folder; set { folder = value; RaisePropertyChanged(() => Folder); } }
        private int startNum;
        /// <summary>
        /// 开始帧帧号
        /// </summary>
        public int StartNum { get => startNum; set { startNum = value; RaisePropertyChanged(() => StartNum); } }
        private int endNum;
        /// <summary>
        /// 结束帧帧号
        /// </summary>
        public int EndNum { get => endNum; set { endNum = value; RaisePropertyChanged(() => EndNum); } }

        public PlayerModel()
        {
            Target = 0;
            Speed = 40;
            Folder = "";
        }
        //计算StartNum和EndNum
        public void Calculate(int length)
        {
            string frameTimeString = ConfigurationManager.AppSettings["FrameTime"];
            int duration = string.IsNullOrEmpty(frameTimeString) ? 250 : Convert.ToInt32(frameTimeString) * 25;
            if (length < duration)
            {
                StartNum = 0;
                EndNum = length - 1;
            }
            else if (length - Target < (duration / 2))
            {
                StartNum = Target - duration / 2;
                EndNum = length - 1;
            }
            else if (Target < duration / 2)
            {
                StartNum = 0;
                EndNum = Target + duration / 2;
            }
            else
            {
                StartNum = Target - duration / 2;
                EndNum = Target + duration / 2;
            }
            Console.WriteLine("Target：" + Target+ "\tStartNum：" + StartNum+ "\tEndNum：" + EndNum);
            Console.WriteLine("持续时间："+(EndNum-StartNum)/25+"秒");
        }
    }
}
