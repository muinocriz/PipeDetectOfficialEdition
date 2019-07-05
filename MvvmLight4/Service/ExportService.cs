using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmLight4.Common;
using Dapper;
using MvvmLight4.Model;
using MvvmLight4.ViewModel;

namespace MvvmLight4.Service
{
    class ExportService : IExportService
    {
        private static ExportService exportService = new ExportService();
        private ExportService() { }
        public static ExportService GetService() { return exportService; }

        /// <summary>
        /// 导出界面
        /// 根据选择的列表，查到
        /// 输出辅助VM类
        /// --元信息
        /// --每个元信息对应的异常列表
        /// </summary>
        /// <param name="l">导出任务编号列表</param>
        /// <returns></returns>
        public List<ExportData> GetExportListData(List<ExportMeta> l)
        {
            List<ExportData> exportDatas = new List<ExportData>();
            using (IDbConnection conn = SqlHelper.GetConnection())
            {
                conn.Open();
                foreach (var item in l)
                {
                    ExportData exportData = new ExportData();

                    MetaModel meta = new MetaModel();
                    var sqlMeta = @"SELECT * FROM TB_METADATA WHERE ID=@VideoId";
                    meta = conn.Query<MetaModel>(sqlMeta, new { item.VideoId }).SingleOrDefault();
                    exportData.Meta = meta;

                    List<AbnormalModel> abnormalModels = new List<AbnormalModel>();
                    var sqlAbn = @"SELECT * FROM TB_ABNORMAL WHERE VideoId=@VideoId";
                    abnormalModels = conn.Query<AbnormalModel>(sqlAbn, new { item.VideoId }).ToList();
                    exportData.AbnormalModels = abnormalModels;
                    exportDatas.Add(exportData);
                }
            }
            return exportDatas;
        }
    }
}
