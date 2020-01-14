using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Apache.Helper;
using System.Data;
using System.Data.Entity;
using System.Net;
using Apache.Models;
//using Apache.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;


namespace Apache.Controllers
{
    public class xlsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private BusinessDataContext bdb = new BusinessDataContext();
              public xlsController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())), new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext())))
        {
        }
        
        public xlsController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;

        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public RoleManager<ApplicationRole> RoleManager { get; private set; }
        public string ImportOrg()           //导入组织机构
        {
            string file_org = Server.MapPath("~") + "Files\\org.xlsx";
            DataTable dt;
            dt = OpenXmlExcelHelper.Import(file_org);


            string file_orgrel = Server.MapPath("~") + "Files\\orgrel.xlsx";
            DataTable dtrel;
            dtrel = OpenXmlExcelHelper.Import(file_orgrel);



            foreach (DataRow dr in dt.Rows)
            {
              // sss+=dr[0].ToString()+""+dr[1].ToString()+dr[2].ToString()+"<br>";
                Organization org1 = new Organization();
                org1.orgName = dr[2].ToString();
                org1.orgShortName = dr[1].ToString();
                org1.temp_id = dr[0].ToString();

                foreach (DataRow drrel in dtrel.Rows)
                {
                    if (dr[0].ToString().Trim()==drrel[1].ToString().Trim()){
                        org1.temp_pid = drrel[0].ToString().Trim();
                        continue;
                    }
                }

                db.Organizations.Add(org1);
                db.SaveChanges();
            }

            return "ok";

        }
 

        public string OrgRel()       //生成组织机构关系
        {
            IEnumerable<Organization> org = db.Organizations;
            List<Organization> orglist = db.Organizations.ToList();
            foreach (var o in org)
            {
                if (o.temp_pid != null)
                {
                    var l = orglist.Where(i => i.temp_id == o.temp_pid).FirstOrDefault();
                    o.parentId = l.ID;
                }
            }
            db.SaveChanges();
            return "ok";
        }


        public async Task<ActionResult> ImportUser()       //导入用户
        {
            string file_user = Server.MapPath("~") + "Files\\per.xlsx";
            DataTable dt;
            dt = OpenXmlExcelHelper.Import(file_user);


            List<Organization> orglist = db.Organizations.ToList();


            foreach (DataRow dr in dt.Rows)
            {

                var l = orglist.Where(i => i.temp_id == dr[3].ToString().Trim()).FirstOrDefault();
                var user = new ApplicationUser() { UserName = dr[0].ToString().Trim() };
                user.TrueName = dr[1].ToString().Trim() + dr[2].ToString().Trim();
                user.OrganizationID = l.ID;
                var result = await UserManager.CreateAsync(user, "000000");

            }

            var json = new
            {
                okMsg = "ok"
            };

            return Json(json, "text/html", JsonRequestBehavior.AllowGet);
        }

        public ActionResult test()
        {
            List<ItServiceItem> dd = bdb.ItServiceItems.ToList();
            DataTable dt = ListToDataTable(dd);
            OpenXmlExcelHelper.ExportByWeb(dt, "ccc.xls", "aaa");
            var json = new
            {
                okMsg = "ok"
            };

            return Json(json, "text/html", JsonRequestBehavior.AllowGet);
        }
        public static DataTable ListToDataTable(IList list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    //获取类型
                    Type colType = pi.PropertyType;
                    //当类型为Nullable<>时
                    if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }
                    result.Columns.Add(pi.Name, colType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

     


	}
}