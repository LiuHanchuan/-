using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Threading;
using System.Configuration;
using System.Runtime.DurableInstancing;
using Microsoft.Activities.Extensions;
using Microsoft.Activities.Extensions.Tracking;
using System.Xaml;
using System.Reflection;
using System.Activities.XamlIntegration;
using Apache.WorkFlow;
using Apache.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Activities.Tracking;
using Newtonsoft.Json.Linq;
using Apache.Filters;
using System.Globalization;
using System.Data;
using Apache.Helper;

namespace Apache.Controllers
{
    [ApachAuthentication]
    [Authorize]
    [Logging]
    public class SpotfireController : Controller
    {
        private HosDataConnect conn = new HosDataConnect();
        private ApplicationDbContext db = new ApplicationDbContext();
        private BusinessDataContext bdb = new BusinessDataContext();
        public ActionResult Lib()
        {
            return View();
        }
        public ActionResult MedicalRecord()
        {
            return View();
        }
        public ActionResult Performance()
        {
            return View();
        }
        public ActionResult ClinicData()
        {
            return View();
        }
        public ActionResult Others()
        {
            return View();
        }
        public ActionResult LibManage()
        {
            return View();
        }
        public ActionResult DxpFrame()
        {
            return View();
        }
        public ActionResult Platform()
        {
            return View();
        }
        public ActionResult SMSSend(string ver_code, string libID)
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Include(i => i.Roles).FirstOrDefault(i => i.Id == currentUserId);
            List<Libraries> list = db.Libraries.ToList();
            String res = String.Empty;

            String smscontext = "'" + user.TrueName.Trim() + "正在请求登录" + list.First(p => p.LibraryID.ToString() == libID).LibraryName + "专病库，请及时确认短信验证码：" + ver_code + " （有效期为10分钟）'";

            var phone = list.FirstOrDefault(p => p.LibraryID.ToString() == libID).Phone.Trim().Split(',');
            String sql = String.Empty;
            conn.OpenConnection();
            foreach (string str in phone)
            {
                sql += string.Format(@"insert into TT_MT_QUEUE(APP_ID,DESTINATION_ADDR,CONTENT) values ({0},{1},{2});", "'专病库管理系统'", str, smscontext);
            }
            
            if (conn.InsertData(sql) > 0)
            {
                res = "短信发送成功";
            }
            conn.CloseConnection();
            var json = new
            {
                okMsg = res
            };
            return Json(json, "text/html", JsonRequestBehavior.AllowGet);
        }

        public ActionResult getUser()
        {
            string currentUserId = User.Identity.GetUserId();
            //List<Libraries> list = db.Libraries.ToList();

            ApplicationUser user = db.Users.Include(i => i.Roles).FirstOrDefault(i => i.Id == currentUserId);  
            var json = new
            {
                User = user.UserName,
                PassWord = user.PW
                //AllowLibId = user.AllowLibID
            };
            return Json(json, "text/html", JsonRequestBehavior.AllowGet);
        }

        public JObject Init(string libtype)
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Include(i => i.Roles).FirstOrDefault(i => i.Id == currentUserId);
            var rolelist = db.Roles.ToList();
            string username = user.UserName;
            string password = user.PW;
            string url = "http://172.30.100.145:90/spotfire/resources/custom-login/custom-login-app-example.html?sessionid=&libraryname=";
            string userinfo = "&username=" + username + "&password=" + password;
            JObject json = new JObject();
            if (!string.IsNullOrEmpty(user.AllowLibID))
            {
                List<Libraries> liblist = db.Libraries.ToList().FindAll(p => p.LibraryType == libtype);
                if (user.Roles.FirstOrDefault().RoleId == rolelist.Find(p => p.Name == "运维管理员").Id)
                {
                    liblist.ForEach(p => p.IsSMSVerify = 0);
                }
                var allowid = user.AllowLibID.Trim().Split(',').ToList().FindAll(p => liblist.Exists(q => q.LibraryID.ToString() == p));
                //List<ItServiceItem> isilist = bdb.ItServiceItems.ToList().FindAll(p => (p.drafter == user.UserName)&&(p.Status == 2)&&(p.isiCompleteDate > DateTime.Now.AddDays(-30))&& (p.isitype == "4"));
                json = new JObject(
                    new JProperty("total", liblist.Count()),
                    new JProperty("rows",
                        new JArray(
                            from r in allowid
                            select new JObject(
                                new JProperty("LibraryID", r),
                                new JProperty("LibraryName", liblist.FirstOrDefault(p => p.LibraryID.ToString() == r).LibraryName),
                                new JProperty("Icon", liblist.FirstOrDefault(p => p.LibraryID.ToString() == r).Icon),
                                new JProperty("SMSFlag", liblist.FirstOrDefault(p => p.LibraryID.ToString() == r).IsSMSVerify),
                                new JProperty("Url", url + liblist.FirstOrDefault(p => p.LibraryID.ToString() == r).Path + userinfo),
                                //new JProperty("ExportFlag", isilist.Exists(p => p.LibraryID.ToString() == r)?1:0),
                                new JProperty("TrueUrl", liblist.FirstOrDefault(p => p.LibraryID.ToString() == r).Url)
                                ))));
            }
            return json;
        }
        public JObject LibList()
        {
            List<Libraries> list = db.Libraries.ToList();
            JObject json = new JObject(
                new JProperty("total", list.Count()),
                new JProperty("rows",
                    new JArray(
                        from r in list
                        select new JObject(
                            new JProperty("LibraryID", r.LibraryID),
                            new JProperty("LibraryName", r.LibraryName),
                            new JProperty("Owner", r.Owner),
                            new JProperty("Phone", r.Phone),
                            new JProperty("LibraryType", r.LibraryType),
                            new JProperty("_parentId", r.ParentID),
                            new JProperty("Path", r.Path),
                            new JProperty("Url", r.Url),
                            new JProperty("Icon", r.Icon),
                            new JProperty("IsSMSVerify", r.IsSMSVerify)
                            ))));

            return json;
        }
        [HttpPost]

        public ActionResult CreateLib(Libraries lib)
        {
            if (ModelState.IsValid)
            {
                Libraries lib1 = new Libraries();
                lib1 = lib;
                //lib1.LibraryName = lib.LibraryName;
                //lib1.Owner = lib.Owner;
                //lib1.Phone = lib.Phone;
                //lib1.LibraryType = lib.LibraryType;
                lib1.ParentID = 0;

                db.Libraries.Add(lib1);
                try
                {
                    db.SaveChanges();
                    var json = new
                    {
                        okMsg = "创建成功"
                    };
                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                    
                }
                catch (Exception e)
                {
                    var json = new
                    {
                        errorMsg = "保存数据出错"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var json = new
                {
                    errorMsg = "验证错误"

                };
                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult EditLib(Libraries dd)
        {
            db.Entry(dd).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                var json = new
                {
                    okMsg = "修改成功"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var json = new
                {
                    errorMsg = "保存数据出错"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DelLib(int id)
        {
            Libraries dd = db.Libraries.Find(id);
            if (dd == null)
            {
                var json = new
                {
                    errorMsg = "删除失败"
                };
                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    db.Libraries.Remove(dd);
                    db.SaveChanges();
                    var json = new
                    {
                        okMsg = "删除成功"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);

                }
                catch
                {
                    var json = new
                    {
                        errorMsg = "数据库操作失败"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
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
