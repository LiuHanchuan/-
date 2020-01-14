
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Collections.Generic;
using System;
using System.Configuration;

namespace Apache.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        [Required]
        public string TrueName { get; set; }
        public string Job { get; set; }
        public string Mobile { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AllowLibID { get; set; }
        public string PW { get; set; }
        [Required]
        [ForeignKey("Organization")]
        public int OrganizationID { get; set; }

        public int Order { get; set; }
        public virtual Organization Organization { get; set; }
    }
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { CreateRoleTime = DateTime.Now; }
        public ApplicationRole(string name) : base(name) { }
        public string Description { get; set; }
        public DateTime? CreateRoleTime { get; set; }
        public int Rank { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }

    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ApacheConnection", throwIfV1Schema: false)
        {
            
        }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<DataDict> DataDicts { get; set; }
        public DbSet<Menu> Menus { get; set; }

        public DbSet<Log> Logs { get; set; }
        public new IDbSet<ApplicationRole> Roles { get; set; }
        public DbSet<Libraries> Libraries { get; set; }
        public DbSet<LibraryApprovers> LibraryApprovers { get; set; }
    }
}