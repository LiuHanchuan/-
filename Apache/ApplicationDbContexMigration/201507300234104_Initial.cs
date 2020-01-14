namespace Apache.ApplicationDbContexMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataDicts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DataDictName = c.String(nullable: false),
                        DataDictCode = c.String(),
                        DataDictType = c.String(),
                        DataDictNote = c.String(),
                        parentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        logTime = c.DateTime(),
                        logUser = c.String(),
                        logUserIp = c.String(),
                        logController = c.String(),
                        logAction = c.String(),
                        logPram = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        menuName = c.String(nullable: false),
                        menuType = c.String(nullable: false),
                        menuController = c.String(),
                        menuAction = c.String(),
                        menuOrder = c.Int(nullable: false),
                        menuNote = c.String(),
                        menuIcon = c.String(),
                        isMenu = c.String(),
                        isSysMenu = c.String(),
                        parentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Description = c.String(),
                        CreateRoleTime = c.DateTime(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        orgName = c.String(nullable: false),
                        orgShortName = c.String(),
                        orgCode = c.String(),
                        orgType = c.String(),
                        orgNote = c.String(),
                        temp_id = c.String(),
                        temp_pid = c.String(),
                        parentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        TrueName = c.String(nullable: false),
                        Job = c.String(),
                        Mobile = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(maxLength: 256),
                        OrganizationID = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationID, cascadeDelete: true)
                .Index(t => t.OrganizationID)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ApplicationRoleMenus",
                c => new
                    {
                        ApplicationRole_Id = c.String(nullable: false, maxLength: 128),
                        Menu_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationRole_Id, t.Menu_ID })
                .ForeignKey("dbo.AspNetRoles", t => t.ApplicationRole_Id, cascadeDelete: true)
                .ForeignKey("dbo.Menus", t => t.Menu_ID, cascadeDelete: true)
                .Index(t => t.ApplicationRole_Id)
                .Index(t => t.Menu_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "OrganizationID", "dbo.Organizations");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationRoleMenus", "Menu_ID", "dbo.Menus");
            DropForeignKey("dbo.ApplicationRoleMenus", "ApplicationRole_Id", "dbo.AspNetRoles");
            DropIndex("dbo.ApplicationRoleMenus", new[] { "Menu_ID" });
            DropIndex("dbo.ApplicationRoleMenus", new[] { "ApplicationRole_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "OrganizationID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.ApplicationRoleMenus");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Organizations");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Menus");
            DropTable("dbo.Logs");
            DropTable("dbo.DataDicts");
        }
    }
}
