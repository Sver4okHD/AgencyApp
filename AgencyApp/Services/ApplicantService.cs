using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgencyApp.Models;
using AgencyApp.Repositories;

namespace AgencyApp.Services
{
    /// <summary>
    /// Сервис для работы с соискателями (над слоем репозиториев).
    /// </summary>
    public class ApplicantService
    {
        private readonly IGenericRepository<Applicant> _repository;

        public ApplicantService(IGenericRepository<Applicant> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Получить всех соискателей.
        /// </summary>
        public Task<IEnumerable<Applicant>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        /// <summary>
        /// Добавить нового соискателя с базовой проверкой данных.
        /// </summary>
        public async Task AddAsync(Applicant applicant)
        {
            if (applicant == null)
                throw new ArgumentNullException(nameof(applicant));

            if (string.IsNullOrWhiteSpace(applicant.FullName))
                throw new ArgumentException("ФИО не может быть пустым", nameof(applicant.FullName));

            if (string.IsNullOrWhiteSpace(applicant.ContactInfo))
                throw new ArgumentException("Контактная информация не может быть пустой", nameof(applicant.ContactInfo));

            if (applicant.BirthDate > DateTime.Now.AddYears(-14))
                throw new ArgumentException("Возраст соискателя должен быть не менее 14 лет", nameof(applicant.BirthDate));

            await _repository.AddAsync(applicant);
        }

        public Task UpdateAsync(Applicant applicant)
        {
            if (applicant == null)
                throw new ArgumentNullException(nameof(applicant));

            return _repository.UpdateAsync(applicant);
        }

        public Task DeleteAsync(Applicant applicant)
        {
            if (applicant == null)
                throw new ArgumentNullException(nameof(applicant));

            return _repository.DeleteAsync(applicant);
        }
    }
}

