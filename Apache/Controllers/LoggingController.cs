using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Apache.Models;
//using Apache.DAL;
using Newtonsoft.Json.Linq;
using Apache.Filters;

namespace Apache.Controllers
{
    [ApachAuthentication]
    public class LoggingController:Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Organization/
        public ActionResult Index()
        {
            return View();
        }
        public JObject LogList(int? page, int? rows, string logUser, DateTime? logTimeB, DateTime? logTimeE)
        {

            int totalCount;
            var query = db.Logs.AsQueryable();
            List<Log> list;

            totalCount = db.Logs.ToList().Count();


            if (!string.IsNullOrWhiteSpace(logUser))
            {
                query = query.Where(i => i.logUser.Contains(logUser));
                totalCount = query.ToList().Count();
            }

            if (!string.IsNullOrWhiteSpace(logTimeB.ToString()) || !string.IsNullOrWhiteSpace(logTimeE.ToString()))
            {
                if (null == logTimeB)
                {
                    logTimeB = DateTime.Now;
                }
                if (null == logTimeE)
                {
                    logTimeE = DateTime.Now;
                }
                else
                {
                    logTimeE = logTimeE.Value.AddDays(1);
                }
                query = query.Where(i => i.logTime >= logTimeB && i.logTime < logTimeE);
                //query = query.Where(w => w.logTime.ToString().Contains(logTime));
                totalCount = query.ToList().Count();
            }
 
           list = query.OrderByDescending(c => c.logTime).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();
   
            JObject json = new JObject(
                new JProperty("total", totalCount),
                new JProperty("rows",
                    new JArray(
                        from r in list
                        select new JObject(
                            new JProperty("ID", r.ID),
                            new JProperty("logTime", r.logTime),
                            new JProperty("logUser", r.logUser),
                            new JProperty("logUserIp", r.logUserIp),
                            new JProperty("logOperateName",GetUsersOperateName(r.logController,r.logAction)+ r.logController+","+r.logAction),
                            new JProperty("logPram", r.logPram)))));



            return json;
        }

        public string GetUsersOperateName(string controllerName, string actionName)
        {
            string optName = "";
            int parentid=0;
            string modelName = "";
            var opt = db.Menus.FirstOrDefault(i => i.menuController == controllerName && i.menuAction == actionName);
            if (opt != null)
            {
                optName = opt.menuName;
                parentid = opt.parentId;
            }
            var model = db.Menus.FirstOrDefault(i => i.ID == parentid);
            if (model != null)
            {
                modelName = model.menuName;
            }
            return modelName+"->"+optName;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}