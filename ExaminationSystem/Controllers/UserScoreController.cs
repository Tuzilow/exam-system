using ExaminationSystem.Models;
using ExaminationSystem.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExaminationSystem.Controllers
{
    public class UserScoreController : Controller
    {
        readonly ExaminationSystemDbEntities db = new ExaminationSystemDbEntities();

        /// <summary>
        /// 根据场次ID获取学生成绩列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetUserScore(int pageIndex = 1, string keyword = "", int ptId = 0, bool isPaging = true)
        {
            int code;
            string message;
            try
            {
                var logs = from l in db.ES_ExamLog
                           where l.IsDel == false
                           join u in db.ES_User on l.UserId equals u.UserId
                           join pt in db.ES_ExamPart on l.EmPtId equals pt.EmPtId
                           join pr in db.ES_ExamPaper on l.EmPaperId equals pr.EmPaperId
                           where pt.IsDel == false && pr.IsDel == false && u.IsDel == false
                           select new
                           {
                               u.UserId,
                               u.UserName,
                               l.LogId,
                               l.IsSubmit,
                               l.ExercisesId,
                               l.Answers,
                               l.ExamScore,
                               l.EmPtId,
                               l.EmPaperId,
                               pt.EmPtStart,
                               pt.EmPtEnd,
                               pr.EmPaperName
                           };

                if (ptId != null && ptId != 0)
                {
                    logs = logs.Where(l => l.EmPtId == ptId);
                }

                if (keyword != "")
                {
                    logs = logs.Where(l => l.UserName.Contains(keyword));
                }

                int totalCount = logs.Count();
                if (isPaging)
                {
                    logs = logs.OrderBy(l => l.LogId).Skip((pageIndex - 1) * 10).Take(10);
                }

                List<object> logList = new List<object>();

                foreach (var log in logs)
                {
                    int userId = log.UserId;
                    string userName = log.UserName;
                    int logId = log.LogId;
                    string isSubmit = log.IsSubmit == false ? "否" : "是";
                    string exercisesId = log.ExercisesId;
                    string answers = log.Answers;
                    double score = log.ExamScore;
                    int partId = Convert.ToInt32(log.EmPtId);
                    int paperId = Convert.ToInt32(log.EmPaperId);

                    string tempDate = log.EmPtStart.GetDateTimeFormats('o')[0];
                    string date = tempDate.Substring(0, tempDate.IndexOf('T')); // 将日期转为2020-05-06
                    string start = log.EmPtStart.ToString("T"); // 开始时间
                    string end = log.EmPtEnd.ToString("T"); // 结束时间
                    string title = start.Substring(0, start.Length - 3) + "到" + end.Substring(0, end.Length - 3) + "场";

                    string examTitle = log.EmPaperName;


                    logList.Add(new { userId, userName, logId, isSubmit, exercisesId, answers, score, partId, paperId, date, title, examTitle });
                }

                logList.Add(totalCount);

                return JsonConvert.SerializeObject(logList);
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务端错误！" + ex.Message;
                return JsonConvert.SerializeObject(new { code, message });
            }
        }

        public string Remove(int id)
        {
            int code;
            string message;
            try
            {
                var log = (from l in db.ES_ExamLog
                           where l.IsDel == false && l.LogId == id
                           select l).FirstOrDefault();

                if (log == null)
                {
                    code = 1;
                    message = "服务端错误！未找到该记录";
                    return JsonConvert.SerializeObject(new { code, message });
                }

                log.IsDel = true;
                db.Entry(log).State = System.Data.Entity.EntityState.Modified;

                if (db.SaveChanges() > 0)
                {
                    code = 0;
                    message = "删除成功";
                    return JsonConvert.SerializeObject(new { code, message });
                }
                code = 1;
                message = "删除失败";
                return JsonConvert.SerializeObject(new { code, message });
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务端错误！" + ex.Message;
                return JsonConvert.SerializeObject(new { code, message });
            }
        }


        public void Export(int pageIndex = 1, string keyword = "", int ptId = 0, bool isPaging = true)
        {
            try
            {
                var logs = from l in db.ES_ExamLog
                           where l.IsDel == false
                           join u in db.ES_User on l.UserId equals u.UserId
                           join pt in db.ES_ExamPart on l.EmPtId equals pt.EmPtId
                           join pr in db.ES_ExamPaper on l.EmPaperId equals pr.EmPaperId
                           where pt.IsDel == false && pr.IsDel == false && u.IsDel == false
                           select new
                           {
                               u.UserId,
                               u.UserName,
                               l.LogId,
                               l.IsSubmit,
                               l.ExercisesId,
                               l.Answers,
                               l.ExamScore,
                               l.EmPtId,
                               l.EmPaperId,
                               pt.EmPtStart,
                               pt.EmPtEnd,
                               pr.EmPaperName
                           };

                if (ptId != null && ptId != 0)
                {
                    logs = logs.Where(l => l.EmPtId == ptId);
                }

                if (keyword != "")
                {
                    logs = logs.Where(l => l.UserName.Contains(keyword));
                }

                int totalCount = logs.Count();
                if (isPaging)
                {
                    logs = logs.OrderBy(l => l.LogId).Skip((pageIndex - 1) * 10).Take(10);
                }

                List<ExportExcelInfo> exportExcelList = new List<ExportExcelInfo>();

                foreach (var log in logs)
                {
                    int userId = log.UserId;
                    string userName = log.UserName;
                    int logId = log.LogId;
                    string isSubmit = log.IsSubmit == false ? "否" : "是";
                    string exercisesId = log.ExercisesId;
                    string answers = log.Answers;
                    double score = log.ExamScore;
                    int partId = Convert.ToInt32(log.EmPtId);
                    int paperId = Convert.ToInt32(log.EmPaperId);

                    string tempDate = log.EmPtStart.GetDateTimeFormats('o')[0];
                    string date = tempDate.Substring(0, tempDate.IndexOf('T')); // 将日期转为2020-05-06
                    string start = log.EmPtStart.ToString("T"); // 开始时间
                    string end = log.EmPtEnd.ToString("T"); // 结束时间
                    string title = start.Substring(0, start.Length - 3) + "到" + end.Substring(0, end.Length - 3) + "场";

                    string examTitle = log.EmPaperName;

                    ExportExcelInfo exportExcel = new ExportExcelInfo()
                    {
                        UserName = userName,
                        LogId = logId,
                        IsSubmit = log.IsSubmit,
                        Part = title,
                        Title = examTitle,
                        Score = score,
                        Date = date
                    };

                    exportExcelList.Add(exportExcel);
                }

                MemoryStream ms = ExcelHelper.ExportExcel(exportExcelList);


                //asp.net输出的Excel文件名
                //如果文件名是中文的话，需要进行编码转换，否则浏览器看到的下载文件是乱码。
                string fileName = HttpUtility.UrlEncode($"{TimeHelper.GetTimeStamp()}.xls");

                Response.ContentType = "application/vnd.ms-excel";
                //Response.ContentType = "application/download"; //也可以设置成download
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));

                Response.Buffer = true;
                Response.Clear();
                Response.BinaryWrite(ms.GetBuffer());
                Response.End();
            }
            catch (Exception ex)
            {
                Response.Write("ERROR" + ex.Message);
            }
        }
    }
}