using ExaminationSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExaminationSystem.Controllers
{
    public class LoginController : Controller
    {
        readonly ExaminationSystemDbEntities db = new ExaminationSystemDbEntities();

        public ActionResult Index(string ReturnUrl)
        {
            if (Session["account"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Url = ReturnUrl;
            return View();
        }

        [HttpPost]
        public string Index(string account, string password)
        {
            var user = (from u in db.ES_User
                        join r in db.ES_Role on u.RoleId equals r.RoleId
                        where u.IsDel == false
                            && u.UserAccount == account
                            && u.UserPassword == password
                        select new
                        {
                            u.UserId,
                            u.UserAccount,
                            u.UserPassword,
                            u.UserName,
                            r.RoleId,
                            r.RoleName
                        }).FirstOrDefault();

            if (user != null)
            {
                Session["account"] = account;
                Session.Timeout = 150; // 150分钟后过期
                int id = user.UserId;
                string name = user.UserName;
                string role = user.RoleName;
                int roleId = user.RoleId;
                DateTime loginTime = DateTime.Now;
                DateTime endTime = loginTime.AddMinutes(Session.Timeout);
                return JsonConvert.SerializeObject(new { id, account, name, roleId, role, loginTime, endTime });
            }
            int code = 1;
            string errMessage = "登陆失败，请检查账号和密码是否正确！";
            return JsonConvert.SerializeObject(new { code, errMessage });
        }


        public ActionResult LogOut()
        {
            Session["account"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}