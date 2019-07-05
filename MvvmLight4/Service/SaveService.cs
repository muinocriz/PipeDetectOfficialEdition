using System;
using System.Collections.Generic;
using NPOI.XSSF.UserModel;
using System.IO;
using MvvmLight4.ViewModel;
using MvvmLight4.Model;
using System.Diagnostics;
using NPOI.SS.UserModel;

namespace MvvmLight4.Service
{
    class SaveService : ISaveService
    {
        private static SaveService saveService = new SaveService();
        private SaveService() { }
        public static SaveService GetService() { return saveService; }

        /// <summary>
        /// 导出界面
        /// 保存Word文件,暂未实现
        /// </summary>
        /// <param name="filePath">位置</param>
        /// <param name="contain">内容</param>
        public void SaveDocxFile(string filePath, object contain)
        {

        }

        /// <summary>
        /// 导出界面
        /// 批量导出
        /// </summary>
        /// <param name="targetSource">目标地址</param>
        /// <param name="exportDatas">数据</param>
        /// <param name="typeDict">异常类型</param>
        public void SaveXlsxFileBatch(string targetSource, List<ExportData> exportDatas, Dictionary<int, AbnormalTypeModel> typeDict)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet s1 = workbook.CreateSheet("Sheet1");

            //输出标题
            IRow row = s1.CreateRow(0);
            for (int i = 0; i < header.Length; i++)
            {
                ICell Cell = row.CreateCell(i);
                Cell.SetCellValue(header[i]);
            }
            int count = 1;
            for (int i = 0; i < exportDatas.Count; i++)
            {
                for (int j = 0; j < exportDatas[i].AbnormalModels.Count; j++)
                {
                    AbnormalModel abnormalModel = exportDatas[i].AbnormalModels[j];
                    MetaModel metaModel = exportDatas[i].Meta;

                    IRow row1 = s1.CreateRow(count);

                    row1.CreateCell(0).SetCellValue(count);

                    row1.CreateCell(1).SetCellValue(metaModel.TaskCode);
                    row1.CreateCell(2).SetCellValue(metaModel.StartTime);
                    row1.CreateCell(3).SetCellValue(metaModel.Addr);

                    string[]qszh = metaModel.PipeCode.Split('-');
                    if(qszh.Length==2)
                    {
                        row1.CreateCell(4).SetCellValue(metaModel.PipeCode.Split('-')[0]);
                        row1.CreateCell(5).SetCellValue(metaModel.PipeCode.Split('-')[1]);
                    }
                    else
                    {
                        row1.CreateCell(4).SetCellValue("");
                        row1.CreateCell(5).SetCellValue("");
                    }                 

                    int type = metaModel.PipeType;
                    if (type == 1)
                    {
                        row1.CreateCell(9).SetCellValue("雨水");
                    }
                    else if (type == 0)
                    {
                        row1.CreateCell(9).SetCellValue("污水");
                    }
                    else
                    {
                        row1.CreateCell(9).SetCellValue("雨水");
                    }

                    row1.CreateCell(10).SetCellValue(metaModel.GC);

                    row1.CreateCell(17).SetCellValue(abnormalModel.QXWZ);

                    int qxmc = abnormalModel.Type;
                    string name = typeDict[qxmc].Name;
                    row1.CreateCell(18).SetCellValue(name);

                    int qxlb = abnormalModel.Type;
                    string category = typeDict[qxlb].Category;
                    row1.CreateCell(19).SetCellValue(category);

                    string numFormat = string.Format("{0:D6}", Convert.ToInt32(abnormalModel.Position));
                    string picName = metaModel.PipeCode + "-" + numFormat + ".jpg";
                    row1.CreateCell(25).SetCellValue(picName);

                    count += 1;
                }
            }

            FileStream sw = File.Create(targetSource);
            workbook.Write(sw);
            sw.Close();

            ExportImg(targetSource, exportDatas);
            return;
        }

        /// <summary>
        /// 导出界面
        /// 将图片重命名+保存到指定的文件夹
        /// </summary>
        /// <param name="targetSource"></param>
        /// <param name="exportDatas"></param>
        private void ExportImg(string targetSource, List<ExportData> exportDatas)
        {
            string newFolderPath = Path.GetDirectoryName(targetSource) + @"\result";
            for (int i = 0; i < exportDatas.Count; i++)
            {
                string oldFolderPath = exportDatas[i].Meta.FramePath;
                for (int j = 0; j < exportDatas[i].AbnormalModels.Count; j++)
                {
                    int position = Convert.ToInt32(exportDatas[i].AbnormalModels[j].Position);
                    string oldFileName = string.Format("{0:D6}", Convert.ToInt32(position));
                    string oldPath = oldFolderPath + @"\" + oldFileName + ".jpg";

                    string picName = exportDatas[i].Meta.PipeCode + "-" + oldFileName + ".jpg";
                    string newPath = newFolderPath + @"\" + picName;

                    try
                    {
                        //如果不存在文件夹，就创建一个文件夹
                        if (!Directory.Exists(newFolderPath))
                        {
                            Directory.CreateDirectory(newFolderPath);
                        }
                        File.Copy(oldPath, newPath, true);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        Debug.WriteLine("异常发生于 {0} 向 {1} 复制的过程中", oldPath, newPath);
                        continue;
                    }
                }
            }
            return;
        }

        public string[] header = {
        "序号",
        "项目名称",
        "项目时间",
        "道路名称",
        "起始井号",
        "终止井号",
        "有无管线点",
        "管线点点号",
        "管线点个数",
        "管线类型",
        "管材",
        "管径",
        "检测方向",
        "管长(m)",
        "起始埋深(m)",
        "终止埋深(m)",
        "切片位置",
        "缺陷位置(m)",
        "缺陷名称",
        "缺陷类别",
        "等级",
        "时钟表示",
        "备注",
        "是否修复",
        "描述",
        "图片名称"};
    }
}
