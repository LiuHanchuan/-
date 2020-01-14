using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Apache.Models;
using Apache.ViewModels;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.Entity;
using Apache.Filters;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Transactions;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Apache.Controllers
{
    [ApachAuthentication]
    [Logging]
    public class AccountController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())), new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext())))
        {
        }
        
        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;

        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public RoleManager<ApplicationRole> RoleManager { get; private set; }


        [AllowAnonymous]
        public ActionResult users()
        {
            return View();
        }
        public ActionResult roles()
        {
            return View();
        }

        public string orgTree()
        {

            List<Organization> list = db.Organizations.ToList();
            JArray json = new JArray(
                from r in list
                select new JObject(
                    new JProperty("id", r.ID),
                    new JProperty("parentId", r.parentId),
                    new JProperty("name", r.orgShortName)
                    ));


            return json.ToString();
        }

        public string menuTree()        //列出系统的所有功能模块
        {

            List<Menu> list = db.Menus.ToList();
            //JArray json = new JArray(
            //    from r in list
            //    select new JObject(
            //        new JProperty("id", r.ID),
            //        new JProperty("_parentId", r.parentId),
            //        new JProperty("name", r.orgName)
            //        ));
       

                JArray json=  new JArray(
                       from r in list
                       select new JObject(
                           new JProperty("id", r.ID),
                           new JProperty("name", r.menuName),
                           new JProperty("menuType", r.menuType),
                           new JProperty("menuController", r.menuController),
                           new JProperty("menuAction", r.menuAction),
                           new JProperty("menuOrder", r.menuOrder),
                           new JProperty("menuNote", r.menuNote),
                           new JProperty("parentId", r.parentId)));


            return json.ToString();
        }

        public async Task<ActionResult> CreateRole(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole(roleViewModel.Name);

                // Save the new Description property:
                role.Description = roleViewModel.Description;
                role.CreateRoleTime = DateTime.Now;
                var roleresult = await RoleManager.CreateAsync(role);
                if (roleresult.Succeeded)
                {
                    var json = new
                    {
                        okMsg = "角色创建成功"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var json = new
                    {
                        okMsg = "角色创建失败"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var json = new
                {
                    okMsg = "数据输入有误"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }


        }
        public async Task<ActionResult> EditRole(RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                if (role != null)
                {
                    role.Name = roleModel.Name;

                    // Update the new Description property:
                    role.Description = roleModel.Description;
                    var roleresult = await RoleManager.UpdateAsync(role);
                    if (roleresult.Succeeded)
                    {
                        var json = new
                        {
                            okMsg = "角色修改成功"
                        };

                        return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var json = new
                        {
                            okMsg = "角色修改失败"
                        };

                        return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var json = new
                    {
                        okMsg = "未找到角色"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var json = new
                {
                    okMsg = "数据输入有误"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }


        }
        public async Task<ActionResult> DelRole(string id)
        {
            if (id != null)
            {
                var role = await RoleManager.FindByIdAsync(id);
                if (role != null)
                {
                    var roleresult = await RoleManager.DeleteAsync(role);
                    if (roleresult.Succeeded)
                    {
                        var json = new
                        {
                            okMsg = "角色删除成功"
                        };

                        return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var json = new
                        {
                            okMsg = "角色删除失败"
                        };

                        return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var json = new
                    {
                        okMsg = "未找到角色"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                var json = new
                {
                    okMsg = "数据输入有误"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }


        }

        public async Task<ActionResult> UsersToRole_old_version(string user_ids, string roleid)
        {
            string rolename = "";
            string[] ids = user_ids.Split(',');
            HashSet<string> selectedUserHS = new HashSet<string>(ids);
            //var roleUsers = new HashSet<string>(RoleManager.Roles.Where(i => i.Id == roleid).Include(i => i.Users).Single().Users.Select(c => c.UserId));
            List<ApplicationRole> role = RoleManager.Roles.Where(i => i.Id == roleid).ToList();
            foreach (var userRole in role)
            {
                rolename = userRole.Name;
            }
            List<ApplicationUser> user = UserManager.Users.ToList();


            if (user_ids == "")
            {
                foreach (ApplicationUser u in user)
                {
                    if (await UserManager.IsInRoleAsync(u.Id, rolename))
                    {
                        var result = await UserManager.RemoveFromRoleAsync(u.Id, rolename);
                    }
                }
                var json = new
                {
                    okMsg = "清空角色用户完成"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);

            }
            else
            {
                foreach (ApplicationUser u in user)
                {
                    if (selectedUserHS.Contains(u.Id))
                    {
                        if (!await UserManager.IsInRoleAsync(u.Id, rolename))
                        {
                            var result = await UserManager.AddToRoleAsync(u.Id, rolename);

                        }
                    }
                    else
                    {
                        if (await UserManager.IsInRoleAsync(u.Id, rolename))
                        {
                            var result = await UserManager.RemoveFromRoleAsync(u.Id, rolename);
                        }
                    }
                }
                var json = new
                {
                    okMsg = "设置角色用户完成"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }


        }
        public ActionResult UsersToRole(string user_ids, string roleid)
        {
            string[] ids = user_ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            using (ApplicationDbContext entity = new ApplicationDbContext())
            {

                string sql = "Delete FROM  [AspNetUserRoles] where RoleId = @ID";
                var args = new DbParameter[] { 
                               new SqlParameter { ParameterName = "ID", Value = roleid} 
                        };
                entity.Database.ExecuteSqlCommand(sql, args);


                DataTable dt = new DataTable();
                dt.Columns.Add("UserId", typeof(string));
                dt.Columns.Add("RoleId", typeof(string));


                foreach (var id_item in ids)
                {
                    DataRow dr = dt.NewRow();
                    dr["UserId"] = id_item.ToString();
                    dr["RoleId"] = roleid.ToString();
                    dt.Rows.Add(dr);
                }
                if (InsertTable(dt, "AspNetUserRoles", dt.Columns))
                {
                    var json = new
                    {
                        okMsg = "设置角色用户成功"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var json = new
                    {
                        okMsg = "设置角色用户失败"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
            }

        }

        /// <summary>批量导入DataTable  
        /// 批量导入DataTable  
        /// </summary>  
        /// <param name="dt">DataTable数据表</param>  
        /// <param name="tableName">表名</param>  
        /// <param name="dtColum">数据列集合</param>  
        /// <return>Boolean值:true成功，false失败</return>  
        public static Boolean InsertTable(DataTable dt, string tableName, DataColumnCollection dtColum)
        {
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope(TransactionScopeOption.Required))
            {

                using (SqlBulkCopy sqlBC = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["ApacheConnection"].ConnectionString, SqlBulkCopyOptions.KeepIdentity))
                {
                    sqlBC.BatchSize = 1000;
                    sqlBC.BulkCopyTimeout = 30;
                    sqlBC.DestinationTableName = tableName;
                    for (int i = 0; i < dtColum.Count; i++)
                    {
                        sqlBC.ColumnMappings.Add(dtColum[i].ColumnName, dtColum[i].ColumnName);
                    }
                    try
                    {
                        //批量写入  
                        sqlBC.WriteToServer(dt);
                        scope.Complete();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }  
       
        public ActionResult MenusToRole(string menu_ids, string roleid)
        {
            //string rolename = "";
            //string[] ids = menu_ids.Split(',');
            //HashSet<string> selectedMenuHS = new HashSet<string>(ids);
            //var roleUsers = new HashSet<string>(RoleManager.Roles.Where(i => i.Id == roleid).Include(i => i.Users).Single().Users.Select(c => c.UserId));
           // List<ApplicationRole> role = RoleManager.Roles.Where(i => i.Id == roleid).ToList();
            //var role = db.Roles.Include().Where(i => i.Id == roleid).Single();
            
          // var u = db.Users.Include(i => i.Organization).Where(i => i.UserName == "limin").Single();   //多对多操作
           // var k = db.Roles.Include(i => i.Menus).Where(i => i.Id == roleid).Single();
            //var c=db.Roles.Include()
            //foreach (var course in db.Courses)
            //{

            //    u.Courses.Remove(course);
            //}
            //db.SaveChanges();
            //var rolecc = db.Roles.Include(i => i.Menus).Where(i => i.Id == roleid).Single();
            //var role=RoleManager.Roles.Include(i=>i.Menus).Where(i => i.Id == roleid).Single();
            string[] ids = menu_ids.Split(',');
            HashSet<string> selectedMenuHS = new HashSet<string>(ids);
            //var role = RoleManager.Roles.Include(i => i.Menus).Where(i => i.Id == roleid).Single();
            var role = db.Roles.Include(i => i.Menus).Where(i => i.Id == roleid).Single();
            var menus = db.Menus.ToList();
            var MenusInRole = new HashSet<int> (role.Menus.Select(c => c.ID));
            if (menu_ids == "")
            {
                role.Menus = new List<Menu>();
                db.SaveChanges();

                var json = new
                {
                    okMsg = "清空角色菜单完成"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);

            }
            else
            {
                foreach (Menu m in menus)
                {
                    if (selectedMenuHS.Contains(m.ID.ToString()))
                    {
                        if (!MenusInRole.Contains(m.ID))
                        {
                            role.Menus.Add(m);

                        }
                    }
                    else
                    {
                        if (MenusInRole.Contains(m.ID))
                        {
                            role.Menus.Remove(m);
                        }
                    }
                }
                db.SaveChanges();
                var json = new
                {
                    okMsg = "设置角色菜单完成"
                };


                return Json(json, "text/html", JsonRequestBehavior.AllowGet);

            }
            

        }
 

        public JObject MenuToRoleList(string roleid)       //角色列表
        {
            var MenusInRole = db.Roles.Include(i => i.Menus).Where(i => i.Id == roleid).Single().Menus.ToList();
   

            JObject json = new JObject(
                new JProperty("rows",
                    new JArray(
                        from r in MenusInRole
                        select new JObject(

                            new JProperty("ID", r.ID)))));


            return json;
        }

  
        public JObject RoleList(int? page, int? rows)       //角色列表
        {
            int totalCount;
            var list = RoleManager.Roles.OrderByDescending(c => c.CreateRoleTime).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();

            totalCount = RoleManager.Roles.ToList().Count();
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
                new JProperty("total", totalCount),
                new JProperty("rows",
                    new JArray(
                        from r in list
                        select new JObject(
                            new JProperty("ID", r.Id),
                            new JProperty("Name", r.Name),
                            new JProperty("Description", r.Description)))));



            return json;
        }

        public JObject LibList()       //专病库列表
        {
            var list = db.Libraries.ToList();
            int totalCount = list.Count();
            JObject json = new JObject(
                new JProperty("total", totalCount),
                new JProperty("rows",
                    new JArray(
                        from r in list
                        select new JObject(
                            new JProperty("ID", r.LibraryID),
                            new JProperty("Name", r.LibraryName)
                            ))));
            return json;
        }
        public async Task<ActionResult> UserLibList(string userid)       //用户拥有的专病库列表
        {
            ApplicationUser user = db.Users.Include(i => i.Roles).FirstOrDefault(i => i.Id == userid);
            if (string.IsNullOrEmpty(user.AllowLibID))
            {
                return Json(null, "text/html", JsonRequestBehavior.AllowGet);
            }
            var allowlibid = user.AllowLibID.Trim().Split(',').ToList();
            var userlist = db.Libraries.ToList().FindAll(p => allowlibid.Exists(q => q == p.LibraryID.ToString()));

            var json = new
            {
                rows = (from r in userlist
                        select new
                        {
                            ID = r.LibraryID,
                            Name = r.LibraryName
                        }).ToArray()
            };

            return Json(json, "text/html", JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> UsersOfLib(string userid, string Libs)    
        {
            ApplicationUser user = db.Users.Include(i => i.Roles).FirstOrDefault(i => i.Id == userid);
            user.AllowLibID = Libs;
            db.Entry(user).State = EntityState.Modified;
            try
            {
                db.SaveChanges();

                if (string.IsNullOrEmpty(Libs))
                {
                    var json = new
                    {
                        okMsg = "清空用户专病库权限完成"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var json = new
                    {
                        okMsg = "设置角色专病库权限完成"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
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

        public ActionResult RoleUserList(string roleid)       //角色包含的用户列表
        {

            var role = db.Roles.Include(x => x.Users).Where(i => i.Id == roleid).SingleOrDefault();

            var usersid = role.Users.Select(i => i.UserId).ToList();
            var users = db.Users.Where(i => usersid.Contains(i.Id));
            var json = new
            {
                rows = (from r in users

                        select new
                        {
                            ID = r.Id,
                            TrueName = r.TrueName

                        }).ToArray()
            };

            return Json(json, "text/html", JsonRequestBehavior.AllowGet);


        }


        public async Task<ActionResult> UserRoleList(string userid)       //用户拥有的角色列表
        {

            var roles_string = await UserManager.GetRolesAsync(userid);
            var roles = new List<ApplicationRole>();

            foreach (var role in roles_string)
            {
                roles.Add(await RoleManager.FindByNameAsync(role));
            }

            var json = new
            {
                rows = (from r in roles
                        select new
                        {
                            ID = r.Id,
                            Name = r.Name
                        }).ToArray()
            };

            return Json(json, "text/html", JsonRequestBehavior.AllowGet);


        }
        /// <summary>
        /// 为用户添加角色
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task<ActionResult> UsersOfRole(string userid, string roles)         //添加用户到角色
        {


            if (roles == "")
            {
                var allroles = RoleManager.Roles;
                foreach (var r in allroles)
                {
                    if (await UserManager.IsInRoleAsync(userid, r.Name))
                    {
                        var result = await UserManager.RemoveFromRoleAsync(userid, r.Name);
                    }
                }
                var json = new
                {
                    okMsg = "清空用户角色完成"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var oldroles = await UserManager.GetRolesAsync(userid);
                string[] rolesname = roles.Split(',');
                var roles_e = oldroles.Except(rolesname).ToArray<string>();
                await UserManager.RemoveFromRolesAsync(userid, roles_e);

                foreach (var r in rolesname)
                {
                    if (!await UserManager.IsInRoleAsync(userid, r))
                    {
                        var result = await UserManager.AddToRoleAsync(userid, r);
                    }
                }
                var json = new
                {
                    okMsg = "设置角色用户完成"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }


        }




        public JObject UserList(int? page, int? rows, int? orgid, string username, string truename)
        {
            //var org = db.Organizations.Include(x => x.ApplicationUsers);  //实体操作ApplicationUsers
            //foreach (Organization o in org)
            //{
            //    foreach (ApplicationUser u in o.ApplicationUsers)
            //    {
            //        ViewBag.u += u.UserName;
            //    }
            //}

            //var org = db.Organizations.ToList();                         //lazy loding
            //foreach (Organization o in org)
            //{
            //    //db.Entry(o).Collection(x => x.ApplicationUsers).Load();
            //    foreach (ApplicationUser u in o.ApplicationUsers)
            //    {
            //        ViewBag.u += u.UserName;
            //    }
            //}

            //var u = db.Users.Include(i => i.Courses).Where(i => i.UserName == "xxxxx").Single();   //多对多操作

            //foreach (var course in db.Courses)
            //{

            //    u.Courses.Remove(course);
            //}
            //db.SaveChanges();

            int totalCount;
            var query = db.Users.AsQueryable();
            List<ApplicationUser> list;



            if (orgid == null)
            {
                totalCount = db.Users.ToList().Count();
            }
            else
            {
                query = query.Where(i => i.OrganizationID == orgid);
                totalCount = db.Users.Where(i => i.OrganizationID == orgid).ToList().Count();
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                query = query.Where(i => i.UserName.Contains(username));
                totalCount = query.ToList().Count();
            }

            if (!string.IsNullOrWhiteSpace(truename))
            {
                query = query.Where(w => w.TrueName.Contains(truename));
                totalCount = query.ToList().Count();
            }
      
            list = query.OrderByDescending(c => c.Order).ThenByDescending(c=>c.UserName).Skip((Convert.ToInt32(page) - 1) * Convert.ToInt32(rows)).Take(Convert.ToInt32(rows)).ToList();


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
                new JProperty("total", totalCount),
                new JProperty("rows",
                    new JArray(
                        from r in list
                        select new JObject(
                            new JProperty("ID", r.Id),
                            new JProperty("UserName", r.UserName),
                            new JProperty("TrueName", r.TrueName),
                            new JProperty("Order", r.Order),
                            new JProperty("OrganizationID", r.OrganizationID),
                            new JProperty("OrgName", GetOrgName(r.OrganizationID)),
                            new JProperty("Job", r.Job),
                            new JProperty("Mobile", r.Mobile),
                            new JProperty("AllowLibID", r.AllowLibID),
                            new JProperty("PhoneNumber", r.PhoneNumber),
                            new JProperty("Roles", GetUsersTrueName(r.Id)),
                            new JProperty("Email", r.Email)))));



            return json;
        }

        public JObject UserListForRoleManage(int? orgid)
        {
          

            int totalCount;
            var query = db.Users.AsQueryable();
            List<ApplicationUser> list;



            if (orgid == null)
            {
                totalCount = db.Users.ToList().Count();
            }
            else
            {
                query = query.Where(i => i.OrganizationID == orgid);
                totalCount = db.Users.Where(i => i.OrganizationID == orgid).ToList().Count();
            }
            list = query.OrderByDescending(c => c.Order).ToList();
         
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
                new JProperty("total", totalCount),
                new JProperty("rows",
                    new JArray(
                        from r in list
                        select new JObject(
                            new JProperty("ID", r.Id),
                            new JProperty("TrueName", r.TrueName)
                          ))));



            return json;
        }

        public string GetOrgName(int orgid)
        {
            Organization org = db.Organizations.Where(i => i.ID == orgid).Single();

            return org.orgName;
        }
        public string GetUsersTrueName(string userid)
        {

            string RNames = "";
            var roles = new List<ApplicationRole>();


            foreach (var role in RoleManager.Roles.ToList())
            {
                if (UserManager.IsInRole(userid, role.Name))
                {
                    RNames += role.Name +",";
                }
            }
            return RNames.TrimEnd(',');
        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [AllowAnonymous]
        public ActionResult NotAuthentication() 
        {
            var json = new
            {
                errorMsg = "当前用户没有此项操作权限"
            };

            return Json(json, "text/html", JsonRequestBehavior.AllowGet);
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    //以下代码将权限保存到Session
                    //string currentUserId = User.Identity.GetUserId();      //此处无法获取到id,除非再次对服务器请求
                    ApplicationUser current_user = db.Users.Include(i => i.Roles).FirstOrDefault(i => i.UserName == model.UserName);
                    HttpContext.Session["UserName"] = current_user.UserName;
                    var user_roles = current_user.Roles.ToList();
                    var rolesid = user_roles.Select(c => c.RoleId).ToList();
                    var role = db.Roles.Include(i => i.Menus).Where(i => rolesid.Contains(i.Id)).ToList();
                    var uml = new UserMenuList();
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
                    HttpContext.Session["uml"] = uml;                    //将当前用户的权限保存到session
                    //保存权限结束
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    //ModelState.AddModelError("", "用户名或密码错误。（初始默认密码：abc123）");
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
 

        public async Task<ActionResult> CheckUserName(RegisterViewModel model)
        {

            if (await UserManager.FindByNameAsync(model.UserName) == null)
            {
                var json = new
                {
                    okMsg = "可以注册"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var json = new
                {
                    okMsg = "已被注册"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);

            }
        }

        //
        // POST: /Account/Register
        [HttpPost]


        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                user.TrueName = model.TrueName;
                //user.Order = model.Order;
                user.Order = 1;
                user.Job = model.Job;
                user.Mobile = model.Mobile;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.OrganizationID = model.OrganizationID;
                //user.PW = model.Password;
                var md5 = new MD5CryptoServiceProvider();
                string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(model.Password + DateTime.Now.ToString())), 4, 8);
                user.PW = t2.Replace("-", "");
                
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // await SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("Index", "Home");
                    var json = new
                    {
                        okMsg = "用户注册成功"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var json = new
                    {
                        okMsg = "用户注册失败"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var json = new
                {
                    okMsg = "数据输入有误"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);

            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            //return View(model);

        }

        [HttpPost]

  
        public async Task<ActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user = await UserManager.FindByIdAsync(model.Id);
                user.TrueName = model.TrueName;
                user.Order = model.Order;
                user.Job = model.Job;
                user.Mobile = model.Mobile;
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.OrganizationID = model.OrganizationID;

                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    //await SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("Index", "Home");
                    var json = new
                    {
                        okMsg = "用户修改成功"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var json = new
                    {
                        okMsg = "用户修改失败"
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
        public async Task<ActionResult> DelUser(string id)
        {

            if (id == User.Identity.GetUserId())
            {
                var json = new
                {
                    errorMsg = "不可以删除自己"
                };

                return Json(json, "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                ApplicationUser user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    var json = new
                    {
                        errorMsg = "没有此用户"
                    };

                    return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var results = await UserManager.DeleteAsync(user);


                    // If successful
                    if (results.Succeeded)
                    {
                        var json = new
                        {
                            okMsg = "用户删除成功"
                        };

                        return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var json = new
                        {
                            errorMsg = "用户删除失败"
                        };

                        return Json(json, "text/html", JsonRequestBehavior.AllowGet);
                    }
                }

            }
        }

      
   

          public async Task<ActionResult> ResetPassword(string id)
          {
              await UserManager.RemovePasswordAsync(id);
              var result = UserManager.AddPassword(id, "abc123");
              if (result.Succeeded)
              {
                  var json = new
                  {
                      okMsg = "用户密码重置成功（默认abc123）"
                  };

                  return Json(json, "text/html", JsonRequestBehavior.AllowGet);
              }
              else
              {
                  var json = new
                  {
                      okMsg = "用户密码重置失败"
                  };

                  return Json(json, "text/html", JsonRequestBehavior.AllowGet);
              }
          }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "你的密码已更改。"
                : message == ManageMessageId.SetPasswordSuccess ? "已设置你的密码。"
                : message == ManageMessageId.RemoveLoginSuccess ? "已删除外部登录名。"
                : message == ManageMessageId.Error ? "出现错误。"
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
       // [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 请求重定向到外部登录提供程序
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        //  [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        //  [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // 从外部登录提供程序获取有关用户的信息
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/ExternalLoginFailure
        //  [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null && RoleManager !=null)
            {
                UserManager.Dispose();
                UserManager = null;
                RoleManager.Dispose();
                RoleManager = null;
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 帮助程序
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion






        public object x { get; set; }
    }
}