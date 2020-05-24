using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExaminationSystem.Models;
using Newtonsoft.Json;

namespace ExaminationSystem.Controllers
{
    public class StartExamController : Controller
    {
        readonly ExaminationSystemDbEntities db = new ExaminationSystemDbEntities();
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 验证用户是否可以参加当前考试
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Auth(int id)
        {
            int code;
            string message;

            try
            {
                var user = (from upt in db.ES_User_ExamPart
                            join u in db.ES_User on upt.UserId equals u.UserId
                            join pt in db.ES_ExamPart on upt.EmPtId equals pt.EmPtId
                            where u.UserId == id && u.IsDel == false && pt.IsDel == false && upt.IsDel == false
                            select new
                            {
                                u.UserName,
                                pt.EmPtStart,
                                pt.EmPtEnd
                            }).FirstOrDefault();

                if (user != null)
                {
                    DateTime current = DateTime.Now;

                    var part = (from p in db.ES_ExamPart
                                where p.IsDel == false && p.EmPtStart <= current && p.EmPtEnd >= current
                                select new
                                {
                                    p.EmPtId,
                                    p.EmPtStart,
                                    p.EmPtEnd
                                }).FirstOrDefault();

                    if (part == null)
                    {
                        code = 1;
                        message = "当前没有正在进行的考试";
                        return JsonConvert.SerializeObject(new { code, message });
                    }

                    if (user.EmPtStart == part.EmPtStart && user.EmPtEnd == part.EmPtEnd)
                    {
                        DateTime start = user.EmPtStart;
                        DateTime end = user.EmPtEnd;
                        string name = user.UserName;
                        int partId = part.EmPtId;

                        code = 0;

                        return JsonConvert.SerializeObject(new { code, name, start, end, partId });
                    }

                    code = 1;
                    message = "验证失败，您暂时无法参加该场考试";
                    return JsonConvert.SerializeObject(new { code, message });
                }

                code = 1;
                message = "验证失败！您可能暂无考试任务";
                return JsonConvert.SerializeObject(new { code, message });
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务器错误!" + ex.Message;
                return JsonConvert.SerializeObject(new { code, message });
            }
        }

        /// <summary>
        /// 验证用户是否已经开始考试
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsStart(int id)
        {
            var log = (from l in db.ES_ExamLog
                       where l.UserId == id && l.IsDel == false
                       select l).FirstOrDefault();

            return log == null ? false : log.IsStart;
        }

        /// <summary>
        /// 获取试卷设定
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        public ES_ExamPaper GetPaper(int partId)
        {
            var paper = (from p in db.ES_ExamPaper
                         where p.EmPtId == partId && p.IsDel == false
                         select p).FirstOrDefault();

            return paper;
        }

        /// <summary>
        /// 获取试卷标签
        /// </summary>
        /// <param name="paperId"></param>
        /// <returns></returns>
        public List<ES_Tag> GetTags(int paperId)
        {
            var tags = from t in db.ES_Tag
                       join p in db.ES_Paper_Tag on t.TagId equals p.TagId
                       where p.PaperId == paperId
                       select t;

            List<ES_Tag> tagList = new List<ES_Tag>();

            foreach (var tag in tags)
            {
                tagList.Add(tag);
            }


            return tagList;
        }

        /// <summary>
        /// 获取单选题
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IQueryable<object> GetSingle(int num)
        {
            return (from e in db.ES_Exercise
                    where e.EsType == "单选题"
                    join s in db.ES_SelectQuestion on e.EsSubExerciseId equals s.SQId
                    orderby (Guid.NewGuid())
                    select new
                    {
                        e.EsId,
                        s.SQTitle,
                        s.SQTrueAns,
                        s.SQAns1,
                        s.SQAns2,
                        s.SQAns3
                    }).Take(num);
        }
        public object GetSingleById(int id)
        {
            return (from e in db.ES_Exercise
                    where e.EsType == "单选题" && e.EsId == id
                    join s in db.ES_SelectQuestion on e.EsSubExerciseId equals s.SQId
                    select new
                    {
                        e.EsId,
                        s.SQTitle,
                        s.SQTrueAns,
                        s.SQAns1,
                        s.SQAns2,
                        s.SQAns3
                    }).FirstOrDefault();
        }

        /// <summary>
        /// 获取多选题
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IQueryable<object> GetMultiple(int num)
        {
            return (from e in db.ES_Exercise
                    where e.EsType == "多选题"
                    join m in db.ES_MultipleQuestion on e.EsSubExerciseId equals m.MQId
                    orderby (Guid.NewGuid())
                    select new
                    {
                        e.EsId,
                        m.MQId,
                        m.MQTitle,
                        m.MQAns1,
                        m.MQAns2,
                        m.MQAns3,
                        m.MQAns4,
                        m.MQAns5,
                        m.MQAns6,
                        m.MQAns7
                    }).Take(num);
        }
        public object GetMultipleById(int id)
        {
            return (from e in db.ES_Exercise
                    where e.EsType == "多选题" && e.EsId == id
                    join m in db.ES_MultipleQuestion on e.EsSubExerciseId equals m.MQId
                    select new
                    {
                        e.EsId,
                        m.MQId,
                        m.MQTitle,
                        m.MQAns1,
                        m.MQAns2,
                        m.MQAns3,
                        m.MQAns4,
                        m.MQAns5,
                        m.MQAns6,
                        m.MQAns7
                    }).FirstOrDefault();
        }

        /// <summary>
        /// 获取判断题
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IQueryable<object> GetJudgment(int num)
        {
            return (from e in db.ES_Exercise
                    where e.EsType == "判断题"
                    join j in db.ES_JudgeQuestion on e.EsSubExerciseId equals j.JQId
                    orderby (Guid.NewGuid())
                    select new
                    {
                        e.EsId,
                        j.JQTitle,
                        j.JQFalseAns,
                        j.JQTrueAns
                    }).Take(num);
        }
        public object GetJudgmentById(int id)
        {
            return (from e in db.ES_Exercise
                    where e.EsType == "判断题" && e.EsId == id
                    join j in db.ES_JudgeQuestion on e.EsSubExerciseId equals j.JQId
                    orderby (Guid.NewGuid())
                    select new
                    {
                        e.EsId,
                        j.JQTitle,
                        j.JQFalseAns,
                        j.JQTrueAns
                    }).FirstOrDefault();
        }

        /// <summary>
        /// 获取填空题
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IQueryable<object> GetFill(int num)
        {
            return (from e in db.ES_Exercise
                    where e.EsType == "填空题"
                    join f in db.ES_FillQuestion on e.EsSubExerciseId equals f.FQId
                    orderby (Guid.NewGuid())
                    select new
                    {
                        e.EsId,
                        f.FQId,
                        f.FQTitle
                    }).Take(num);
        }
        public object GetFillById(int id)
        {
            return (from e in db.ES_Exercise
                    where e.EsType == "填空题" && e.EsId == id
                    join f in db.ES_FillQuestion on e.EsSubExerciseId equals f.FQId
                    orderby (Guid.NewGuid())
                    select new
                    {
                        e.EsId,
                        f.FQId,
                        f.FQTitle
                    }).FirstOrDefault();
        }

        /// <summary>
        /// 创建详细考试试卷
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        public string CreateExamPaper(int partId, int userId)
        {
            // 如果已经有记录，则直接获取记录中的试卷
            if (IsStart(userId))
            {
                return GetExamPaper(userId);
            }

            int code;
            string message;

            var paper = GetPaper(partId);

            if (paper == null)
            {
                code = 1;
                message = "获取试卷失败! 该场次可能暂未设定试卷";
                return JsonConvert.SerializeObject(new { code, message });
            }

            var tags = GetTags(paper.EmPaperId);

            // 如果没选择标签
            if (tags.Count == 0)
            {
                // 单选题
                var singles = GetSingle(paper.EmPaperSelectNum).ToList();

                // 多选题
                var multiples = GetMultiple(paper.EmPaperMultipleNum).ToList();

                // 获取判断题
                var judgments = GetJudgment(paper.EmPaperJudgeNum).ToList();

                // 获取填空题
                var fills = GetFill(paper.EmPaperFillNum).ToList();

                // 获取填空答案数量
                List<int> fillAnsNum = new List<int>();

                foreach (var fill in fills)
                {
                    int fillId = Convert.ToInt32(fill.GetType().GetProperty("FQId").GetValue(fill));

                    fillAnsNum.Add((from fa in db.ES_FillAnswer
                                    where fa.FQId == fillId
                                    select fa).Count());
                }

                CreateLog(new List<List<object>>() { singles, multiples, judgments, fills }, userId, paper.EmPaperId, partId);

                return JsonConvert.SerializeObject(new { title = paper.EmPaperName, singles, multiples, judgments, fills, fillAnsNum });
            }

            // TODO 如果有标签

            code = 1;
            message = "错误";
            return JsonConvert.SerializeObject(new { code, message });
        }

        /// <summary>
        /// 创建考试记录
        /// </summary>
        /// <param name="allQuestions"></param>
        /// <param name="userId"></param>
        /// <param name="paperId"></param>
        /// <param name="partId"></param>
        /// <returns></returns>
        public bool CreateLog(List<List<object>> allQuestions, int userId, int paperId, int partId)
        {
            List<int> ids = new List<int>();
            foreach (var subQuestions in allQuestions)
            {
                foreach (var question in subQuestions)
                {
                    ids.Add(Convert.ToInt32(question.GetType().GetProperty("EsId").GetValue(question)));
                }
            }

            string idStr = string.Join(",", ids.ToArray());

            ES_ExamLog log = new ES_ExamLog()
            {
                UserId = userId,
                EmPaperId = paperId,
                EmPtId = partId,
                ExercisesId = idStr,
                IsStart = true
            };
            db.ES_ExamLog.Add(log);

            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// 获取记录未提交的试卷
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetExamPaper(int id)
        {
            int code;
            string message;
            // 获取记录中的题目ID
            var log = (from l in db.ES_ExamLog
                       where l.UserId == id && l.IsDel == false && l.IsSubmit == false
                       select l).FirstOrDefault();

            if (log == null)
            {
                code = 1;
                message = "未找到记录，可能您已提交试卷或您已错过考试时间";
                return JsonConvert.SerializeObject(new { code, message });
            }

            List<string> exIdList = new List<string>(log.ExercisesId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));


            List<object> singles = new List<object>();
            List<object> multiples = new List<object>();
            List<object> judgments = new List<object>();
            List<object> fills = new List<object>();

            try
            {
                // 获取记录中的所有题目
                exIdList.ForEach(esIdStr =>
                {
                    int esId = Convert.ToInt32(esIdStr);

                    var question = (from q in db.ES_Exercise
                                    where q.EsId == esId
                                    select q).FirstOrDefault();

                    switch (question.EsType)
                    {
                        case "单选题":
                            var single = GetSingleById(question.EsId);
                            singles.Add(single);
                            break;
                        case "多选题":
                            var multiple = GetMultipleById(question.EsId);
                            multiples.Add(multiple);
                            break;
                        case "判断题":
                            var judgment = GetJudgmentById(question.EsId);
                            judgments.Add(judgment);
                            break;
                        case "填空题":
                            var fill = GetFillById(question.EsId);
                            int fillId = Convert.ToInt32(fill.GetType().GetProperty("FQId").GetValue(fill));
                            fills.Add(fill);
                            break;
                    }
                });

                // 获取填空答案数量
                List<int> fillAnsNum = new List<int>();

                foreach (var fill in fills)
                {
                    int fillId = Convert.ToInt32(fill.GetType().GetProperty("FQId").GetValue(fill));

                    fillAnsNum.Add((from fa in db.ES_FillAnswer
                                    where fa.FQId == fillId
                                    select fa).Count());
                }

                string title = (from l in db.ES_ExamLog
                                join p in db.ES_ExamPaper on l.EmPaperId equals p.EmPaperId
                                where l.UserId == id && l.IsDel == false && p.IsDel == false
                                select p.EmPaperName).FirstOrDefault();

                return JsonConvert.SerializeObject(new { title, singles, multiples, judgments, fills, fillAnsNum, answers = log.Answers });
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务器错误！" + ex.Message;
                return JsonConvert.SerializeObject(new { code, message });
            }
        }

        /// <summary>
        /// 保存试卷
        /// </summary>
        /// <param name="ansStr"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public string SaveLog(string ansStr, int id)
        {
            int code;
            string message;
            try
            {
                var log = (from l in db.ES_ExamLog
                           where l.UserId == id && l.IsStart == true && l.IsSubmit == false
                           select l).FirstOrDefault();

                if (log == null)
                {
                    code = 1;
                    message = "无此考试记录";
                    return JsonConvert.SerializeObject(new { code, message });
                }

                log.Answers = ansStr;
                db.Entry(log).State = System.Data.Entity.EntityState.Modified;

                if (db.SaveChanges() > 0)
                {
                    code = 0;
                    message = "保存成功";
                    return JsonConvert.SerializeObject(new { code, message });
                }
                code = 1;
                message = "保存失败";
                return JsonConvert.SerializeObject(new { code, message });
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务器错误！" + ex.Message;
                return JsonConvert.SerializeObject(new { code, message });
            }
        }
    }
}