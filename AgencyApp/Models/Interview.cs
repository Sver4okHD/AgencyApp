using System;
using System.ComponentModel.DataAnnotations;

namespace AgencyApp.Models
{
    /// <summary>
    /// Собеседование / отклик на вакансию.
    /// </summary>
    public class Interview
    {
        public int Id { get; set; }

        /// <summary>
        /// Дата проведения / планируемая дата.
        /// </summary>
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// Статус собеседования (Назначено, Проведено, Принят, Отказ).
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        /// <summary>
        /// Комментарии по итогам собеседования.
        /// </summary>
        [StringLength(1000)]
        public string Comments { get; set; }

        /// <summary>
        /// Связанное резюме (кандидат).
        /// </summary>
        [Required]
        public int ResumeId { get; set; }

        public virtual Resume Resume { get; set; }

        /// <summary>
        /// Связанная вакансия.
        /// </summary>
        [Required]
        public int VacancyId { get; set; }

        public virtual Vacancy Vacancy { get; set; }
    }
}

