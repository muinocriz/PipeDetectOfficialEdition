using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MvvmLight4.Common;
using MvvmLight4.Model;
using MvvmLight4.ViewModel;

namespace MvvmLight4.Service
{
    public class AbnormalService : IAbnormalService
    {
        private static AbnormalService abnormalService = new AbnormalService();
        private AbnormalService() { }
        public static AbnormalService GetService()
        {
            return abnormalService;
        }

        /// <summary>
        /// 人工回溯界面
        /// 获得要显示在人工回溯界面的所有异常
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<AbnormalViewModel> SelectAll(List<int> list)
        {
            ObservableCollection<AbnormalViewModel> avms = new ObservableCollection<AbnormalViewModel>();
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT ADDR,PIPECODE,PIPETYPE,STARTTIME,TB_ABNORMAL.TYPE,POSITION,TB_ABNORMAL.ID AS TID,VIDEOID AS VID,FRAMEPATH 
                            FROM TB_ABNORMAL,TB_METADATA 
                            WHERE TB_METADATA.ID IN @ids AND TB_ABNORMAL.VIDEOID = TB_METADATA.ID;";
                IEnumerable<dynamic> dynamics = conn.Query(sql, new { ids = list.ToArray() });
                foreach (var item in dynamics)
                {
                    MetaModel mm = new MetaModel();
                    AbnormalModel am = new AbnormalModel();
                    AbnormalViewModel avm = new AbnormalViewModel();

                    mm.Addr = item.ADDR;
                    mm.PipeCode = item.PIPECODE;
                    mm.PipeType = (int)item.PIPETYPE;
                    mm.FramePath = item.FRAMEPATH;

                    if (!string.IsNullOrEmpty(item.STARTTIME))
                        mm.StartTime = item.STARTTIME;
                    else
                        mm.StartTime = "未填写";

                    am.VideoId = (int)item.VID;
                    am.Type = (int)item.TYPE;
                    am.Position = item.POSITION;
                    avm.AbnormalId = (int)item.TID;

                    avm.Meta = mm;
                    avm.Abnormal = am;
                    avms.Add(avm);
                }
            }
            return avms;
        }

        /// <summary>
        /// 根据异常类里的视频id找到元数据里视频分帧之后存放的文件夹名字
        /// </summary>
        /// <param name="id">视频id</param>
        /// <returns>该视频分帧文件夹</returns>
        public string SelectFolder(int id)
        {
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT FRAMEPATH FROM TB_METADATA WHERE ID = @ID;";
                var result = conn.Query(sql, new { ID = id }).FirstOrDefault();
                return result.FRAMEPATH;
            }
        }

        /// <summary>
        /// 人工回溯界面
        /// 更新异常类型
        /// </summary>
        /// <param name="id">异常所在的数据库ID</param>
        /// <param name="type">异常类型</param>
        /// <returns></returns>
        public int UpdateAbnormalType(int id, int type)
        {
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                var sql = @"UPDATE TB_ABNORMAL SET TYPE = @type WHERE ID = @id";
                return conn.Execute(sql, new { type, id });
            }
        }

        public List<ComplexInfoModel> QueryVideo()
        {
            List<ComplexInfoModel> l = new List<ComplexInfoModel>();
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT DISTINCT VIDEOID,TASKCODE FROM TB_METADATA,TB_ABNORMAL WHERE TB_METADATA.ID = TB_ABNORMAL.VIDEOID ORDER BY VIDEOID DESC;";
                IEnumerable<dynamic> dynamics = conn.Query(sql);
                foreach (var item in dynamics)
                {
                    ComplexInfoModel c = new ComplexInfoModel();
                    c.Key = Convert.ToString(item.VIDEOID);
                    c.Text = item.TASKCODE;
                    l.Add(c);
                }
            }
            return l;
        }

        /// <summary>
        /// 导出界面
        /// 获得某个视频的元信息和所有检测结果信息
        /// </summary>
        /// <param name="id">视频id</param>
        /// <returns></returns>
        public List<AbnormalViewModel> ExportByVideoId(int id)
        {
            List<AbnormalViewModel> list = new List<AbnormalViewModel>();
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT * FROM TB_ABNORMAL,TB_METADATA WHERE TB_ABNORMAL.VIDEOID=TB_METADATA.ID AND TB_ABNORMAL.VIDEOID=@id;";
                IEnumerable<dynamic> dynamics = conn.Query(sql, new { id });
                foreach (var item in dynamics)
                {
                    MetaModel mm = new MetaModel();
                    AbnormalModel am = new AbnormalModel();
                    AbnormalViewModel avm = new AbnormalViewModel();
                    mm.PipeCode = item.PIPECODE;
                    mm.PipeType = (int)item.PIPETYPE;
                    mm.TaskCode = item.TASKCODE;
                    mm.FramePath = item.FRAMEPATH;
                    mm.Addr = item.ADDR;
                    mm.Charge = item.CHARGE;
                    mm.StartTime = item.STARTTIME;
                    am.VideoId = (int)item.VIDEOID;
                    am.Type = (int)item.TYPE;
                    am.Position = item.POSITION;
                    avm.Meta = mm;
                    avm.Abnormal = am;
                    list.Add(avm);

                }
            }
            return list;
        }

        /// <summary>
        /// 检测界面
        /// 将获得的异常批量存入数据库
        /// </summary>
        /// <param name="abnormalModels">异常类型</param>
        /// <returns>存入数量</returns>
        public int AddAbnormal(List<AbnormalModel> abnormalModels)
        {
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    var sql = @"INSERT INTO TB_ABNORMAL(VIDEOID,POSITION,TYPE,QXWZ,STATE,TASKID) VALUES(@VideoId,@Position,@Type,@QXWZ,@State,@TaskId);";
                    var executeResult = conn.Execute(sql, abnormalModels, trans);
                    trans.Commit();
                    return executeResult;
                }
            }
        }

        /// <summary>
        /// 导出界面
        /// 查找所有异常类型，存入字典中
        /// key:    Type
        /// value:  AbnormalTypeModel
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, AbnormalTypeModel> GetAbnormalTypeModelsToDict()
        {
            Dictionary<int, AbnormalTypeModel> dics = new Dictionary<int, AbnormalTypeModel>();
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT * FROM TB_ABNORMALLIST;";
                IEnumerable<dynamic> dynamics = conn.Query<AbnormalTypeModel>(sql).ToList();
                foreach (var item in dynamics)
                {
                    dics[item.Type] = item;
                }
            }
            return dics;
        }

        /// <summary>
        /// 设置界面
        /// 获取异常类型列表
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<AbnormalTypeModel> GetAbnormalTypeModels()
        {
            ObservableCollection<AbnormalTypeModel> models = new ObservableCollection<AbnormalTypeModel>();
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT * FROM TB_ABNORMALLIST;";
                IEnumerable<dynamic> dynamics = conn.Query<AbnormalTypeModel>(sql).ToList();
                foreach (var item in dynamics)
                {
                    models.Add(item);
                }
            }
            return models;
        }

        /// <summary>
        /// 人工回溯界面
        /// 通过异常id删除一条异常列表信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteItem(int id)
        {
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                var sql = @"DELETE FROM TB_ABNORMAL WHERE ID=@id;";
                int result = conn.Execute(sql, new { id });
                return result;
            }
        }

        public ObservableCollection<AbnormalViewModel> SelectAllWithoutWatch(List<int> TaskIds)
        {
            ObservableCollection<AbnormalViewModel> avms = new ObservableCollection<AbnormalViewModel>();
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT ADDR,PIPECODE,PIPETYPE,STARTTIME,TB_ABNORMAL.TYPE,
                                POSITION,TB_ABNORMAL.ID AS TID,VIDEOID AS VID,
                                FRAMEPATH,STATE,TASKID 
                            FROM TB_ABNORMAL,TB_METADATA 
                            WHERE TASKID IN @ids 
                                AND STATE==1000 
                                AND VID=TB_METADATA.ID;";
                IEnumerable<dynamic> dynamics = conn.Query(sql, new { ids = TaskIds.ToArray() });
                foreach (var item in dynamics)
                {
                    MetaModel mm = new MetaModel();
                    AbnormalModel am = new AbnormalModel();
                    AbnormalViewModel avm = new AbnormalViewModel();

                    mm.Addr = item.ADDR;
                    mm.PipeCode = item.PIPECODE;
                    mm.PipeType = (int)item.PIPETYPE;
                    mm.FramePath = item.FRAMEPATH;

                    if (!string.IsNullOrEmpty(item.STARTTIME))
                        mm.StartTime = item.STARTTIME;
                    else
                        mm.StartTime = "未填写";

                    am.VideoId = (int)item.VID;
                    am.Type = (int)item.TYPE;
                    am.Position = item.POSITION;

                    //新加的状态和任务编号
                    am.State = (int)item.STATE;
                    am.TaskId = (int)item.TASKID;

                    avm.AbnormalId = (int)item.TID;
                    Debug.WriteLine("get tid: " + avm.AbnormalId);
                    avm.Meta = mm;
                    avm.Abnormal = am;
                    avms.Add(avm);
                }
            }
            return avms;
        }

        /// <summary>
        /// 人工回溯界面
        /// 修改异常的查看状态
        /// </summary>
        /// <param name="abnormalId">TB_ABNORMAL的id</param>
        /// <param name="state">要修改到的状态</param>
        public void ChangeState(int abnormalId, int state)
        {
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                if(state==2000)
                {
                    var sql = @"UPDATE TB_ABNORMAL SET STATE = @state WHERE ID = @abnormalId AND STATE!=3000;";
                    conn.Execute(sql, new { state, abnormalId });
                }
                else
                {
                    var sql = @"UPDATE TB_ABNORMAL SET STATE = @state WHERE ID = @abnormalId;";
                    conn.Execute(sql, new { state, abnormalId });
                }
                
                Debug.WriteLine(abnormalId + " has changed to " + state);
            }
        }

        /// <summary>
        /// 开始界面
        /// 通过任务id查看是否有未观看的项目
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public int SearchLastTaskById(int taskId)
        {
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT COUNT(*) AS COUNT FROM TB_ABNORMAL WHERE TASKID = @taskId AND STATE == 1000;";
                var result = conn.Query(sql, new { taskId }).SingleOrDefault();
                if(result!=null || result.COUNT != null)
                {
                    int a = Convert.ToInt32(result.COUNT);
                    return a;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
