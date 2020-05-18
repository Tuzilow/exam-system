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
                        if (tagsId == null  || tagsId.Length == 0)
                        {
                            code = 0;
                            message = "添加成功";

                            return JsonConvert.SerializeObject(new { code, message });
                        }
                        if (esId != -1)
                        {
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
    }
}