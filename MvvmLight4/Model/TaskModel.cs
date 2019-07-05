namespace MvvmLight4.Model
{
    /// <summary>
    /// 检测任务
    /// </summary>
    public class TaskModel
    {
        public TaskModel() { }
        private int id;
        private string vListJSON;
        private string dTime;
        private int count;
        /// <summary>
        /// 任务编号
        /// </summary>
        public int Id { get => id; set => id = value; }

        /// <summary>
        /// 任务包含视频列表
        /// </summary>
        public string VListJSON { get => vListJSON; set => vListJSON = value; }

        /// <summary>
        /// 任务完成时间
        /// </summary>
        public string DTime { get => dTime; set => dTime = value; }

        /// <summary>
        /// 任务检测出异常个数
        /// </summary>
        public int Count { get => count; set => count = value; }
    }
}
