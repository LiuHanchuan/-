using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Apache.Models;
using Apache.ViewModels;
using Apache.Filters;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Apache.Controllers
{
   [Authorize]
   [Logging]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private BusinessDataContext bdb = new BusinessDataContext();
        public ActionResult Index()
        {
            string currentUserId = User.Identity.GetUserId();
            // ApplicationUser user = db.Users.FirstOrDefault(x => x.Id == currentUserId);

            ApplicationUser user = db.Users.Include(i => i.Roles).FirstOrDefault(i => i.Id == currentUserId);
            ViewBag.trueName = user.TrueName;
            var username = user.UserName;
            var uml = new UserMenuList();
            var ump = new UserMenuList();
            if (username == "admin")          //如果是管理员，则加载所有菜单
            {
                uml.Menus = db.Menus.ToList();
            }
            else
            {
                var user_roles = user.Roles.ToList();
                var rolesid = user_roles.Select(c => c.RoleId).ToList();
                //db.Menus.Include(i=>i.ApplicationRoles).Where(i=>i.ApplicationRoles.id)
                var role = db.Roles.Include(i => i.Menus).Where(i => rolesid.Contains(i.Id)).ToList();
                
                foreach (var r in role)
                {
                    if (uml.Menus == null)
                    {
                        uml.Menus = r.Menus.ToList();
                    }
                    else
                    {
                        uml.Menus = uml.Menus.Union(r.Menus.ToList());
                    }
                }
            }
            if (uml.Menus != null)
            {

                var rootMenu = uml.Menus.FirstOrDefault(i => i.menuType == "根模块");
                if (rootMenu != null)
                {
                    var rootMenuId = rootMenu.ID;

                    var catalogMenu = uml.Menus.Where(i => i.parentId == rootMenuId && i.isMenu == "是");       //非菜单模块不加载

                    var UserMenuPackages = (from r in catalogMenu
                                            select new UserMenuPackage
                                            {
                                                menuName = r.menuName,
                                                menuType = r.menuType,
                                                menuOrder = r.menuOrder,
                                                menuIcon = r.menuIcon,
                                                Menus = uml.Menus.Where(i => i.parentId == r.ID).OrderByDescending(a => a.menuOrder)
                                            }).OrderByDescending(c => c.menuOrder);
                    ump.UserMenuPackages = UserMenuPackages.ToList();


                }
            }

            return View(ump);
        }
        public async Task<ActionResult> ChangePassword(ManageUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var um=new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var result = await um.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var json = new
                    {
                        okMsg = "用户密码修改成功"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var json = new
                    {
                        okMsg = "用户密码修改失败"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var json = new
                {
                    okMsg = "输入信息格式有误"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult StartPage()
        {
            ViewBag.ItServiceCompleted = bdb.WorkFlowItems.Where(i=>i.Wfstatus=="已结束").Count();
            ViewBag.ItServiceUnCompleted = bdb.WorkFlowItems.Where(i => i.Wfstatus != "已结束").Count();
            ViewBag.ItServiceAll = bdb.WorkFlowItems.Count();
            return View();
        }
          public JObject StartPageToDoList()
        {
            string username=HttpContext.Session["UserName"].ToString().Trim();
            List<WorkFlowItem> list = bdb.WorkFlowItems.OrderByDescending(c => c.WfCreateDate).Where(i => i.WfCurrentUser.Trim() == username).ToList();

            JObject json = new JObject(
                new JProperty("total", bdb.WorkFlowItems.ToList().Count()),
                new JProperty("rows",
                    new JArray(
                        from r in list
                        select new JObject(
                            new JProperty("ID", r.ID),
                            new JProperty("WfInstanceId", r.WfInstanceId),
                            new JProperty("WfType", r.WfType),
                            new JProperty("WfBussinessUrl", r.WfBussinessUrl),
                            new JProperty("WfTitle", GetWfName(r.WfBusinessId)),
                            new JProperty("WfCurrentUser", r.WfCurrentUser),
                            new JProperty("Wfstatus", r.Wfstatus),
                            new JProperty("WfCreateDate", r.WfCreateDate.ToString("yyyy/MM/dd hh:mm:ss"))))));

            return json;
        }
        public string GetWfName(int bid)
        {
            ItServiceItem isi = bdb.ItServiceItems.Where(i => i.ID == bid).Single();
            //ItServiceItem isi = bdb.ItServiceItems.Where(i => i.ID == bid).FirstOrDefault();
            return isi.Title;
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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