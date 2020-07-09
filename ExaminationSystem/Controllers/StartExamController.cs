using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExaminationSystem.Models;
using ExaminationSystem.Utils;
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
                var user = from upt in db.ES_User_ExamPart
                           join u in db.ES_User on upt.UserId equals u.UserId
                           join pt in db.ES_ExamPart on upt.EmPtId equals pt.EmPtId
                           where u.UserId == id && u.IsDel == false && pt.IsDel == false && upt.IsDel == false
                           select new
                           {
                               u.UserName,
                               pt.EmPtStart,
                               pt.EmPtEnd,
                               pt.EmPtId
                           };

                if (user != null)
                {
                    foreach (var userPart in user)
                    {

                        DateTime current = DateTime.Now;

                        var parts = from p in db.ES_ExamPart
                                    where p.IsDel == false && p.EmPtStart <= current && p.EmPtEnd >= current && p.EmPtId == userPart.EmPtId
                                    select new
                                    {
                                        p.EmPtId,
                                        p.EmPtStart,
                                        p.EmPtEnd
                                    };

                        if (parts == null)
                        {
                            code = 1;
                            message = "当前没有正在进行的考试";
                            return JsonConvert.SerializeObject(new { code, message });
                        }
                        foreach (var part in parts)
                        {
                            if (userPart.EmPtStart == part.EmPtStart && userPart.EmPtEnd == part.EmPtEnd)
                            {
                                DateTime start = userPart.EmPtStart;
                                DateTime end = userPart.EmPtEnd;
                                string name = userPart.UserName;
                                int partId = part.EmPtId;

                                code = 0;

                                return JsonConvert.SerializeObject(new { code, name, start, end, partId });
                            }
                        }
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
        public bool IsStart(int id, int partId)
        {
            var log = (from l in db.ES_ExamLog
                       where l.UserId == id && l.IsDel == false && l.EmPtId == partId
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
                    where e.EsType == "单选题" && e.IsDel == false
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

        /// <summary>
        /// 根据标签获取
        /// </summary>
        /// <param name="num"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public List<object> GetSingle(int num, List<TagPercentInfo> tagPercents)
        {
            List<object> res = new List<object>();
            int fill = 0;// 当次标签题数不够时，让下一标签来补
            foreach (var tagPercent in tagPercents)
            {
                int count = Convert.ToInt32(Math.Floor(num * 0.01 * tagPercent.Percent));
                // 百分比 * 单选题数目

                var singles = (from e in db.ES_Exercise
                               where e.EsType == "单选题" && e.IsDel == false
                               join et in db.ES_Tag_Exercise on e.EsId equals et.EsId
                               join t in db.ES_Tag on et.TagId equals t.TagId
                               join s in db.ES_SelectQuestion on e.EsSubExerciseId equals s.SQId
                               where t.IsDel == false && et.IsDel == false && t.TagId == tagPercent.Id
                               orderby (Guid.NewGuid())
                               select new
                               {
                                   e.EsId,
                                   s.SQTitle,
                                   s.SQTrueAns,
                                   s.SQAns1,
                                   s.SQAns2,
                                   s.SQAns3,
                                   t.TagId
                               }).Take(count + fill).ToList();

                fill = count - singles.Count();

                foreach (var item in singles)
                {
                    res.Add(item);
                }
            }

            return res;
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
        /// 获取必选题
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public List<object> GetMustSingle(int tagId)
        {
            List<object> res = new List<object>();

            var singles = (from e in db.ES_Exercise
                           where e.EsType == "单选题" && e.IsDel == false
                           join et in db.ES_Tag_Exercise on e.EsId equals et.EsId
                           join t in db.ES_Tag on et.TagId equals t.TagId
                           join s in db.ES_SelectQuestion on e.EsSubExerciseId equals s.SQId
                           where t.IsDel == false && et.IsDel == false && t.TagId == tagId
                           orderby (Guid.NewGuid())
                           select new
                           {
                               e.EsId,
                               s.SQTitle,
                               s.SQTrueAns,
                               s.SQAns1,
                               s.SQAns2,
                               s.SQAns3,
                               t.TagId
                           }).ToList();

            foreach (var item in singles)
            {
                res.Add(item);
            }

            return res;
        }

        /// <summary>
        /// 获取多选题
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IQueryable<object> GetMultiple(int num)
        {
            return (from e in db.ES_Exercise
                    where e.EsType == "多选题" && e.IsDel == false
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

        /// <summary>
        /// 根据tag获取
        /// </summary>
        /// <param name="num"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public List<object> GetMultiple(int num, List<TagPercentInfo> tagPercents)
        {
            List<object> res = new List<object>();
            int fill = 0;// 当次标签题数不够时，让下一标签来补
            foreach (var tagPercent in tagPercents)
            {
                // 百分比 * 题数目
                int count = Convert.ToInt32(Math.Floor(num * 0.01 * tagPercent.Percent));

                var muls = (from e in db.ES_Exercise
                            where e.EsType == "多选题" && e.IsDel == false
                            join et in db.ES_Tag_Exercise on e.EsId equals et.EsId
                            join t in db.ES_Tag on et.TagId equals t.TagId
                            join m in db.ES_MultipleQuestion on e.EsSubExerciseId equals m.MQId
                            where t.IsDel == false && et.IsDel == false && t.TagId == tagPercent.Id
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
                            }).Take(count + fill).ToList();

                fill = count - muls.Count();

                foreach (var item in muls)
                {
                    res.Add(item);
                }
            }

            return res;
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
        /// 获取必选多选题
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public List<object> GetMustMultiple(int tagId)
        {
            List<object> res = new List<object>();

            var muls = (from e in db.ES_Exercise
                        where e.EsType == "多选题" && e.IsDel == false
                        join et in db.ES_Tag_Exercise on e.EsId equals et.EsId
                        join t in db.ES_Tag on et.TagId equals t.TagId
                        join m in db.ES_MultipleQuestion on e.EsSubExerciseId equals m.MQId
                        where t.IsDel == false && et.IsDel == false && t.TagId == tagId
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
                        }).ToList();


            foreach (var item in muls)
            {
                res.Add(item);
            }


            return res;
        }



        /// <summary>
        /// 获取判断题
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IQueryable<object> GetJudgment(int num)
        {
            return (from e in db.ES_Exercise
                    where e.EsType == "判断题" && e.IsDel == false
                    join j in db.ES_JudgeQuestion on e.EsSubExerciseId equals j.JQId
                    orderby (Guid.NewGuid())
                    select new
                    {
                        e.EsId,
                        j.JQTitle,
                        j.JQIsTrue
                    }).Take(num);
        }

        /// <summary>
        /// 根据tag获取
        /// </summary>
        /// <param name="num"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public List<object> GetJudgment(int num, List<TagPercentInfo> tagPercents)
        {
            List<object> res = new List<object>();
            int fill = 0;// 当次标签题数不够时，让下一标签来补
            foreach (var tagPercent in tagPercents)
            {
                // 百分比 * 题数目
                int count = Convert.ToInt32(Math.Floor(num * 0.01 * tagPercent.Percent));

                var judges = (from e in db.ES_Exercise
                              where e.EsType == "判断题" && e.IsDel == false
                              join et in db.ES_Tag_Exercise on e.EsId equals et.EsId
                              join t in db.ES_Tag on et.TagId equals t.TagId
                              join j in db.ES_JudgeQuestion on e.EsSubExerciseId equals j.JQId
                              where t.IsDel == false && et.IsDel == false && t.TagId == tagPercent.Id
                              orderby (Guid.NewGuid())
                              select new
                              {
                                  e.EsId,
                                  j.JQTitle,
                                  j.JQIsTrue,
                                  t.TagId
                              }).Take(count + fill).ToList();

                fill = count - judges.Count();

                foreach (var item in judges)
                {
                    res.Add(item);
                }
            }

            return res;
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
                        j.JQIsTrue
                    }).FirstOrDefault();
        }

        /// <summary>
        /// 获取必选判断
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public List<object> GetMustJudgment(int tagId)
        {
            List<object> res = new List<object>();

            var judges = (from e in db.ES_Exercise
                          where e.EsType == "判断题" && e.IsDel == false
                          join et in db.ES_Tag_Exercise on e.EsId equals et.EsId
                          join t in db.ES_Tag on et.TagId equals t.TagId
                          join j in db.ES_JudgeQuestion on e.EsSubExerciseId equals j.JQId
                          where t.IsDel == false && et.IsDel == false && t.TagId == tagId
                          orderby (Guid.NewGuid())
                          select new
                          {
                              e.EsId,
                              j.JQTitle,
                              j.JQIsTrue,
                              t.TagId
                          }).ToList();

            foreach (var item in judges)
            {
                res.Add(item);
            }

            return res;
        }


        /// <summary>
        /// 获取填空题
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IQueryable<object> GetFill(int num)
        {
            return (from e in db.ES_Exercise
                    where e.EsType == "填空题" && e.IsDel == false
                    join f in db.ES_FillQuestion on e.EsSubExerciseId equals f.FQId
                    orderby (Guid.NewGuid())
                    select new
                    {
                        e.EsId,
                        f.FQId,
                        f.FQTitle
                    }).Take(num);
        }
        /// <summary>
        /// 根据tag获取
        /// </summary>
        /// <param name="num"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public List<object> GetFill(int num, List<TagPercentInfo> tagPercents)
        {
            List<object> res = new List<object>();
            int fill = 0;// 当次标签题数不够时，让下一标签来补
            foreach (var tagPercent in tagPercents)
            {
                // 百分比 * 题数目
                int count = Convert.ToInt32(Math.Floor(num * 0.01 * tagPercent.Percent));

                var fills = (from e in db.ES_Exercise
                             where e.EsType == "填空题" && e.IsDel == false
                             join et in db.ES_Tag_Exercise on e.EsId equals et.EsId
                             join t in db.ES_Tag on et.TagId equals t.TagId
                             join f in db.ES_FillQuestion on e.EsSubExerciseId equals f.FQId
                             where t.IsDel == false && et.IsDel == false && t.TagId == tagPercent.Id
                             orderby (Guid.NewGuid())
                             select new
                             {
                                 e.EsId,
                                 f.FQId,
                                 f.FQTitle,
                                 t.TagId
                             }).Take(count + fill).ToList();

                fill = count - fills.Count();

                foreach (var item in fills)
                {
                    res.Add(item);
                }
            }

            return res;
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
        /// 获取必选填空
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public List<object> GetMustFill(int tagId)
        {
            List<object> res = new List<object>();

            var fills = (from e in db.ES_Exercise
                         where e.EsType == "填空题" && e.IsDel == false
                         join et in db.ES_Tag_Exercise on e.EsId equals et.EsId
                         join t in db.ES_Tag on et.TagId equals t.TagId
                         join f in db.ES_FillQuestion on e.EsSubExerciseId equals f.FQId
                         where t.IsDel == false && et.IsDel == false && t.TagId == tagId
                         orderby (Guid.NewGuid())
                         select new
                         {
                             e.EsId,
                             f.FQId,
                             f.FQTitle,
                             t.TagId
                         }).ToList();


            foreach (var item in fills)
            {
                res.Add(item);
            }

            return res;
        }


        /// <summary>
        /// 获取必选题数目
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public List<int> GetMustQuestionNum(int tagId)
        {
            int singleNum = (from e in db.ES_Exercise
                             where e.EsType == "单选题" && e.IsDel == false
                             join et in db.ES_Tag_Exercise on e.EsId equals et.EsId
                             join t in db.ES_Tag on et.TagId equals t.TagId
                             where t.IsDel == false && et.IsDel == false && t.TagId == tagId
                             select e).Count();
            int mulNum = (from e in db.ES_Exercise
                          where e.EsType == "多选题" && e.IsDel == false
                          join et in db.ES_Tag_Exercise on e.EsId equals et.EsId
                          join t in db.ES_Tag on et.TagId equals t.TagId
                          where t.IsDel == false && et.IsDel == false && t.TagId == tagId
                          select e).Count();
            int judgeNum = (from e in db.ES_Exercise
                            where e.EsType == "判断题" && e.IsDel == false
                            join et in db.ES_Tag_Exercise on e.EsId equals et.EsId
                            join t in db.ES_Tag on et.TagId equals t.TagId
                            where t.IsDel == false && et.IsDel == false && t.TagId == tagId
                            select e).Count();
            int fillNum = (from e in db.ES_Exercise
                           where e.EsType == "填空题" && e.IsDel == false
                           join et in db.ES_Tag_Exercise on e.EsId equals et.EsId
                           join t in db.ES_Tag on et.TagId equals t.TagId
                           where t.IsDel == false && et.IsDel == false && t.TagId == tagId
                           select e).Count();
            return new List<int>() { singleNum, mulNum, judgeNum, fillNum };
        }

        /// <summary>
        /// 创建详细考试试卷
        /// </summary>
        /// <param name="partId"></param>
        /// <returns></returns>
        public string CreateExamPaper(int partId, int userId)
        {
            // 如果已经有记录，则直接获取记录中的试卷
            if (IsStart(userId, partId))
            {
                return GetExamPaper(userId, partId);
            }

            int code;
            string message;

            var paper = GetPaper(partId);

            // 获取每个标签所占比例
            List<TagPercentInfo> tagPercents = JsonConvert.DeserializeObject<List<TagPercentInfo>>(paper.EmTagPercent);

            if (paper == null)
            {
                code = 1;
                message = "获取试卷失败! 该场次可能暂未设定试卷";
                return JsonConvert.SerializeObject(new { code, message });
            }

            try
            {
                var tags = GetTags(paper.EmPaperId);

                List<object> singles = new List<object>();
                List<object> multiples = new List<object>();
                List<object> judgments = new List<object>();
                List<object> fills = new List<object>();
                List<int> fillAnsNum = new List<int>();
                // 如果没选择标签
                if (tags.Count == 0)
                {
                    // 单选题
                    singles = GetSingle(paper.EmPaperSelectNum).ToList();

                    // 多选题
                    multiples = GetMultiple(paper.EmPaperMultipleNum).ToList();

                    // 获取判断题
                    judgments = GetJudgment(paper.EmPaperJudgeNum).ToList();

                    // 获取填空题
                    fills = GetFill(paper.EmPaperFillNum).ToList();

                    // 获取填空答案数量
                    fillAnsNum = new List<int>();

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


                foreach (var tag in tagPercents)
                {
                    if (tag.IsMust)
                    {
                        singles = GetMustSingle(tag.Id);
                        multiples = GetMustMultiple(tag.Id);
                        judgments = GetMustJudgment(tag.Id);
                        fills = GetMustFill(tag.Id);
                    }
                }

                // 单选题
                var normalSingle = GetSingle(paper.EmPaperSelectNum - singles.Count(), tagPercents);
                foreach (var item in normalSingle)
                {
                    singles.Add(item);
                }
                // 如果题目不够，自动补
                if (singles.Count() < paper.EmPaperSelectNum)
                {
                    var notTag = GetSingle(paper.EmPaperSelectNum - singles.Count()).ToList();
                    foreach (var item in notTag)
                    {
                        singles.Add(item);
                    }
                }

                // 多选题
                var normalMultiples = GetMultiple(paper.EmPaperMultipleNum - multiples.Count(), tagPercents);
                foreach (var item in normalMultiples)
                {
                    multiples.Add(item);
                }
                if (multiples.Count() < paper.EmPaperMultipleNum)
                {
                    var notTag = GetMultiple(paper.EmPaperMultipleNum - multiples.Count()).ToList();
                    foreach (var item in notTag)
                    {
                        multiples.Add(item);
                    }
                }

                // 获取判断题
                var normalJudgments = GetJudgment(paper.EmPaperJudgeNum - judgments.Count(), tagPercents);
                foreach (var item in normalJudgments)
                {
                    judgments.Add(item);
                }
                if (judgments.Count() < paper.EmPaperJudgeNum)
                {
                    var notTag = GetJudgment(paper.EmPaperJudgeNum - judgments.Count()).ToList();
                    foreach (var item in notTag)
                    {
                        judgments.Add(item);
                    }
                }

                // 获取填空题
                var normalFills = GetFill(paper.EmPaperFillNum - fills.Count(), tagPercents);
                foreach (var item in normalFills)
                {
                    fills.Add(item);
                }
                if (fills.Count() < paper.EmPaperFillNum)
                {
                    var notTag = GetFill(paper.EmPaperFillNum - fills.Count()).ToList();
                    foreach (var item in notTag)
                    {
                        fills.Add(item);
                    }
                }

                // 获取填空答案数量
                fillAnsNum = new List<int>();

                foreach (var fill in fills)
                {
                    int fillId = Convert.ToInt32(fill.GetType().GetProperty("FQId").GetValue(fill));

                    fillAnsNum.Add((from fa in db.ES_FillAnswer
                                    where fa.FQId == fillId
                                    select fa).Count());
                }

                ES_ExamLog log = CreateLog(new List<List<object>>() { singles, multiples, judgments, fills }, userId, paper.EmPaperId, partId);

                return JsonConvert.SerializeObject(new { logId = log.LogId, title = paper.EmPaperName, singles, multiples, judgments, fills, fillAnsNum, startTime = log.StartTime });
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务端错误！" + ex.Message;
                return JsonConvert.SerializeObject(new { code, message });
            }
        }

        /// <summary>
        /// 创建考试记录
        /// </summary>
        /// <param name="allQuestions"></param>
        /// <param name="userId"></param>
        /// <param name="paperId"></param>
        /// <param name="partId"></param>
        /// <returns></returns>
        public ES_ExamLog CreateLog(List<List<object>> allQuestions, int userId, int paperId, int partId)
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
                IsStart = true,
                StartTime = TimeHelper.ToJsTime(DateTime.Now)
            };
            db.ES_ExamLog.Add(log);

            if (db.SaveChanges() > 0)
            {
                return log;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取记录中未提交的试卷
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetExamPaper(int id, int partId)
        {
            int code;
            string message;
            // 获取记录中的题目ID
            var log = (from l in db.ES_ExamLog
                       where l.UserId == id && l.EmPtId == partId && l.IsDel == false && l.IsSubmit == false
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
                                where l.UserId == id && l.EmPtId == partId && l.IsDel == false && p.IsDel == false
                                select p.EmPaperName).FirstOrDefault();

                return JsonConvert.SerializeObject(new { logId = log.LogId, title, singles, multiples, judgments, fills, fillAnsNum, answers = log.Answers, startTime = log.StartTime });
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务器错误！" + ex.Message;
                return JsonConvert.SerializeObject(new { code, message });
            }
        }

        /// <summary>
        /// 获取已经提交的试卷，及答案
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetExamPaperIsSubmit(int id)
        {
            int code;
            string message;
            // 获取记录中的题目ID
            var log = (from l in db.ES_ExamLog
                       where l.LogId == id && l.IsDel == false && l.IsSubmit == true
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
            List<object> fillAnswers = new List<object>();

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
                            int MQId = Convert.ToInt32(multiple.GetType().GetProperty("MQId").GetValue(multiple));

                            var mulAns = from ma in db.ES_MultipleAnswer
                                         where ma.MQId == MQId
                                         select new { ma.MAContent };

                            multiples.Add(new { multiple, mulAns });
                            break;
                        case "判断题":
                            var judgment = GetJudgmentById(question.EsId);
                            judgments.Add(judgment);
                            break;
                        case "填空题":
                            var fill = GetFillById(question.EsId);
                            int fillId = Convert.ToInt32(fill.GetType().GetProperty("FQId").GetValue(fill));

                            var fillAns = from fa in db.ES_FillAnswer
                                          where fa.FQId == fillId
                                          select new { fa.FQId, fa.FAContent };
                            fillAnswers.Add(fillAns);
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

                return JsonConvert.SerializeObject(new { title, singles, multiples, judgments, fills, fillAnsNum, fillAnswers, answers = log.Answers });
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
        [HttpPost]
        public string SaveLog(string ansStr, int id)
        {
            int code;
            string message;
            try
            {
                var log = (from l in db.ES_ExamLog
                           where l.LogId == id && l.IsStart == true && l.IsSubmit == false
                           select l).FirstOrDefault();

                if (log == null)
                {
                    code = 1;
                    message = "无此考试记录或您已提交试卷";
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

        /// <summary>
        /// 计算分数、保存记录
        /// </summary>
        /// <param name="ansStr"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public string FinishExam(string ansStr, int id)
        {
            List<AnswerInfo> ans = JsonConvert.DeserializeObject<List<AnswerInfo>>(ansStr);

            int code;
            string message;
            try
            {
                var log = (from l in db.ES_ExamLog
                           where l.LogId == id && l.IsStart == true && l.IsSubmit == false && l.IsDel == false
                           select l).FirstOrDefault();

                if (log == null)
                {
                    code = 1;
                    message = "获取题目失败";
                    return JsonConvert.SerializeObject(new { code, message });
                }

                var scores = (from p in db.ES_ExamPaper
                              where p.EmPaperId == log.EmPaperId && p.IsDel == false && log.IsDel == false
                              select new
                              {
                                  p.EmPaperSelectScore,
                                  p.EmPaperMultipleScore,
                                  p.EmPaperJudgeScore,
                                  p.EmPaperFillScore
                              }).FirstOrDefault();

                if (scores == null)
                {
                    code = 1;
                    message = "获取分数失败";
                    return JsonConvert.SerializeObject(new { code, message });
                }

                List<string> exIdList = new List<string>(log.ExercisesId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));

                double singleScore = 0;
                double multipleScore = 0;
                double judgmentScore = 0;
                double fillScore = 0;

                // 获取记录中的所有题目
                exIdList.ForEach(esIdStr =>
                {
                    int esId = Convert.ToInt32(esIdStr);

                    // 获取该题考生的答案
                    AnswerInfo nowAns = null;
                    foreach (var ansItem in ans)
                    {
                        if (ansItem.EsId == esId)
                        {
                            nowAns = ansItem;
                            break;
                        }
                    }

                    // 获取该题
                    var question = (from q in db.ES_Exercise
                                    where q.EsId == esId
                                    select q).FirstOrDefault();
                    // 判断对错，计算分数
                    switch (question.EsType)
                    {
                        case "单选题":
                            if (nowAns.Ans[0] == "SQTrueAns")
                            {
                                singleScore += scores.EmPaperSelectScore;
                            }
                            break;
                        case "多选题":
                            var multiple = GetMultipleById(question.EsId);
                            int MQId = Convert.ToInt32(multiple.GetType().GetProperty("MQId").GetValue(multiple));

                            var mulAns = (from ma in db.ES_MultipleAnswer
                                          where ma.MQId == MQId
                                          select ma).ToList();

                            if (mulAns.Count() >= nowAns.Ans.Count())
                            {
                                int mulTrueNum = 0;
                                foreach (var item in mulAns)
                                {
                                    for (int i = 0; i < nowAns.Ans.Count(); i++)
                                    {
                                        if (item.MAContent == nowAns.Ans[i])
                                        {
                                            mulTrueNum++;
                                            break;
                                        }
                                    }
                                }

                                if (mulTrueNum == mulAns.Count())
                                {
                                    multipleScore += scores.EmPaperMultipleScore;
                                }
                                else if (mulTrueNum != 0)// 如果没有全对
                                {
                                    multipleScore += scores.EmPaperMultipleScore / 2;
                                }
                            }
                            break;
                        case "判断题":
                            if (nowAns.Ans[0] == "true")
                            {
                                judgmentScore += scores.EmPaperJudgeScore;
                            }
                            break;
                        case "填空题":
                            var fill = GetFillById(question.EsId);
                            int fillId = Convert.ToInt32(fill.GetType().GetProperty("FQId").GetValue(fill));

                            var fillAns = (from fa in db.ES_FillAnswer
                                           where fa.FQId == fillId
                                           select fa).ToList();
                            // 判断题顺序不能错，且答案数量不会出错

                            int trueNum = 0;

                            for (int i = 0; i < nowAns.Ans.Count(); i++)
                            {
                                if (fillAns[i].FAContent == nowAns.Ans[i])
                                {
                                    trueNum++;
                                }
                            }


                            if (trueNum == fillAns.Count())
                            {
                                fillScore += scores.EmPaperFillScore;
                            }
                            else if (trueNum != 0)// 如果没有全对
                            {
                                fillScore += scores.EmPaperFillScore / fillAns.Count();
                            }

                            break;
                    }
                });
                double finalScore = singleScore + multipleScore + judgmentScore + fillScore;

                log.Answers = ansStr;
                log.IsSubmit = true;
                log.ExamScore = finalScore;

                db.Entry(log).State = System.Data.Entity.EntityState.Modified;

                if (db.SaveChanges() > 0)
                {
                    return JsonConvert.SerializeObject(new { singleScore, multipleScore, judgmentScore, fillScore, finalScore });
                }

                code = 1;
                message = "考生记录保存失败";
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