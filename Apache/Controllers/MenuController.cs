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
    public class MenuController : Controller
    {
        //
        // GET: /Menu/
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Organization/
        public ActionResult Index()
        {
            return View();
        }

        public JObject menuList()
        {

            List<Menu> list = db.Menus.ToList();
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
                new JProperty("total", db.Menus.ToList().Count()),
                new JProperty("rows",
                    new JArray(
                        from r in list
                        select new JObject(
                            new JProperty("ID", r.ID),
                            new JProperty("menuName", r.menuName),
                            new JProperty("menuType", r.menuType),
                            new JProperty("isMenu", r.isMenu),
                            new JProperty("menuController", r.menuController),
                            new JProperty("menuAction", r.menuAction),
                            new JProperty("menuOrder", r.menuOrder),
                            new JProperty("menuNote", r.menuNote),
                            new JProperty("menuIcon", r.menuIcon),
                            new JProperty("isSysMenu", r.isSysMenu),
                            new JProperty("_parentId", r.parentId)))));



            return json;
        }
        [HttpPost]

        public ActionResult CreateMenu(Menu menu)
        {
            if (ModelState.IsValid)
            {
                Menu m = new Menu();
                m.menuName = menu.menuName;
                m.menuType = menu.menuType;
                m.isMenu = menu.isMenu;
                m.menuController = menu.menuController;
                m.menuAction = menu.menuAction;
                m.menuOrder = menu.menuOrder;
                m.menuIcon = menu.menuIcon;
                m.parentId = menu.ID;
                m.menuNote = menu.menuNote;
                db.Menus.Add(m);
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
        public ActionResult EditMenu(Menu m)
        {

            if (ModelState.IsValid)
            {
                Menu menu = db.Menus.Find(m.ID);
                if (menu != null)
                {
                    if (menu.isSysMenu != "1")          //判断是否系统模块
                    {
                        menu.menuName = m.menuName;
                        menu.menuType = m.menuType;
                        menu.isMenu = m.isMenu;
                        menu.menuController = m.menuController;
                        menu.menuAction = m.menuAction;
                        menu.menuOrder = m.menuOrder;
                        menu.menuIcon = m.menuIcon;
                        menu.menuNote = m.menuNote;
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
                            errorMsg = "系统模块不能编辑"
                        };

                        return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    var json = new
                    {
                        errorMsg = "未找到需要编辑的模块"
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
        public ActionResult DelMenu(int id, int pid)
        {
            if (pid != 0)
            {
                Menu pmenu = db.Menus.Where(t => t.parentId == id).FirstOrDefault();
                if (pmenu != null)
                {
                    var json = new
                    {
                        errorMsg = "删除失败，需保证此菜单下没有子菜单"
                    };
                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Menu menu = db.Menus.Find(id);
                    if (menu.isSysMenu != "1")
                    {
                        if (menu == null)
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
                                db.Menus.Remove(menu);
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
                    else
                    {
                        var json = new
                        {
                            errorMsg = "系统模块不能删除"
                        };

                        return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {

                var json = new
                {
                    errorMsg = "根模块不能删除"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);

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