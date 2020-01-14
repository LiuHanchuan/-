using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Apache.Models;

// 下载于www.mycodes.net
namespace Apache.DAL
{
    public class ApacheContext : DbContext
    {

        public ApacheContext()
            : base("ApacheConnection")
        {
        }


    }
}