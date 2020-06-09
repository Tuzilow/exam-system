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
        public string GetExamPart(int pageIndex = 1, bool isPaging = true)
        {
            int code;
            string message;
            try
            {
                // 获取所有考试场次
                var exams = from ep in db.ES_ExamPart where ep.IsDel == false select ep;

                int totalCount = exams.Count();

                // 判断是否需要分页
                if (isPaging)
                {
                    exams = exams.OrderByDescending(e => e.EmPtStart).Skip((pageIndex - 1) * 10).Take(10);
                }
                else
                {
                    exams = exams.OrderByDescending(e => e.EmPtStart);
                }

                // 格式化
                List<object> examList = new List<object>();

                foreach (var exam in exams)
                {
                    int id = exam.EmPtId;
                    string tempDate = exam.EmPtStart.GetDateTimeFormats('o')[0];
                    string date = tempDate.Substring(0, tempDate.IndexOf('T')); // 将日期转为2020-05-06
                    string start = exam.EmPtStart.ToString("T"); // 开始时间
                    string end = exam.EmPtEnd.ToString("T"); // 结束时间
                    string title = start.Substring(0, start.Length - 3) + "到" + end.Substring(0, end.Length - 3) + "场";

                    examList.Add(new { id, date, start, end, title });
                }

                examList.Add(totalCount);

                // 序列化为JSON 传递到View
                return JsonConvert.SerializeObject(examList);
            }
            catch (Exception ex)
            {
                code = 1;
                message = "服务器错误！" + ex;

                return JsonConvert.SerializeObject(new { code, message });
            }
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
            var users = db.ES_User.Where(u => u.IsDel == false && u.UserId != 1 && u.RoleId == 1);
            var parts = from up in db.ES_User_ExamPart
                        join p in db.ES_ExamPart on up.EmPtId equals p.EmPtId
                        where p.IsDel == false && up.IsDel == false
                        select new
                        {
                            up.UserId,
                            p.EmPtId,
                            p.EmPtStart,
                            p.EmPtEnd
                        };

            List<object> userList = new List<object>();

            foreach (var user in users)
            {
                int key = user.UserId;
                string label = user.UserName;
                bool disabled = false;

                List<object> haveParts = new List<object>();

                var part = parts.Where(p => p.UserId == user.UserId);
                if (part != null)
                {
                    foreach (var item in part)
                    {
                        string start = item.EmPtStart.ToString("yyyy/MM/dd HH:mm:ss");
                        string end = item.EmPtEnd.ToString("yyyy/MM/dd HH:mm:ss");
                        haveParts.Add(new { start, end });
                    }
                }

                userList.Add(new { key, label, disabled, haveParts });
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
                foreach (var user_part in user_parts)
                {
                    user_part.IsDel = true;
                    db.Entry(user_part).State = System.Data.Entity.EntityState.Modified;
                }
            }


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

        /// <summary>
        /// 获取考生
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="keyword"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object GetUsers(int pageIndex, string keyword = "", int pageSize = 10)
        {
            var users = from u in db.ES_User
                        join ue in db.ES_User_ExamPart on u.UserId equals ue.UserId
                        where u.IsDel == false && u.RoleId == 1 && ue.IsDel == false
                        select new
                        {
                            u.UserId,
                            u.UserAccount,
                            u.UserName,
                            ue.IsJoin,
                            ue.EmPtId
                        };

            if (keyword != "")
            {
                users = users.Where(u => u.UserName.Contains(keyword));
            }

            int totalCount = users.Count();
            int totalPages = Convert.ToInt32(Math.Ceiling((double)totalCount / pageSize));

            users = users.OrderBy(u => u.UserId).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            List<object> userList = new List<object>();

            foreach (var user in users)
            {
                int id = user.UserId;
                string account = user.UserAccount;
                string name = user.UserName;
                string isJoin = user.IsJoin == false ? "否" : "是";
                int partId = Convert.ToInt32(user.EmPtId);

                userList.Add(new { id, account, name, isJoin, partId });
            }

            string userInfo = JsonConvert.SerializeObject(userList);

            return JsonConvert.SerializeObject(new { userInfo, totalCount });
        }

        /// <summary>
        /// 更改场次
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="partId"></param>
        /// <returns></returns>
        [HttpPost]
        public string ChangeUserPart(int userId, int partId)
        {
            int code;
            string message;

            var user = db.ES_User_ExamPart.Where(ue => ue.UserId == userId && ue.IsDel == false).FirstOrDefault();

            if (user != null)
            {
                try
                {
                    user.EmPtId = partId;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;

                    var log = (from l in db.ES_ExamLog where l.UserId == user.UserId && l.IsDel == false select l).FirstOrDefault();

                    if (log != null)
                    {
                        var paper = (from p in db.ES_ExamPaper
                                     where p.ES_ExamPart.EmPtId == partId && p.IsDel == false
                                     select p).FirstOrDefault();

                        if (paper != null)
                        {
                            log.EmPaperId = paper.EmPaperId;
                        }

                        log.EmPtId = partId;
                        db.Entry(log).State = System.Data.Entity.EntityState.Modified;
                    }

                    if (db.SaveChanges() > 0)
                    {
                        code = 0;
                        message = "修改成功";

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