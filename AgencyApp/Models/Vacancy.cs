using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgencyApp.Models
{
    /// <summary>
    /// Вакансия работодателя.
    /// </summary>
    public class Vacancy
    {
        public int Id { get; set; }

        /// <summary>
        /// Должность (название вакансии).
        /// </summary>
        [Required(ErrorMessage = "Должность обязательна")]
        [StringLength(200)]
        public string Position { get; set; }

        /// <summary>
        /// Описание вакансии.
        /// </summary>
        [Required(ErrorMessage = "Описание вакансии обязательно")]
        public string Description { get; set; }

        /// <summary>
        /// Требования к кандидату.
        /// </summary>
        [Required(ErrorMessage = "Требования к кандидату обязательны")]
        public string Requirements { get; set; }

        /// <summary>
        /// Предлагаемая зарплата (строкой для удобства, можно хранить и числом).
        /// </summary>
        [StringLength(100)]
        public string Salary { get; set; }

        /// <summary>
        /// Владелец вакансии (работодатель).
        /// </summary>
        public int EmployerId { get; set; }

        public virtual Employer Employer { get; set; }

        /// <summary>
        /// Навыки, требуемые для вакансии (многие-ко-многим).
        /// </summary>
        public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

        /// <summary>
        /// Отклики / собеседования по данной вакансии.
        /// </summary>
        public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    }
}

