using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Apache.Models
{
    public class ItServiceItem
    {
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }
        public string isitype { get; set; }
        public string sub_isitype { get; set; }
        public string end_isitype { get; set; }
        public string drafter { get; set; }
        public string applicant { get; set; }      //申请人
        public string applicant_dept { get; set; }   //申请人部门
        public string description { get; set; }
        public string solution { get; set; }
        public string DealwithpeopleName { get; set; }   //处理人名字
        public DateTime isiCreateDate { get; set; }
        public DateTime? isiCompleteDate { get; set; }
        public string Object { get; set; }   //研究对象
        public string Purpose { get; set; }  //研究目的
        public string Topic { get; set; }   //研究主题
        public string FirstExamine { get; set; }
        public string SecondExamine { get; set; }
        public string LastExamine { get; set; }
        public int LibraryID { get; set; }
        public int Status { get; set; }  //1  审批中； 2  审批完成； 3  驳回； 4  撤销； 0  无效
    }
}