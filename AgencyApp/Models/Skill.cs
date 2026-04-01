using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgencyApp.Models
{
    /// <summary>
    /// Навык (например, C#, SQL, Коммуникабельность).
    /// </summary>
    public class Skill
    {
        public int Id { get; set; }

        /// <summary>
        /// Название навыка.
        /// </summary>
        [Required(ErrorMessage = "Название навыка обязательно")]
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Описание категории (например, "Технический", "Soft skill").
        /// </summary>
        [StringLength(200)]
        public string CategoryDescription { get; set; }

        public virtual ICollection<Resume> Resumes { get; set; } = new List<Resume>();

        public virtual ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
    }
}

