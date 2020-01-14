namespace Apache.ApplicationDbContexMigration
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Collections.Generic;
    using Apache.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Web.Mvc;


    internal sealed class Configuration : DbMigrationsConfiguration<Apache.Models.ApplicationDbContext>
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public Configuration()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())), new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext())))
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"ApplicationDbContexMigration";
        }

        public Configuration(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;

        }
        public UserManager<ApplicationUser> UserManager { get; private set; }

        public RoleManager<ApplicationRole> RoleManager { get; private set; }

        protected override void Seed(Apache.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            #region         增加组织机构根目录、字典根目录
            var Organizations = new List<Organization>                    //初始化组织机构

            {
                new Organization {orgName = "您的组织机构全程",orgShortName="您的组织机构简称",orgCode="",orgType="根组织",orgNote=""}
            };
            Organizations.ForEach(s => context.Organizations.AddOrUpdate(p => p.orgName, s));
            context.SaveChanges();

            var DataDicts = new List<DataDict>                    //初始化数据字典

            {
                new DataDict {DataDictName = "根字典", DataDictCode="",DataDictType="根数据字典",DataDictNote=""}
            };
            DataDicts.ForEach(s => context.DataDicts.AddOrUpdate(p => p.DataDictName, s));
            context.SaveChanges();

            var rootDictList = db.DataDicts.Where(i => i.DataDictName == "根字典").FirstOrDefault(); //在根字典下增加服务类别字典 
            if (rootDictList != null)                                                                  
            {
                var catalogDict = new List<DataDict>
                {
				   new DataDict {DataDictName = "IT服务类别", DataDictCode="",DataDictType="",DataDictNote="",parentId=rootDictList.ID}
			    };
                catalogDict.ForEach(s => context.DataDicts.AddOrUpdate(p => p.DataDictName, s));
                context.SaveChanges();
            }
            #endregion

            #region         增加管理员账号
            if (UserManager.FindByName("admin") == null)       //初始化admin
            {
                var org = db.Organizations.Where(i => i.orgType == "根组织").FirstOrDefault();
                if (org != null)
                {
                    var user = new ApplicationUser() { UserName = "admin" };
                    user.TrueName = "管理员";
                    user.Order = 1000;
                    user.OrganizationID = org.ID;
                    var result = UserManager.Create(user, "admin888");
                }
            }
            #endregion

            #region         增加目录--》菜单--》功能
            var rootMenu = new List<Menu>                                            //增加根模块

            {
                new Menu {menuName = "根模块", menuType="根模块",menuOrder=0,parentId=0,isSysMenu="1"}
            };
            rootMenu.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
            context.SaveChanges();
            var rootMenuList = db.Menus.Where(i => i.menuName == "根模块").FirstOrDefault();
            if (rootMenuList != null)                                                            //增加系统管理目录模块       
            {
                var catalogMenus = new List<Menu>
                {
				   new Menu {menuName = "系统管理", menuType="模块目录",menuOrder=1,isMenu="是",menuIcon="icon-sys-manage",parentId=rootMenuList.ID,isSysMenu="1"}
			    };
                catalogMenus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }


            var catalogMenuList = db.Menus.Where(i => i.menuName == "系统管理").FirstOrDefault();
            if (catalogMenuList != null)                                                            //增加系统管理模块       
            {
                var Menus = new List<Menu>
                {
				   new Menu {menuName = "组织机构管理", menuType="模块",menuController="Organization",menuAction="Index",menuOrder=10000,menuIcon="icon-sys-organization",parentId=catalogMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "用户管理", menuType="模块",menuController="Account",menuAction="Users",menuOrder=9000,menuIcon="icon-sys-user",parentId=catalogMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "模块管理", menuType="模块",menuController="Menu",menuAction="Index",menuOrder=8000,menuIcon="icon-sys-model",parentId=catalogMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "角色管理", menuType="模块",menuController="Account",menuAction="Roles",menuOrder=7000,menuIcon="icon-sys-role",parentId=catalogMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "日志管理", menuType="模块",menuController="Logging",menuAction="Index",menuOrder=6000,menuIcon="icon-sys-log",parentId=catalogMenuList.ID,isSysMenu="1"}, 
                   new Menu {menuName = "数据字典管理", menuType="模块",menuController="DataDict",menuAction="Index",menuOrder=5000,menuIcon="icon-sys-datadict",parentId=catalogMenuList.ID,isSysMenu="1"}
			    };
                Menus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }

            var orgMenuList = db.Menus.Where(i => i.menuName == "组织机构管理").FirstOrDefault();                   //增加组织机构管理系统功能模块   
            if (orgMenuList != null)
            {
                var Menus = new List<Menu>
                {
				   new Menu {menuName = "添加组织机构", menuType="功能",menuController="Organization",menuAction="CreateOrg",menuOrder=1000,menuIcon="",parentId=orgMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "修改组织机构", menuType="功能",menuController="Organization",menuAction="EditOrg",menuOrder=1000,menuIcon="",parentId=orgMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "删除组织机构", menuType="功能",menuController="Organization",menuAction="DelOrg",menuOrder=1000,menuIcon="",parentId=orgMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "显示组织机构列表", menuType="功能",menuController="Organization",menuAction="orgList",menuOrder=1000,menuIcon="",parentId=orgMenuList.ID,isSysMenu="1"}

			    };
                Menus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }


            var datadictMenuList = db.Menus.Where(i => i.menuName == "数据字典管理").FirstOrDefault();                   //增加数据字典管理系统功能模块   
            if (datadictMenuList != null)
            {
                var Menus = new List<Menu>
                {
				   new Menu {menuName = "添加数据字典", menuType="功能",menuController="DataDict",menuAction="CreateDataDict",menuOrder=1000,menuIcon="",parentId=datadictMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "修改数据字典", menuType="功能",menuController="DataDict",menuAction="EditDataDict",menuOrder=1000,menuIcon="",parentId=datadictMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "删除数据字典", menuType="功能",menuController="DataDict",menuAction="DelDataDict",menuOrder=1000,menuIcon="",parentId=datadictMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "显示数据字典列表", menuType="功能",menuController="DataDict",menuAction="DataDictList",menuOrder=1000,menuIcon="",parentId=datadictMenuList.ID,isSysMenu="1"}

			    };
                Menus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }



            var userMenuList = db.Menus.Where(i => i.menuName == "用户管理").FirstOrDefault();                   //增加用户管理系统功能模块   
            if (userMenuList != null)
            {
                var Menus = new List<Menu>
                {
				   new Menu {menuName = "添加用户", menuType="功能",menuController="Account",menuAction="Register",menuOrder=1000,menuIcon="",parentId=userMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "修改用户", menuType="功能",menuController="Account",menuAction="EditUser",menuOrder=1000,menuIcon="",parentId=userMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "删除用户", menuType="功能",menuController="Account",menuAction="DelUser",menuOrder=1000,menuIcon="",parentId=userMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "重置用户密码", menuType="功能",menuController="Account",menuAction="ResetPassword",menuOrder=1000,menuIcon="",parentId=userMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "显示人员列表", menuType="功能",menuController="Account",menuAction="UserList",menuOrder=1000,menuIcon="",parentId=userMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "左侧显示组织机构列表", menuType="功能",menuController="Account",menuAction="orgTree",menuOrder=1000,menuIcon="",parentId=userMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "设置用户角色", menuType="功能",menuController="Account",menuAction="UsersOfRole",menuOrder=1000,menuIcon="",parentId=userMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "显示所有角色列表", menuType="功能",menuController="Account",menuAction="RoleList",menuOrder=1000,menuIcon="",parentId=userMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "显示用户拥有的角色列表", menuType="功能",menuController="Account",menuAction="UserRoleList",menuOrder=1000,menuIcon="",parentId=userMenuList.ID,isSysMenu="1"}

			    };
                Menus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }

            var memuMenuList = db.Menus.Where(i => i.menuName == "模块管理").FirstOrDefault();                   //增加模块管理系统功能模块   
            if (memuMenuList != null)
            {
                var Menus = new List<Menu>
                {
                   new Menu {menuName = "添加模块", menuType="功能",menuController="Menu",menuAction="CreateMenu",menuOrder=1000,menuIcon="",parentId=memuMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "修改模块", menuType="功能",menuController="Menu",menuAction="EditMenu",menuOrder=1000,menuIcon="",parentId=memuMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "删除模块", menuType="功能",menuController="Menu",menuAction="DelMenu",menuOrder=1000,menuIcon="",parentId=memuMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "显示模块列表", menuType="功能",menuController="Menu",menuAction="menuList",menuOrder=1000,menuIcon="",parentId=memuMenuList.ID,isSysMenu="1"}

			    };
                Menus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }

            var roleMenuList = db.Menus.Where(i => i.menuName == "角色管理").FirstOrDefault();                   //增加模块管理系统功能模块   
            if (roleMenuList != null)
            {
                var Menus = new List<Menu>
                {
                   new Menu {menuName = "添加角色", menuType="功能",menuController="Account",menuAction="CreateRole",menuOrder=1000,menuIcon="",parentId=roleMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "修改角色", menuType="功能",menuController="Account",menuAction="EditRole",menuOrder=1000,menuIcon="",parentId=roleMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "删除角色", menuType="功能",menuController="Account",menuAction="DelRole",menuOrder=1000,menuIcon="",parentId=roleMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "显示角色列表", menuType="功能",menuController="Account",menuAction="RoleList",menuOrder=1000,menuIcon="",parentId=roleMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "显示系统组织机构", menuType="功能",menuController="Account",menuAction="orgTree",menuOrder=1000,menuIcon="",parentId=roleMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "显示系统所有用户", menuType="功能",menuController="Account",menuAction="UserListForRoleManage",menuOrder=1000,menuIcon="",parentId=roleMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "显示角色拥有的用户", menuType="功能",menuController="Account",menuAction="RoleUserList",menuOrder=1000,menuIcon="",parentId=roleMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "显示系统的所有功能模块", menuType="功能",menuController="Account",menuAction="menuTree",menuOrder=1000,menuIcon="",parentId=roleMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "显示角色拥有的功能模块", menuType="功能",menuController="Account",menuAction="MenuToRoleList",menuOrder=1000,menuIcon="",parentId=roleMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "保存用户到角色", menuType="功能",menuController="Account",menuAction="UsersToRole",menuOrder=1000,menuIcon="",parentId=roleMenuList.ID,isSysMenu="1"},
                   new Menu {menuName = "保存模块到角色", menuType="功能",menuController="Account",menuAction="MenusToRole",menuOrder=1000,menuIcon="",parentId=roleMenuList.ID,isSysMenu="1"}

			    };
                Menus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }


            var logMenuList = db.Menus.Where(i => i.menuName == "日志管理").FirstOrDefault();                   //增加组织机构管理系统功能模块   
            if (orgMenuList != null)
            {
                var Menus = new List<Menu>
                {
				   new Menu {menuName = "查看日志列表", menuType="功能",menuController="Logging",menuAction="LogList",menuOrder=1000,menuIcon="",parentId=logMenuList.ID,isSysMenu="1"}

			    };
                Menus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }
            #endregion

            #region         增加业务目录--》业务菜单--》业务功能，此处为IT服务管理 (注：新增加业务功能模块后，可在此初始化数据库，也可以在系统界面中的模块管理中进行增加)
            var bRootMenuList = db.Menus.Where(i => i.menuName == "根模块").FirstOrDefault();
            if (bRootMenuList != null)                                                            //增加IT服务目录模块       
            {
                var catalogMenus = new List<Menu>
                {
				   new Menu {menuName = "IT服务", menuType="模块目录",menuOrder=9,isMenu="是",menuIcon="icon-it-services",parentId=bRootMenuList.ID,isSysMenu="0"}
			    };
                catalogMenus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }


            var bCatalogMenuList = db.Menus.Where(i => i.menuName == "IT服务").FirstOrDefault();
            if (bCatalogMenuList != null)                                                            //增加IT服务菜单模块       
            {
                var Menus = new List<Menu>
                {
				   new Menu {menuName = "申请服务", menuType="模块",menuController="ItService",menuAction="Create",menuOrder=10000,menuIcon="icon-it-create",parentId=bCatalogMenuList.ID,isSysMenu="0"},
                   new Menu {menuName = "我的申请", menuType="模块",menuController="ItService",menuAction="MyWorkFlow",menuOrder=9000,menuIcon="icon-it-services-list",parentId=bCatalogMenuList.ID,isSysMenu="0"},
                   new Menu {menuName = "申请管理", menuType="模块",menuController="ItService",menuAction="WorkFlowManage",menuOrder=8000,menuIcon="icon_wf_manage",parentId=bCatalogMenuList.ID,isSysMenu="0"},
                   new Menu {menuName = "统计分析", menuType="模块",menuController="ItService",menuAction="Statistic",menuOrder=7000,menuIcon="icon_isi_chart",parentId=bCatalogMenuList.ID,isSysMenu="0"}
			    };
                Menus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }

            var bCreateWfMenuList = db.Menus.Where(i => i.menuName == "申请服务").FirstOrDefault();                   //增加组织机构管理系统功能模块   
            if (bCreateWfMenuList != null)
            {
                var Menus = new List<Menu>
                {
				   new Menu {menuName = "起草者创建流程", menuType="功能",menuController="ItService",menuAction="CreateWorkFlow",menuOrder=1000,menuIcon="",parentId=bCreateWfMenuList.ID,isSysMenu="0"},
                   new Menu {menuName = "起草者提交下一环节", menuType="功能",menuController="ItService",menuAction="DrafterToNextState",menuOrder=1000,menuIcon="",parentId=bCreateWfMenuList.ID,isSysMenu="0"}
			    };
                Menus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }

            var bMyWfMenuList = db.Menus.Where(i => i.menuName == "我的申请").FirstOrDefault();                   //增加组织机构管理系统功能模块   
            if (bMyWfMenuList != null)
            {
                var Menus = new List<Menu>
                {
				   new Menu {menuName = "用户申请单列表", menuType="功能",menuController="ItService",menuAction="MyWorkFlowList",menuOrder=1000,menuIcon="",parentId=bMyWfMenuList.ID,isSysMenu="0"},
                   new Menu {menuName = "用户查看自己的申请单", menuType="功能",menuController="ItService",menuAction="ViewWorkFlow",menuOrder=1000,menuIcon="",parentId=bMyWfMenuList.ID,isSysMenu="0"}
			    };
                Menus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }

            var bManageWfMenuList = db.Menus.Where(i => i.menuName == "申请管理").FirstOrDefault();                   //增加组织机构管理系统功能模块   
            if (bManageWfMenuList != null)
            {
                var Menus = new List<Menu>
                {
				   new Menu {menuName = "显示所有的申请列表", menuType="功能",menuController="ItService",menuAction="AllWorkFlowList",menuOrder=1000,menuIcon="",parentId=bManageWfMenuList.ID,isSysMenu="0"},
                   new Menu {menuName = "查看申请单详细信息", menuType="功能",menuController="ItService",menuAction="ViewWorkFlow",menuOrder=1000,menuIcon="",parentId=bManageWfMenuList.ID,isSysMenu="0"},
                   new Menu {menuName = "删除申请及业务数据", menuType="功能",menuController="ItService",menuAction="DelWorkFlow",menuOrder=1000,menuIcon="",parentId=bManageWfMenuList.ID,isSysMenu="0"}
			    };
                Menus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }

            var bStatisticMenuList = db.Menus.Where(i => i.menuName == "统计分析").FirstOrDefault();                   //增加组织机构管理系统功能模块   
            if (bStatisticMenuList != null)
            {
                var Menus = new List<Menu>
                {
				   new Menu {menuName = "显示统计分析列表", menuType="功能",menuController="ItService",menuAction="ItServiceList",menuOrder=1000,menuIcon="",parentId=bStatisticMenuList.ID,isSysMenu="0"},
                   new Menu {menuName = "导出统计分析xls", menuType="功能",menuController="ItService",menuAction="IsiExportXls",menuOrder=1000,menuIcon="",parentId=bStatisticMenuList.ID,isSysMenu="0"}
			    };
                Menus.ForEach(s => context.Menus.AddOrUpdate(p => p.menuName, s));
                context.SaveChanges();
            }

            #endregion
        }
    }
}
