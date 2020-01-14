namespace Apache.BusinessDataContextMigration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItServiceItems",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        isitype = c.String(),
                        sub_isitype = c.String(),
                        end_isitype = c.String(),
                        drafter = c.String(),
                        applicant = c.String(),
                        applicantPhone = c.String(),
                        applicant_dept = c.String(),
                        description = c.String(),
                        solution = c.String(),
                        DealwithpeopleName = c.String(),
                        ItManagerOpinion = c.String(),
                        ProcessingDepartmentOpinion = c.String(),
                        DealwithpeopleOpinion = c.String(),
                        isiCreateDate = c.DateTime(nullable: false),
                        isiCompleteDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.WorkFlowItems",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        WfInstanceId = c.Guid(nullable: false),
                        WfType = c.String(),
                        WfBusinessId = c.Int(nullable: false),
                        WfBussinessUrl = c.String(),
                        WfCurrentUser = c.String(),
                        WfDrafter = c.String(),
                        Wfstatus = c.String(),
                        WfCreateDate = c.DateTime(nullable: false),
                        WfCompleteDate = c.DateTime(),
                        WfFlowChart = c.String(),
                        WfWriteField = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WorkFlowItems");
            DropTable("dbo.ItServiceItems");
        }
    }
}
