using ExaminationSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExaminationSystem.Controllers
{
    public class UserScoreController : Controller
    {
        readonly ExaminationSystemDbEntities db = new ExaminationSystemDbEntities();

        /// <summary>
        /// 获取场次
        /// </summary>
        /// <returns></returns>
        public string GetExamPart()
        {
            // 获取所有考试场次
            var exams = from ep in db.ES_ExamPart where ep.IsDel == false orderby ep.EmPtId descending select ep;

            // 格式化
            List<object> examList = new List<object>();

            foreach (var exam in exams)
            {
                int id = exam.EmPtId;
                string tempDate = exam.EmPtStart.GetDateTimeFormats('o')[0];
                string date = tempDate.Substring(0, tempDate.IndexOf('T')); // 将日期转为2020-05-06
                string start = exam.EmPtStart.TimeOfDay.ToString(); // 开始时间
                string end = exam.EmPtEnd.TimeOfDay.ToString(); // 结束时间
                string title = start.Substring(0, start.Length - 3) + "到" + end.Substring(0, end.Length - 3) + "场";

                examList.Add(new { id, date, title });
            }

            // 序列化为JSON 传递到View
            return JsonConvert.SerializeObject(examList);
        }

        /// <summary>
        /// 根据场次ID获取学生成绩列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetUserScoreByPartId(string id, int pageIndex)
        {
            var userScores = from u in db.ES_User
                             join up in db.ES_User_ExamPart on u.UserId equals up.UserId
                             join epart in db.ES_ExamPart on up.EmPtId equals epart.EmPtId
                             join upaper in db.ES_User_ExamPaper on u.UserId equals upaper.UserId
                             join epaper in db.ES_ExamPaper on upaper.EmPaperId equals epaper.EmPaperId
                             where u.IsDel == false && u.RoleId == 1
                             select new
                             {
                                 u.UserId,
                                 u.UserName,
                                 epart.EmPtId,
                                 epart.EmPtStart,
                                 epart.EmPtEnd,
                                 epaper.EmPaperTrueScore
                             };

            if (id != "" && id != "0")
            {
                int emptId = Convert.ToInt32(id);
                userScores = userScores.Where(u => u.EmPtId == emptId);
            }

            int totalCount = userScores.Count();
            int totalPages = Convert.ToInt32(Math.Ceiling((double)totalCount / 10));

            userScores = userScores.OrderBy(u => u.UserId).Skip((pageIndex - 1) * 10).Take(10);

            List<object> userScoreList = new List<object>();

            foreach (var userScore in userScores)
            {
                int userId = userScore.UserId;
                string name = userScore.UserName;
                string tempDate = userScore.EmPtStart.GetDateTimeFormats('o')[0];
                string date = tempDate.Substring(0, tempDate.IndexOf('T')); // 将日期转为2020-05-06
                string start = userScore.EmPtStart.TimeOfDay.ToString(); // 开始时间
                string end = userScore.EmPtEnd.TimeOfDay.ToString(); // 结束时间
                string title = start.Substring(0, start.Length - 3) + "到" + end.Substring(0, end.Length - 3) + "场";
                int score = Convert.ToInt32(userScore.EmPaperTrueScore ?? 0);

                userScoreList.Add(new { userId, name, date, title, score });
            }

            return JsonConvert.SerializeObject(userScoreList);
        }
    }
}