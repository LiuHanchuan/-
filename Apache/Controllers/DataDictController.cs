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
    [Logging]
    public class DataDictController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /DataDict/
        public ActionResult Index()
        {
            return View();
        }

        public JObject DataDictList()
        {

            List<DataDict> list = db.DataDicts.ToList();
            //var json = new
            //{
            //    total = db.Organizations.ToList().Count(),
            //    rows = (from r in list
            //            select new
            //            {
            //                ID = r.ID,
            //                orgName = r.orgName,
            //                orgCode = r.orgCode,
            //                orgType = r.orgType,
            //                orgNote = r.orgNote,
            //                _parentId = r.parentId

            //            }).ToArray()
            //};
            JObject json = new JObject(
                new JProperty("total", db.DataDicts.ToList().Count()),
                new JProperty("rows",
                    new JArray(
                        from r in list
                        select new JObject(
                            new JProperty("ID", r.ID),
                            new JProperty("DataDictName", r.DataDictName),
                            new JProperty("DataDictCode", r.DataDictCode),
                            new JProperty("DataDictType", r.DataDictType),
                            new JProperty("DataDictNote", r.DataDictNote),
                            new JProperty("_parentId", r.parentId)))));



            return json;
        }
        [HttpPost]

        public ActionResult CreateDataDict(DataDict dd)
        {
            if (ModelState.IsValid)
            {
                DataDict dd1 = new DataDict();
                dd1.DataDictName = dd.DataDictName;
                dd1.DataDictCode = dd.DataDictCode;
                dd1.DataDictType = dd.DataDictType;
                dd1.DataDictNote = dd.DataDictNote;
                dd1.parentId = dd.ID;

                db.DataDicts.Add(dd1);
                try
                {
                    db.SaveChanges();
                    var json = new
                    {
                        okMsg = "创建成功"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
                catch
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
        public ActionResult EditDataDict(DataDict dd)
        {
            if (ModelState.IsValid)
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
                catch
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
        public ActionResult DelDataDict(int id, int pid)
        {
            if (pid != 0)
            {

                DataDict porg = db.DataDicts.Where(t => t.parentId == id).FirstOrDefault();
                if (porg != null)
                {
                    var json = new
                    {
                        errorMsg = "删除失败，需保证此数据字典下没有子数据字典"
                    };
                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {

                    DataDict dd = db.DataDicts.Find(id);
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
                            db.DataDicts.Remove(dd);
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
            }
            else
            {

                var json = new
                {
                    errorMsg = "根数据字典不能删除"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);

            }

        }
        // GET: /Organization/Details/5

   
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
