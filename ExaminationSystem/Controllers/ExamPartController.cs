using ExaminationSystem.Models;
using ExaminationSystem.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExaminationSystem.Controllers
{
    public class ExamPartController : Controller
    {
        readonly ExaminationSystemDbEntities db = new ExaminationSystemDbEntities();
        public string GetExamPart(int pageIndex = 1)
        {
            // 获取所有考试场次
            var exams = from ep in db.ES_ExamPart where ep.IsDel == false select ep;

            int totalCount = exams.Count();

            exams = exams.OrderByDescending(e => e.EmPtStart).Skip((pageIndex - 1) * 10).Take(10);

            // 格式化
            List<object> examList = new List<object>();

            foreach (var exam in exams)
            {
                int id = exam.EmPtId;
                string tempDate = exam.EmPtStart.GetDateTimeFormats('o')[0];
                string date = tempDate.Substring(0, tempDate.IndexOf('T')); // 将日期转为2020-05-06
                string start = exam.EmPtStart.ToString("T"); // 开始时间
                string end = exam.EmPtEnd.ToString("T"); // 结束时间

                examList.Add(new { id, date, start, end });
            }

            examList.Add(totalCount);

            // 序列化为JSON 传递到View
            return JsonConvert.SerializeObject(examList);
        }

        /// <summary>
        /// 根据场次获取考生
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetUserByExamPart(string id)
        {
            int emptId = Convert.ToInt32(id);
            var users = from u in db.ES_User
                        join e_u_part in db.ES_User_ExamPart on u.UserId equals e_u_part.UserId
                        join exam_part in db.ES_ExamPart on e_u_part.EmPtId equals exam_part.EmPtId
                        where exam_part.EmPtId == emptId && u.RoleId == 1 && u.IsDel == false
                        select new
                        {
                            u.UserId,
                            u.UserAccount,
                            u.UserName
                        };

            List<object> userList = new List<object>();

            foreach (var user in users)
            {
                int userId = user.UserId;
                string account = user.UserAccount;
                string name = user.UserName;

                userList.Add(new { userId, account, name });
            }

            return JsonConvert.SerializeObject(userList);
        }

        /// <summary>
        /// 穿梭框限定格式学生数据
        /// </summary>
        /// <returns></returns>
        public string GetUser()
        {
            // 查找UserId不存在于ES_User_ExamPart表中的User数据
            var users = db.ES_User.Where(
                u => u.IsDel == false &&
                u.RoleId == 1 &&
                !db.ES_User_ExamPart.Where(ue => ue.IsDel == false).Select(ue => ue.UserId).Contains(u.UserId)
                );

            List<object> userList = new List<object>();

            foreach (var user in users)
            {
                int key = user.UserId;
                string label = user.UserName;
                bool disabled = false;

                userList.Add(new { key, label, disabled });
            }

            return JsonConvert.SerializeObject(userList);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public string Remove(int id)
        {
            int code;
            string message;

            var part = db.ES_ExamPart.Where(e => e.EmPtId == id && e.IsDel == false).FirstOrDefault();
            var user_parts = db.ES_User_ExamPart.Where(ue => ue.EmPtId == id && ue.IsDel == false);

            // 先删除中间表
            if (user_parts != null)
            {
                foreach(var user_part in user_parts)
                {
                    user_part.IsDel = true;
                }
            }

            db.Entry(user_parts).State = System.Data.Entity.EntityState.Modified;

            if (part != null)
            {
                part.IsDel = true;
                db.Entry(part).State = System.Data.Entity.EntityState.Modified;

                try
                {
                    if (db.SaveChanges() > 0)
                    {
                        code = 0;
                        message = "删除成功！";

                        return JsonConvert.SerializeObject(new { code, message });
                    }
                }
                catch
                {
                    code = 1;
                    message = "删除失败，服务端错误！";

                    return JsonConvert.SerializeObject(new { code, message });
                }
            }
            code = 1;
            message = "删除失败！场次可能不存在";

            return JsonConvert.SerializeObject(new { code, message });
        }

        /// <summary>
        /// 添加场次
        /// </summary>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="userIds">考生ID</param>
        /// <returns></returns>
        [HttpPost]
        public string CreateNewPart(long start, long end, int[] userIds)
        {
            int code;
            string message;

            DateTime startTime = TimeHelper.ToDateTime(start);
            DateTime endTime = TimeHelper.ToDateTime(end);

            ES_ExamPart part = new ES_ExamPart()
            {
                EmPtStart = startTime,
                EmPtEnd = endTime
            };
            db.ES_ExamPart.Add(part);
            try
            {
                // 先创建新的场次
                if (db.SaveChanges() > 0)
                {
                    // 获取场次ID
                    var newPart = db.ES_ExamPart.OrderByDescending(e => e.EmPtId).FirstOrDefault();

                    int partId = newPart.EmPtId;

                    // 添加考生ID与场次ID的关联
                    foreach (int id in userIds)
                    {
                        ES_User_ExamPart user_part = new ES_User_ExamPart()
                        {
                            UserId = id,
                            EmPtId = partId
                        };
                        db.ES_User_ExamPart.Add(user_part);
                    }

                    if (db.SaveChanges() == userIds.Length)
                    {
                        code = 0;
                        message = "添加成功！";

                        return JsonConvert.SerializeObject(new { code, message });
                    }

                    code = 1;
                    message = "添加失败！考生添加出错";

                    return JsonConvert.SerializeObject(new { code, message });
                }

                code = 1;
                message = "添加失败！场次添加出错";

                return JsonConvert.SerializeObject(new { code, message });
            }
            catch (Exception ex)
            {
                code = 1;
                message = "添加失败！服务器错误:" + ex.Message;

                return JsonConvert.SerializeObject(new { code, message });
            }
        }
    }
}