using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Apache.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Reflection;

namespace Apache.Filters
{
    public class LoggingAttribute : ActionFilterAttribute
    {


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //filterContext.HttpContext.Trace.Write("(Logging Filter)Action Executing: " +
            //filterContext.ActionDescriptor.ActionName);
            //base.OnActionExecuting(filterContext);
            ApplicationDbContext db = new ApplicationDbContext();
            if (!(filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)))
            {
                string currentUserId = filterContext.HttpContext.User.Identity.GetUserId();
                ApplicationUser user = db.Users.Include(i => i.Roles).FirstOrDefault(i => i.Id == currentUserId);
                Log syslog = new Log();
                syslog.ID = Guid.NewGuid();
                syslog.logTime = DateTime.Now;
                syslog.logUser = user.UserName;
                syslog.logUserIp = GetIP();
                syslog.logController = filterContext.RouteData.Values["controller"].ToString();
                syslog.logAction = filterContext.RouteData.Values["action"].ToString();
                syslog.logPram = GetParmValue(filterContext);
                db.Logs.Add(syslog);
                db.SaveChanges();
            }

        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //if (filterContext.Exception != null)
            //    filterContext.HttpContext.Trace.Write("(Logging Filter)Exception thrown");
            //base.OnActionExecuted(filterContext);
        }
        private string GetIP()
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(ip))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            return ip;
        }
        private string GetParmValue(ActionExecutingContext filterContext)
        {
            string pv = "";
            int parametersCount = filterContext.ActionParameters.Count;
            if (parametersCount > 0)
            {
                var keys = filterContext.ActionParameters.Keys;
                if (null != keys)
                {
                    foreach (string key in keys)
                    {
                        var value = filterContext.ActionParameters[key];
                        if (null == value)
                            continue;
                        if (value.GetType().IsClass && value.GetType().Name != "String")
                        {
                            object objClass = value;
                            PropertyInfo[] infos = objClass.GetType().GetProperties();
                            foreach (PropertyInfo info in infos)
                            {
                                if (info.CanRead)
                                {
                                    pv += info.Name + "=" + info.GetValue(objClass, null) + ",";
                                }
                            }

                        }
                        else
                        {
                            pv += key + "=" + value + ",";
                        }

                    }
                }
            }
            return pv;
        }

    }
}