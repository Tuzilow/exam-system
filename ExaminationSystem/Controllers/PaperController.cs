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
        public string AddPaper(string title, int singleNum, int singleScore, int multipleNum, int multipleScore, int judgmentNum, int judgmentScore, int fillNum, int fillScore, int[] tags, int partId, string tagPercent)
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
                EmPtId = partId,
                EmTagPercent = tagPercent
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

        /// <summary>
        /// 获取试卷
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="selectPartId"></param>
        /// <returns></returns>
        public string GetPaper(int pageIndex = 1, int selectPartId = 0)
        {
            int code;
            string message;
            try
            {
                var papers = from p in db.ES_ExamPaper
                             join pt in db.ES_ExamPart on p.EmPtId equals pt.EmPtId
                             where p.IsDel == false && pt.IsDel == false
                             select new
                             {
                                 p,
                                 pt
                             };

                if (selectPartId != 0)
                {
                    papers = papers.Where(p => p.pt.EmPtId == selectPartId);
                }

                int totalCount = papers.Count();

                papers = papers.OrderBy(p => p.p.EmPaperId).Skip((pageIndex - 1) * 10).Take(10);

                // 格式化
                List<object> paperList = new List<object>();

                foreach (var paper in papers)
                {
                    int id = paper.p.EmPaperId;
                    string title = paper.p.EmPaperName;
                    int singleNum = paper.p.EmPaperSelectNum;
                    int singleScore = paper.p.EmPaperSelectScore;
                    int multipleNum = paper.p.EmPaperMultipleNum;
                    int multipleScore = paper.p.EmPaperMultipleScore;
                    int judgmentNum = paper.p.EmPaperJudgeNum;
                    int judgmentScore = paper.p.EmPaperJudgeScore;
                    int fillNum = paper.p.EmPaperFillNum;
                    int fillScore = paper.p.EmPaperFillScore;

                    // 场次
                    int partId = paper.pt.EmPtId;
                    string tempDate = paper.pt.EmPtStart.GetDateTimeFormats('o')[0];
                    string date = tempDate.Substring(0, tempDate.IndexOf('T')); // 将日期转为2020-05-06
                    string start = paper.pt.EmPtStart.ToString("T"); // 开始时间
                    string end = paper.pt.EmPtEnd.ToString("T"); // 结束时间
                    string partTitle = start.Substring(0, start.Length - 3) + "到" + end.Substring(0, end.Length - 3) + "场";

                    // 获取标签
                    var tags = from pt in db.ES_Paper_Tag
                               join t in db.ES_Tag on pt.TagId equals t.TagId
                               where t.IsDel == false && pt.IsDel == false && pt.PaperId == id
                               select new
                               {
                                   t.TagName
                               };

                    List<string> tagList = new List<string>();

                    foreach (var tag in tags)
                    {
                        tagList.Add(tag.TagName);
                    }

                    paperList.Add(new
                    {
                        id,
                        title,
                        singleNum,
                        singleScore,
                        multipleNum,
                        multipleScore,
                        judgmentNum,
                        judgmentScore,
                        fillNum,
                        fillScore,
                        partId,
                        date,
                        partTitle,
                        tags = tagList
                    });
                }

                paperList.Add(totalCount);

                // 序列化为JSON 传递到View
                return JsonConvert.SerializeObject(paperList);
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务器错误！" + ex;

                return JsonConvert.SerializeObject(new { code, message });
            }
        }

        /// <summary>
        /// 删除试卷
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string RemovePaper(int id)
        {
            int code;
            string message;

            var paper = db.ES_ExamPaper.Where(p => p.EmPaperId == id && p.IsDel == false).FirstOrDefault();


            if (paper != null)
            {
                try
                {
                    paper.IsDel = true;
                    db.Entry(paper).State = System.Data.Entity.EntityState.Modified;

                    if (db.SaveChanges() > 0)
                    {
                        code = 0;
                        message = "删除成功";

                        return JsonConvert.SerializeObject(new { code, message });
                    }
                }
                catch (Exception ex)
                {
                    code = 1;
                    message = "服务端错误" + ex;

                    return JsonConvert.SerializeObject(new { code, message });
                }
            }
            code = 1;
            message = "未找到该记录";

            return JsonConvert.SerializeObject(new { code, message });
        }
    }
}