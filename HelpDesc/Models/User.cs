using HelpDesc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HelpDesc.Models
{
    public class User
    {

        // ID 
        public int Id { get; set; }
        // Имя
        [Required]
        [Display(Name = "Имя")]
        [MaxLength(50, ErrorMessage = "Превышена максимальная длина записи")]
        public string Name { get; set; }
        // Фамилия
        [Required]
        [Display(Name = "Фамилия")]
        [MaxLength(50, ErrorMessage = "Превышена максимальная длина записи")]
        public string LastName { get; set; }
        // Логин 
        [Required]
        [Display(Name = "Логин")]
        [MaxLength(50, ErrorMessage = "Превышена максимальная длина записи")]
        public string Login { get; set; }
        // Пароль
        [Required]
        [Display(Name = "Пароль")]
        [MaxLength(50, ErrorMessage = "Превышена максимальная длина записи")]
        public string Password { get; set; }
        // Email
        [Display(Name = "Email")]
        [MaxLength(50, ErrorMessage = "Превышена максимальная длина записи")]
        public string Email { get; set; }
        // Активные / не активный 
        [Required]
        [Display(Name = "Активный")]
        public Boolean Active { get; set; }
        // Номер телефона
        [Display(Name = "Телефон")]
        [MaxLength(20, ErrorMessage = "Превышена максимальная длина записи")]
        public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Должность")]
        [MaxLength(50, ErrorMessage = "Превышена максимальная длина записи")]
        public string Position { get; set; }
        // Отдел
        [Display(Name = "Отдел")]
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
        // Статус
        [Required]
        [Display(Name = "Роль")]
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}