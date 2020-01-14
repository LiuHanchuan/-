using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Apache.Models
{
    public class BusinessDataContext : DbContext
    {
        public DbSet<WorkFlowItem> WorkFlowItems { get; set; }

        public DbSet<ItServiceItem> ItServiceItems { get; set; }
    }
}