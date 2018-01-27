using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesc.Models
{
    //указывает на категорию проблемы, например, проблема с сетью, с оборудованием или проблема с программным обеспечением.
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Название категории")]
        [MaxLength(50, ErrorMessage = "Превышена максимальная длина записи")]
        public string Name { get; set; }
    }
}
