using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgencyApp.Models
{
    /// <summary>
    /// Работодатель (компания).
    /// </summary>
    public class Employer
    {
        public int Id { get; set; }

        /// <summary>
        /// Название компании.
        /// </summary>
        [Required(ErrorMessage = "Название компании обязательно")]
        [StringLength(200)]
        public string CompanyName { get; set; }

        /// <summary>
        /// Контакты работодателя.
        /// </summary>
        [Required(ErrorMessage = "Контакты работодателя обязательны")]
        [StringLength(300)]
        public string ContactInfo { get; set; }

        /// <summary>
        /// Краткое описание компании.
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        public virtual ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
    }
}

