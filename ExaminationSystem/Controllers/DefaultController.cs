using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExaminationSystem.Controllers
{
    public class DefaultController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            var account = Session["account"] as String;
            if (String.IsNullOrEmpty(account))
            {
                //重定向至登录页面
                filterContext.Result = RedirectToAction("Index", "Login");
                return;
            }

        }
    }
}