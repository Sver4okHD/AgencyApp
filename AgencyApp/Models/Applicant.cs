using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgencyApp.Models
{
    /// <summary>
    /// Соискатель (кандидат) на вакансию.
    /// </summary>
    public class Applicant
    {
        /// <summary>
        /// Уникальный идентификатор соискателя.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ФИО соискателя.
        /// </summary>
        [Required(ErrorMessage = "ФИО обязательно для заполнения")]
        [StringLength(200, ErrorMessage = "Максимальная длина ФИО — 200 символов")]
        public string FullName { get; set; }

        /// <summary>
        /// Контактная информация (телефон, e-mail и т.п.).
        /// </summary>
        [Required(ErrorMessage = "Контактные данные обязательны")]
        [StringLength(300, ErrorMessage = "Максимальная длина контактов — 300 символов")]
        public string ContactInfo { get; set; }

        /// <summary>
        /// Дата рождения соискателя.
        /// </summary>
        [Required(ErrorMessage = "Дата рождения обязательна")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Навигационное свойство — список резюме соискателя.
        /// </summary>
        public virtual ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    }
}

