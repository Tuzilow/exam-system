using ExaminationSystem.Models;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ExaminationSystem.Utils
{
    /// <summary>
    /// Excel帮助类
    /// </summary>
    public static class ExcelHelper
    {
        public static MemoryStream ExportExcel(List<ExportExcelInfo> exportExcelList)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            //Excel文件的摘要信息
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "blog.csdn.net";
            hssfworkbook.DocumentSummaryInformation = dsi;

            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "Export Excel";
            hssfworkbook.SummaryInformation = si;

            //下面代码输出的Excel有三列(姓名、年龄、性别)
            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");

            int i = 0;
            IRow row0 = sheet1.CreateRow(i++);

            row0.CreateCell(0).SetCellValue("编号");
            row0.CreateCell(1).SetCellValue("姓名");
            row0.CreateCell(2).SetCellValue("日期");
            row0.CreateCell(3).SetCellValue("场次");
            row0.CreateCell(4).SetCellValue("试卷");
            row0.CreateCell(5).SetCellValue("分数");
            row0.CreateCell(6).SetCellValue("是否提交");

            foreach (ExportExcelInfo excelInfo in exportExcelList)
            {
                IRow row = sheet1.CreateRow(i++);

                row.CreateCell(0).SetCellValue($"{excelInfo.LogId}");
                row.CreateCell(1).SetCellValue($"{excelInfo.UserName}");
                row.CreateCell(2).SetCellValue($"{excelInfo.Date}");
                row.CreateCell(3).SetCellValue($"{excelInfo.Part}");
                row.CreateCell(4).SetCellValue($"{excelInfo.Title}");
                row.CreateCell(5).SetCellValue($"{excelInfo.Score}");
                row.CreateCell(6).SetCellValue($"{(excelInfo.IsSubmit ? "是" : "否")}");
            }

            MemoryStream ms = new MemoryStream();
            hssfworkbook.Write(ms);


            return ms;
        }
    }
}