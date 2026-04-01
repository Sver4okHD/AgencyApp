using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgencyApp.Models
{
    /// <summary>
    /// Резюме соискателя.
    /// </summary>
    public class Resume
    {
        public int Id { get; set; }

        /// <summary>
        /// Название резюме (например, "Junior C# разработчик").
        /// </summary>
        [Required(ErrorMessage = "Название резюме обязательно")]
        [StringLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// Подробное описание, опыт, образование и т.п.
        /// </summary>
        [Required(ErrorMessage = "Описание резюме обязательно")]
        public string Description { get; set; }

        /// <summary>
        /// Дата создания резюме.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Статус резюме (активно, архив и т.п.).
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        /// <summary>
        /// Владелец резюме.
        /// </summary>
        [Required]
        public int ApplicantId { get; set; }

        public virtual Applicant Applicant { get; set; }

        /// <summary>
        /// Навыки, указанные в резюме (многие-ко-многим).
        /// </summary>
        public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}

