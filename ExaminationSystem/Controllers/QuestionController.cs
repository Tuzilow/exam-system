using ExaminationSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExaminationSystem.Controllers
{
    public class QuestionController : Controller
    {
        readonly ExaminationSystemDbEntities db = new ExaminationSystemDbEntities();

        /// <summary>
        /// 将题目添加到总题目表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int AddExercise(string type, int id)
        {
            ES_Exercise exercise = new ES_Exercise()
            {
                EsType = type,
                EsSubExerciseId = id
            };
            db.ES_Exercise.Add(exercise);

            if (db.SaveChanges() > 0)
            {
                var last = db.ES_Exercise.OrderByDescending(e => e.EsId).FirstOrDefault();
                if (last != null)
                {
                    return last.EsId;
                }
            }

            return -1;
        }

        /// <summary>
        /// 添加单选题
        /// </summary>
        /// <param name="title"></param>
        /// <param name="trueSel"></param>
        /// <param name="sel1"></param>
        /// <param name="sel2"></param>
        /// <param name="sel3"></param>
        /// <param name="score"></param>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public string AddSingle(string title, string trueSel, string sel1, string sel2, string sel3, int score, int[] tagsId)
        {
            int code;
            string message;
            ES_SelectQuestion single = new ES_SelectQuestion()
            {
                SQTitle = title,
                SQTrueAns = trueSel,
                SQAns1 = sel1,
                SQAns2 = sel2,
                SQAns3 = sel3,
                SQScore = score
            };

            try
            {
                db.ES_SelectQuestion.Add(single);
                if (db.SaveChanges() > 0)
                {
                    var thisSingle = db.ES_SelectQuestion.OrderByDescending(sq => sq.SQId).FirstOrDefault();

                    if (thisSingle != null)
                    {
                        int esId = AddExercise("单选题", thisSingle.SQId);
                        if (tagsId == null || tagsId.Length == 0)
                        {
                            code = 0;
                            message = "添加成功";

                            return JsonConvert.SerializeObject(new { code, message });
                        }
                        if (esId != -1)
                        {
                            // 添加标签
                            foreach (int tagId in tagsId)
                            {
                                ES_Tag_Exercise te = new ES_Tag_Exercise()
                                {
                                    TagId = tagId,
                                    EsId = esId
                                };

                                db.ES_Tag_Exercise.Add(te);
                            }
                            if (db.SaveChanges() >= tagsId.Length)
                            {
                                code = 0;
                                message = "添加成功";

                                return JsonConvert.SerializeObject(new { code, message });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务器错误！" + ex.Message;

                return JsonConvert.SerializeObject(new { code, message });
            }

            code = 1;
            message = "服务器错误！题目添加失败";

            return JsonConvert.SerializeObject(new { code, message });
        }

        /// <summary>
        /// 添加多选题
        /// </summary>
        /// <param name="title"></param>
        /// <param name="answers"></param>
        /// <param name="trueSels"></param>
        /// <param name="score"></param>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public string AddMultiple(string title, string[] answers, string[] trueSels, int score, int[] tagsId)
        {
            int code;
            string message;

            ES_MultipleQuestion multiple = new ES_MultipleQuestion()
            {
                MQTitle = title,
                MQAns1 = answers[0],
                MQAns2 = answers[1],
                MQAns3 = answers[2],
                MQAns4 = answers[3],
                MQAns5 = answers[4] == "" ? null : answers[4],
                MQAns6 = answers[5] == "" ? null : answers[5],
                MQAns7 = answers[6] == "" ? null : answers[6],
                MQScore = score
            };
            try
            {
                db.ES_MultipleQuestion.Add(multiple);

                if (db.SaveChanges() > 0)
                {

                    // 添加答案，将题目关联到总表
                    var question = db.ES_MultipleQuestion.OrderByDescending(mq => mq.MQId).FirstOrDefault();
                    foreach (string sel in trueSels)
                    {
                        ES_MultipleAnswer answer = new ES_MultipleAnswer()
                        {
                            MQId = question.MQId,
                            MAContent = sel
                        };
                        db.ES_MultipleAnswer.Add(answer);
                    }
                    int addRes = db.SaveChanges();
                    int esId = AddExercise("多选题", question.MQId); // 获取对应在总表中的id
                    if (addRes >= trueSels.Length && esId != -1)
                    {
                        if (tagsId == null || tagsId.Length == 0)
                        {
                            code = 0;
                            message = "添加成功";

                            return JsonConvert.SerializeObject(new { code, message });
                        }
                        // 添加标签
                        foreach (int tagId in tagsId)
                        {
                            ES_Tag_Exercise te = new ES_Tag_Exercise()
                            {
                                TagId = tagId,
                                EsId = esId
                            };

                            db.ES_Tag_Exercise.Add(te);
                        }
                        if (db.SaveChanges() >= tagsId.Length)
                        {
                            code = 0;
                            message = "添加成功";

                            return JsonConvert.SerializeObject(new { code, message });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务器错误！" + ex.Message;

                return JsonConvert.SerializeObject(new { code, message });
            }

            code = 1;
            message = "服务器错误！题目添加失败";

            return JsonConvert.SerializeObject(new { code, message });
        }

        /// <summary>
        /// 添加判断题
        /// </summary>
        /// <param name="title"></param>
        /// <param name="trueSel"></param>
        /// <param name="falseSel"></param>
        /// <param name="score"></param>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public string AddJudgment(string title, string trueSel, string falseSel, int score, int[] tagsId)
        {
            int code;
            string message;

            ES_JudgeQuestion judge = new ES_JudgeQuestion()
            {
                JQTitle = title,
                JQTrueAns = trueSel,
                JQFalseAns = falseSel,
                JQScore = score
            };
            try
            {
                db.ES_JudgeQuestion.Add(judge);

                if (db.SaveChanges() > 0)
                {

                    // 添加答案，将题目关联到总表
                    var question = db.ES_JudgeQuestion.OrderByDescending(jq => jq.JQId).FirstOrDefault();

                    int esId = AddExercise("多选题", question.JQId); // 获取对应在总表中的id
                    if (esId != -1)
                    {
                        if (tagsId == null || tagsId.Length == 0)
                        {
                            code = 0;
                            message = "添加成功";

                            return JsonConvert.SerializeObject(new { code, message });
                        }
                        // 添加标签
                        foreach (int tagId in tagsId)
                        {
                            ES_Tag_Exercise te = new ES_Tag_Exercise()
                            {
                                TagId = tagId,
                                EsId = esId
                            };

                            db.ES_Tag_Exercise.Add(te);
                        }
                        if (db.SaveChanges() >= tagsId.Length)
                        {
                            code = 0;
                            message = "添加成功";

                            return JsonConvert.SerializeObject(new { code, message });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务器错误！" + ex.Message;

                return JsonConvert.SerializeObject(new { code, message });
            }

            code = 1;
            message = "服务器错误！题目添加失败";

            return JsonConvert.SerializeObject(new { code, message });
        }

        /// <summary>
        /// 添加判断题
        /// </summary>
        /// <param name="title"></param>
        /// <param name="score"></param>
        /// <param name="answers"></param>
        /// <param name="tagsId"></param>
        /// <returns></returns>
        public string AddFill(string title, int score, string[] answers, int[] tagsId)
        {
            int code;
            string message;

            ES_FillQuestion fill = new ES_FillQuestion()
            {
                FQTitle = title,
                FQScore = score
            };

            try
            {
                db.ES_FillQuestion.Add(fill);

                if (db.SaveChanges() > 0)
                {
                    var question = db.ES_FillQuestion.OrderByDescending(fq=>fq.FQId).FirstOrDefault();
                    foreach (string ans in answers)
                    {
                        ES_FillAnswer answer = new ES_FillAnswer()
                        {
                            FQId = question.FQId,
                            FAContent = ans
                        };
                        db.ES_FillAnswer.Add(answer);
                    }
                    int addRes = db.SaveChanges();
                    int esId = AddExercise("填空题", question.FQId); // 获取对应在总表中的id
                    if (addRes >= answers.Length && esId != -1)
                    {
                        if (tagsId == null || tagsId.Length == 0)
                        {
                            code = 0;
                            message = "添加成功";

                            return JsonConvert.SerializeObject(new { code, message });
                        }
                        // 添加标签
                        foreach (int tagId in tagsId)
                        {
                            ES_Tag_Exercise te = new ES_Tag_Exercise()
                            {
                                TagId = tagId,
                                EsId = esId
                            };

                            db.ES_Tag_Exercise.Add(te);
                        }
                        if (db.SaveChanges() >= tagsId.Length)
                        {
                            code = 0;
                            message = "添加成功";

                            return JsonConvert.SerializeObject(new { code, message });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务器错误！" + ex.Message;

                return JsonConvert.SerializeObject(new { code, message });
            }

            code = 1;
            message = "服务器错误！题目添加失败";

            return JsonConvert.SerializeObject(new { code, message });
        }
    }
}