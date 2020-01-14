using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Apache.Models
{
    public class WorkFlowItem
    {
        public int ID { get; set; }

        [Required]
        public Guid   WfInstanceId { get; set; }
        public String WfType { get; set; }
        public int WfBusinessId { get; set; }
        public string WfBussinessUrl { get; set; }
        public String WfCurrentUser { get; set; }
        public String WfDrafter { get; set; }
        public String Wfstatus { get; set; }
        public DateTime WfCreateDate { get; set; }
        public DateTime? WfCompleteDate { get; set; }
        public String WfFlowChart { get; set; }
        public String WfWriteField { get; set; }         //定义写入域类型，即意见写入什么地方，如果为空，则不写入意见，2015/7/19
    }
    public class WorkFlowInParameter
    {
        public String drafter { get; set; }
    }
}