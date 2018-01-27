using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesc.Models
{
    public class EmailTemplate
    {
        public string UserName { get; set; }
        public string UserLastname { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserEmail { get; set; }
        public string UserPosition { get; set; }
        public string UserDepartment { get; set; }

        public string RequestName { get; set; }
        public string RequestDescription { get; set; }
        public string RequestComment { get; set; }
        public int RequestPriority { get; set; }
        public string RequestActiv { get; set; }
        public string RequestCategory { get; set; }

    }
}