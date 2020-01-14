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
    public class OrganizationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Organization/
        public ActionResult Index()
        {
            return View();
        }

        public JObject orgList()
        {

            List<Organization> list = db.Organizations.ToList();
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
                new JProperty("total", db.Organizations.ToList().Count()),
                new JProperty("rows",
                    new JArray(
                        from r in list
                        select new JObject(
                            new JProperty("ID", r.ID),
                            new JProperty("orgShortName", r.orgShortName),
                            new JProperty("orgName", r.orgName),
                            new JProperty("orgCode", r.orgCode),
                            new JProperty("orgType", r.orgType),
                            new JProperty("orgNot", r.orgNote),
                            new JProperty("_parentId", r.parentId)))));



            return json;
        }
        [HttpPost]

        public ActionResult CreateOrg(Organization org)
        {
            if (ModelState.IsValid)
            {
                Organization org1 = new Organization();
                org1.orgName = org.orgName;
                org1.orgShortName = org.orgShortName;
                org1.orgCode = org.orgCode;
                org1.orgType = org.orgType;
                org1.orgNote = org.orgNote;
                org1.parentId = org.ID;

                db.Organizations.Add(org1);
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
        public ActionResult EditOrg(Organization org)
        {
            if (ModelState.IsValid)
            {
                db.Entry(org).State = EntityState.Modified;
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
        public ActionResult DelOrg(int id, int pid)
        {
            if (pid != 0)
            {

                Organization porg = db.Organizations.Where(t => t.parentId == id).FirstOrDefault();
                if (porg != null)
                {
                    var json = new
                    {
                        errorMsg = "删除失败，需保证此组织机构下没有子组织机构"
                    };
                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {

                    Organization org = db.Organizations.Find(id);
                    if (org == null)
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
                            db.Organizations.Remove(org);
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
                    errorMsg = "根组织机构不能删除"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);

            }

        }
        // GET: /Organization/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // GET: /Organization/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Organization/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Organization organization)
        {
            if (ModelState.IsValid)
            {
                db.Organizations.Add(organization);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(organization);
        }

        // GET: /Organization/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // POST: /Organization/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,orgName,orgCode,orgType,parentId")] Organization organization)
        {
            if (ModelState.IsValid)
            {
                db.Entry(organization).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(organization);
        }

        // GET: /Organization/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = db.Organizations.Find(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // POST: /Organization/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Organization organization = db.Organizations.Find(id);
            db.Organizations.Remove(organization);
            db.SaveChanges();
            return RedirectToAction("Index");
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
