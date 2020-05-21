using ExaminationSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExaminationSystem.Controllers
{
    public class PaperController : Controller
    {
        readonly ExaminationSystemDbEntities db = new ExaminationSystemDbEntities();

        /// <summary>
        /// 添加试卷
        /// </summary>
        /// <param name="title"></param>
        /// <param name="singleNum"></param>
        /// <param name="singleScore"></param>
        /// <param name="multipleNum"></param>
        /// <param name="multipleScore"></param>
        /// <param name="judgmentNum"></param>
        /// <param name="judgmentScore"></param>
        /// <param name="fillNum"></param>
        /// <param name="fillScore"></param>
        /// <param name="tags"></param>
        /// <param name="partId"></param>
        /// <returns></returns>
        public string AddPaper(string title, int singleNum, int singleScore, int multipleNum, int multipleScore, int judgmentNum, int judgmentScore, int fillNum, int fillScore, int[] tags, int partId)
        {
            int code;
            string message;

            ES_ExamPaper paper = new ES_ExamPaper()
            {
                EmPaperName = title,
                EmPaperSelectNum = singleNum,
                EmPaperSelectScore = singleScore,
                EmPaperMultipleNum = multipleNum,
                EmPaperMultipleScore = multipleScore,
                EmPaperJudgeNum = judgmentNum,
                EmPaperJudgeScore = judgmentScore,
                EmPaperFillNum = fillNum,
                EmPaperFillScore = fillScore,
                EmPtId = partId
            };
            try
            {
                db.ES_ExamPaper.Add(paper);

                if (db.SaveChanges() > 0)
                {
                    if (tags == null || tags.Length == 0)
                    {
                        code = 0;
                        message = "添加成功！";
                        return JsonConvert.SerializeObject(new { code, message });
                    }

                    // 获取添加的试卷对象
                    var newPaper = db.ES_ExamPaper.Where(p => p.IsDel == false).OrderByDescending(p => p.EmPaperId).FirstOrDefault();

                    // 添加试卷与tag的关联
                    if (newPaper != null)
                    {
                        for (int i = 0; i < tags.Length; i++)
                        {
                            ES_Paper_Tag pt = new ES_Paper_Tag()
                            {
                                TagId = tags[i],
                                PaperId = newPaper.EmPaperId
                            };
                            db.ES_Paper_Tag.Add(pt);
                        }
                        if (db.SaveChanges() >= tags.Length)
                        {
                            code = 0;
                            message = "添加成功！";
                            return JsonConvert.SerializeObject(new { code, message });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                code = 1;
                message = "试卷添加失败！" + ex.Message;
                return JsonConvert.SerializeObject(new { code, message });
            }


            code = 1;
            message = "试卷添加失败！服务端错误";
            return JsonConvert.SerializeObject(new { code, message });
        }


    }
}