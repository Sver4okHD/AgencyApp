using System;
using System.IO;
using AgencyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AgencyApp.Database
{
    /// <summary>
    /// Контекст базы данных кадрового агентства.
    /// Отвечает за подключение к SQLite и описание сущностей.
    /// </summary>
    public class AgencyContext : DbContext
    {
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<Interview> Interviews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Формируем путь к файлу БД в подпапке Database рядом с exe
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string dbFolder = Path.Combine(baseDir, "Database");
                if (!Directory.Exists(dbFolder))
                {
                    Directory.CreateDirectory(dbFolder);
                }

                string dbPath = Path.Combine(dbFolder, "agency.db");
                string connectionString = $"Data Source={dbPath}";

                optionsBuilder.UseSqlite(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Связь многие-ко-многим: Resume <-> Skill
            modelBuilder.Entity<Resume>()
                .HasMany(r => r.Skills)
                .WithMany(s => s.Resumes)
                .UsingEntity(j => j.ToTable("ResumeSkills"));

            // Связь многие-ко-многим: Vacancy <-> Skill
            modelBuilder.Entity<Vacancy>()
                .HasMany(v => v.Skills)
                .WithMany(s => s.Vacancies)
                .UsingEntity(j => j.ToTable("VacancySkills"));

            // Связь Applicant - Resume (один-ко-многим)
            modelBuilder.Entity<Applicant>()
                .HasMany(a => a.Resumes)
                .WithOne(r => r.Applicant)
                .HasForeignKey(r => r.ApplicantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь Employer - Vacancy (один-ко-многим)
            modelBuilder.Entity<Employer>()
                .HasMany(e => e.Vacancies)
                .WithOne(v => v.Employer)
                .HasForeignKey(v => v.EmployerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь Vacancy - Interview (один-ко-многим)
            modelBuilder.Entity<Vacancy>()
                .HasMany(v => v.Interviews)
                .WithOne(i => i.Vacancy)
                .HasForeignKey(i => i.VacancyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь Resume - Interview (один-ко-многим)
            modelBuilder.Entity<Resume>()
                .HasMany<Interview>()
                .WithOne(i => i.Resume)
                .HasForeignKey(i => i.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

