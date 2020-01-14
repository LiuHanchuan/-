using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Apache.Models;

namespace Apache.ViewModels
{
    public class UserMenuList
    {
        public IEnumerable<UserMenuPackage> UserMenuPackages { get; set; }
        public IEnumerable<Menu> Menus { get; set; }
    }
    public class UserMenuPackage
    {

        public String menuName { get; set; }

        public String menuType { get; set; }

        public int menuOrder { get; set; }

        public String menuIcon { get; set; }

        public IEnumerable<Menu> Menus { get; set; }
    }

}