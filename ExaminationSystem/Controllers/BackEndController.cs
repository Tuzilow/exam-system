using ExaminationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace ExaminationSystem.Controllers
{
    public class BackEndController : DefaultController
    {
        readonly ExaminationSystemDbEntities db = new ExaminationSystemDbEntities();

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(int pageIndex = 1, int pageSize = 10)
        {
            var userList = UserTableChange(pageIndex, "", pageSize);

            // 序列化为JSON 传递到View
            ViewBag.Users = userList.GetType().GetProperty("users").GetValue(userList).ToString();
            ViewBag.TotalNum = Convert.ToInt32(userList.GetType().GetProperty("totalCount").GetValue(userList));

            return View();
        }

        /// <summary>
        /// 用户列表数据刷新
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object UserTableChange(int pageIndex, string keyword = "", int pageSize = 10)
        {
            var users = from u in db.ES_User
                        join r in db.ES_Role on u.RoleId equals r.RoleId
                        where u.IsDel == false
                        select new
                        {
                            u.UserId,
                            u.UserAccount,
                            u.UserPassword,
                            u.UserName,
                            r.RoleName
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
                string password = user.UserPassword;
                string name = user.UserName;
                string role = user.RoleName;

                userList.Add(new { id, account, password, name, role });
            }

            return new { users = JsonConvert.SerializeObject(userList), totalCount };
        }

        /// <summary>
        /// 添加新用户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string AddUser(string account, string password, string name, string roleId)
        {
            int code;
            string message;
            ES_User user = new ES_User
            {
                UserAccount = account,
                UserPassword = password,
                UserName = name,
                RoleId = Convert.ToInt32(roleId)
            };

            db.ES_User.Add(user);
            try
            {
                if (db.SaveChanges() > 0)
                {
                    code = 0;
                    message = "添加成功！";

                    return JsonConvert.SerializeObject(new { code, message });
                }
            }
            catch
            {
                code = 1;
                message = "添加用户失败，请检查数据是否合规或账号是否已存在！";

                return JsonConvert.SerializeObject(new { code, message });
            }


            code = 1;
            message = "添加用户失败，请检查数据是否合规！";

            return JsonConvert.SerializeObject(new { code, message });
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public string EditUser(int id, string account, string password, string name, string roleId)
        {
            int code;
            string message;

            var user = db.ES_User.Where(u => u.UserId == id && u.IsDel == false).FirstOrDefault();
            if (user != null)
            {
                user.UserAccount = account;
                user.UserPassword = password;
                user.UserName = name;
                user.RoleId = Convert.ToInt32(roleId);

                db.Entry(user).State = System.Data.Entity.EntityState.Modified;

                try
                {
                    if (db.SaveChanges() > 0)
                    {
                        code = 0;
                        message = "修改成功！";

                        return JsonConvert.SerializeObject(new { code, message });
                    }
                }
                catch
                {
                    code = 1;
                    message = "修改失败，服务端错误！";

                    return JsonConvert.SerializeObject(new { code, message });
                }
            }
            code = 1;
            message = "修改失败！用户可能不存在";

            return JsonConvert.SerializeObject(new { code, message });
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string RemoveUser(int id)
        {
            int code;
            string message;

            var user = db.ES_User.Where(u => u.UserId == id && u.IsDel == false).FirstOrDefault();
            if (user != null)
            {
                user.IsDel = true;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;

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
            message = "删除失败！用户可能不存在";

            return JsonConvert.SerializeObject(new { code, message });
        }

        //TODO 批量添加新用户
    }
}