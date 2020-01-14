using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Apache.Models
{
    public class Organization
    {
    
        public int ID { get; set; }

        [Required]
        public String orgName { get; set; }
        public String orgShortName { get; set; }
        public String orgCode { get; set; }
        public String orgType { get; set; }
        public String orgNote { get; set; }
        public string temp_id { get; set; }
        public string temp_pid { get; set; }
        public int parentId { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }

    public class DataDict
    {
        public int ID { get; set; }

        [Required]
        public String DataDictName { get; set; }
        public String DataDictCode { get; set; }
        public String DataDictType { get; set; }
        public String DataDictNote { get; set; }

        public int parentId { get; set; }

    }

    public class Menu

    {
        public int ID { get; set; }

        [Required]
        public String menuName { get; set; }

        [Required]
        public String menuType { get; set; }
        public String menuController { get; set; }
        public String menuAction { get; set; }
        public int menuOrder { get; set; }

        public String menuNote { get; set; }

        public String menuIcon { get; set; }
        public String isMenu { get; set; }
        public String isSysMenu { get; set;}
        public int parentId { get; set; }

        public virtual ICollection<ApplicationRole> ApplicationRoles { get; set; }
    }

    public class Log
    {
        public Guid ID { get; set; }

        public DateTime? logTime { get; set; }
        public String logUser { get; set; }
        public String logUserIp { get; set; }
        public String logController { get; set; }
        public String logAction { get; set; }
        public String logPram { get; set; }

    }
    public class Libraries
    {
        [Key]
        public int LibraryID { get; set; }
        public String LibraryName { get; set; }
        public String Owner { get; set; }
        public String Phone { get; set; }
        public String LibraryType { get; set; }

        public int ParentID { get; set; }
        public String Icon { get; set; }
        public String Path { get; set; }
        public String Url { get; set; }
        public int IsSMSVerify { get; set; }
    }
    public class LibraryApprovers
    {
        [Key]
        public String ID { get; set; }
        public String LibID { get; set; }
        public String Approver { get; set; }
        public String ApproverName { get; set; }
        public String ApproverLevel { get; set; }

    }

}