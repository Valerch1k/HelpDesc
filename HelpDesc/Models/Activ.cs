using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesc.Models
{
    // Модель Активы
    // Эта модель будет представлять актив - кабинет, принадлежащий определенному отделу.
    // В процессе заполнения заявки пользователь сможет указать кабинет, в котором происходит проблема.
    public class Activ
    {
        public int Id { get; set; }
        // номер кабинета
        [Required]
        [Display(Name = "Номер кабинета")]
        [MaxLength(50, ErrorMessage = "Превышена максимальная длина записи")]
        public string CabNumber { get; set; }

        // Внешний ключ
        // ID Отдела - обычное свойство
        [Required]
        [Display(Name = "Отдел")]
        public int? DepartmentId { get; set; }
        // Отдел - Навигационное свойство
        public Department Department { get; set; }
    }
}
