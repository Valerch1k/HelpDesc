using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesc.Models
{
    public class Role
    {
        public int Id { get; set; }
        [Display(Name = "Название роли")]
        public string Name { get; set; }
    }
}
