using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExaminationSystem.Models;
using Newtonsoft.Json;

namespace ExaminationSystem.Controllers
{
    public class HomeController : Controller
    {
        readonly ExaminationSystemDbEntities db = new ExaminationSystemDbEntities();

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // 获取所有考试场次
            var exams = from ep in db.ES_ExamPart where ep.IsDel == false select ep;

            // 格式化
            List<object> examList = new List<object>();

            foreach (var exam in exams)
            {
                string tempDate = exam.EmPtStart.GetDateTimeFormats('o')[0];
                string date = tempDate.Substring(0, tempDate.IndexOf('T')); // 将日期转为2020-05-06
                string start = exam.EmPtStart.ToString("T"); // 开始时间
                string end = exam.EmPtEnd.ToString("T"); // 结束时间
                string title = start.Substring(0, start.Length - 3) + "到" + end.Substring(0, end.Length - 3) + "场";

                examList.Add(new { date, title });
            }

            // 序列化为JSON 传递到View
            ViewBag.Exams = JsonConvert.SerializeObject(examList);

            return View();
        }

        /// <summary>
        /// 成绩查询
        /// </summary>
        /// <returns></returns>
        public ActionResult Message()
        {
            if (Session["account"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        public string GetScore(int id)
        {
            int code;
            string message;
            try
            {
                var log = (from l in db.ES_ExamLog
                           where l.IsDel == false && l.UserId == id
                           join u in db.ES_User on l.UserId equals u.UserId
                           join pt in db.ES_ExamPart on l.EmPtId equals pt.EmPtId
                           join pr in db.ES_ExamPaper on l.EmPaperId equals pr.EmPaperId
                           where pt.IsDel == false && pr.IsDel == false && u.IsDel == false
                           select new
                           {
                               u.UserId,
                               u.UserName,
                               u.UserAccount,
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
                           }).FirstOrDefault();
                if (log == null)
                {
                    code = 1;
                    message = "未找到考试记录";

                    return JsonConvert.SerializeObject(new { code, message });
                }
                int userId = log.UserId;
                string account = log.UserAccount;
                string name = log.UserName;
                string tempDate = log.EmPtStart.GetDateTimeFormats('o')[0];
                string date = tempDate.Substring(0, tempDate.IndexOf('T')); // 将日期转为2020-05-06
                string start = log.EmPtStart.ToString("T"); // 开始时间
                string end = log.EmPtEnd.ToString("T"); // 结束时间
                string title = start.Substring(0, start.Length - 3) + "到" + end.Substring(0, end.Length - 3) + "场";
                string paperTitle = log.EmPaperName;
                double score = log.ExamScore;

                return JsonConvert.SerializeObject(new { userId, account, name, date, title, paperTitle, score });
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务端错误！" + ex.Message;

                return JsonConvert.SerializeObject(new { code, message });
            }
        }
    }
}