using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesc.Models
{
    // модель отделы
    public class Department
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название отдела")]
        [MaxLength(50, ErrorMessage = "Превышена максимальная длина записи")]
        public string Name { get; set; }
    }
}
