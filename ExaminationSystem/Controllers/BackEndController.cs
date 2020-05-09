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

            // 序列化为JSON 传递到View
            return new { users = JsonConvert.SerializeObject(userList), totalCount };
        }
    }
}