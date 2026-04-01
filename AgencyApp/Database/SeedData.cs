using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AgencyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AgencyApp.Database
{
    /// <summary>
    /// Заполнение БД тестовыми данными.
    /// Важно: метод сделан идемпотентным — при повторном запуске данные не дублируются.
    /// </summary>
    public static class SeedData
    {
        public static async Task SeedAsync(AgencyContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            // Быстрая проверка: если уже есть ключевые записи, сидинг не нужен.
            // Но всё равно ниже используем проверки по уникальным полям, чтобы не было дублей.
            await SeedSkillsAsync(context);
            await SeedApplicantsAsync(context);
            await SeedEmployersAsync(context);
            await SeedVacanciesAsync(context);
            await SeedResumesAndLinksAsync(context);
            await SeedInterviewsAsync(context);
        }

        private static async Task SeedSkillsAsync(AgencyContext context)
        {
            var skills = new (string Name, string Category)[]
            {
                ("C#", "Программирование"),
                ("SQL", "Базы данных"),
                ("JavaScript", "Программирование"),
                ("Python", "Программирование"),
                ("Коммуникабельность", "Soft skills"),
                ("Лидерство", "Soft skills"),
                ("Английский язык", "Языки"),
                ("1С", "Бухгалтерия"),
            };

            foreach (var s in skills)
            {
                bool exists = await context.Skills.AnyAsync(x => x.Name == s.Name);
                if (!exists)
                {
                    context.Skills.Add(new Skill
                    {
                        Name = s.Name,
                        CategoryDescription = s.Category
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedApplicantsAsync(AgencyContext context)
        {
            var applicants = new (string FullName, string Contacts, string BirthDateRu)[]
            {
                ("Иванов Иван Иванович", "ivan@mail.ru, +7-999-123-45-67", "15.05.1995"),
                ("Петрова Мария Сергеевна", "maria@yandex.ru, +7-999-234-56-78", "23.08.1998"),
                ("Сидоров Алексей Петрович", "a.sidorov@gmail.com, +7-999-345-67-89", "10.11.1992"),
                ("Козлова Елена Дмитриевна", "elena@bk.ru, +7-999-456-78-90", "05.03.2000"),

                // Доп. тестовые записи (чтобы данных было больше)
                ("Смирнов Павел Андреевич", "p.smirnov@mail.ru, +7-999-111-22-33", "02.02.1997"),
                ("Васильева Ольга Игоревна", "olga.v@gmail.com, +7-999-222-33-44", "12.12.1996"),
            };

            foreach (var a in applicants)
            {
                bool exists = await context.Applicants.AnyAsync(x => x.FullName == a.FullName);
                if (!exists)
                {
                    context.Applicants.Add(new Applicant
                    {
                        FullName = a.FullName,
                        ContactInfo = a.Contacts,
                        BirthDate = ParseRuDate(a.BirthDateRu)
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedEmployersAsync(AgencyContext context)
        {
            var employers = new (string Company, string Contacts, string Description)[]
            {
                ("ООО \"ТехноСофт\"", "hr@technosoft.ru", "IT-компания, разработка ПО"),
                ("ПАО \"Банк-Центр\"", "job@bankcenter.ru", "Финансовый сектор"),
                ("ИП Иванов", "ivanov@business.ru", "Малый бизнес, розничная торговля"),
                ("ООО \"СтройМастер\"", "info@stroymaster.ru", "Строительная компания"),
            };

            foreach (var e in employers)
            {
                bool exists = await context.Employers.AnyAsync(x => x.CompanyName == e.Company);
                if (!exists)
                {
                    context.Employers.Add(new Employer
                    {
                        CompanyName = e.Company,
                        ContactInfo = e.Contacts,
                        Description = e.Description
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedVacanciesAsync(AgencyContext context)
        {
            // Подтягиваем нужные сущности
            var technoSoft = await context.Employers.SingleAsync(e => e.CompanyName == "ООО \"ТехноСофт\"");
            var bank = await context.Employers.SingleAsync(e => e.CompanyName == "ПАО \"Банк-Центр\"");
            var ipIvanov = await context.Employers.SingleAsync(e => e.CompanyName == "ИП Иванов");
            var stroy = await context.Employers.SingleAsync(e => e.CompanyName == "ООО \"СтройМастер\"");

            var skillCSharp = await context.Skills.SingleAsync(s => s.Name == "C#");
            var skillSql = await context.Skills.SingleAsync(s => s.Name == "SQL");
            var skillJs = await context.Skills.SingleAsync(s => s.Name == "JavaScript");
            var skill1c = await context.Skills.SingleAsync(s => s.Name == "1С");
            var skillComm = await context.Skills.SingleAsync(s => s.Name == "Коммуникабельность");

            var vacancies = new List<(string Position, Employer Employer, string Desc, string Req, string Salary, Skill[] Skills)>
            {
                ("C# Developer", technoSoft, "Разработка backend", "C#, SQL", "150 000 руб", new[] { skillCSharp, skillSql }),
                ("SQL Developer", bank, "Работа с БД", "SQL, аналитика", "120 000 руб", new[] { skillSql }),
                ("Full-stack", technoSoft, "Frontend+Backend", "C#, JS", "180 000 руб", new[] { skillCSharp, skillJs }),
                ("Бухгалтер", stroy, "Учет", "1С", "90 000 руб", new[] { skill1c }),
                ("Менеджер", ipIvanov, "Продажи", "коммуникабельность", "70 000 руб + %", new[] { skillComm }),

                // Доп. вакансия
                ("Python Developer", technoSoft, "Автоматизация и сервисы", "Python, SQL", "160 000 руб", new[] { await context.Skills.SingleAsync(s => s.Name == "Python"), skillSql }),
            };

            foreach (var v in vacancies)
            {
                bool exists = await context.Vacancies.AnyAsync(x => x.Position == v.Position && x.EmployerId == v.Employer.Id);
                if (!exists)
                {
                    var vacancy = new Vacancy
                    {
                        Position = v.Position,
                        Description = v.Desc,
                        Requirements = v.Req,
                        Salary = v.Salary,
                        EmployerId = v.Employer.Id
                    };

                    foreach (var sk in v.Skills)
                        vacancy.Skills.Add(sk);

                    context.Vacancies.Add(vacancy);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedResumesAndLinksAsync(AgencyContext context)
        {
            var ivanov = await context.Applicants.SingleAsync(a => a.FullName == "Иванов Иван Иванович");
            var petrova = await context.Applicants.SingleAsync(a => a.FullName == "Петрова Мария Сергеевна");
            var sidorov = await context.Applicants.SingleAsync(a => a.FullName == "Сидоров Алексей Петрович");
            var kozlova = await context.Applicants.SingleAsync(a => a.FullName == "Козлова Елена Дмитриевна");

            var skillCSharp = await context.Skills.SingleAsync(s => s.Name == "C#");
            var skillSql = await context.Skills.SingleAsync(s => s.Name == "SQL");
            var skillJs = await context.Skills.SingleAsync(s => s.Name == "JavaScript");
            var skillEng = await context.Skills.SingleAsync(s => s.Name == "Английский язык");
            var skillLeadership = await context.Skills.SingleAsync(s => s.Name == "Лидерство");
            var skillComm = await context.Skills.SingleAsync(s => s.Name == "Коммуникабельность");
            var skill1c = await context.Skills.SingleAsync(s => s.Name == "1С");

            var resumes = new List<(Applicant Applicant, string Title, string Desc, string Status, Skill[] Skills)>
            {
                (ivanov, "Junior C# Developer", "Начинающий разработчик, стремлюсь к росту.", "Активное", new[] { skillCSharp, skillSql }),
                (ivanov, "Стажер", "Готов к стажировке и обучению в команде.", "Активное", new[] { skillCSharp, skillComm }),
                (petrova, "Frontend-разработчик", "Разработка интерфейсов, адаптивная верстка.", "Активное", new[] { skillJs, skillEng }),
                (sidorov, "Team Lead", "Управление командой, архитектура, код-ревью.", "Активное", new[] { skillCSharp, skillSql, skillLeadership }),
                (kozlova, "Бухгалтер", "Ведение учета, первичная документация.", "Активное", new[] { skill1c, skillComm }),

                // Доп. резюме
                (petrova, "Junior Full-stack", "Интересуюсь backend и frontend.", "Активное", new[] { skillJs, skillCSharp, skillSql }),
            };

            foreach (var r in resumes)
            {
                bool exists = await context.Resumes.AnyAsync(x => x.ApplicantId == r.Applicant.Id && x.Title == r.Title);
                if (!exists)
                {
                    var resume = new Resume
                    {
                        ApplicantId = r.Applicant.Id,
                        Title = r.Title,
                        Description = r.Desc,
                        Status = r.Status,
                        CreatedAt = DateTime.Now
                    };

                    foreach (var sk in r.Skills)
                        resume.Skills.Add(sk);

                    context.Resumes.Add(resume);
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedInterviewsAsync(AgencyContext context)
        {
            var resumeIvanovJunior = await context.Resumes
                .Include(r => r.Applicant)
                .SingleAsync(r => r.Applicant.FullName == "Иванов Иван Иванович" && r.Title == "Junior C# Developer");

            var resumePetrova = await context.Resumes
                .Include(r => r.Applicant)
                .SingleAsync(r => r.Applicant.FullName == "Петрова Мария Сергеевна" && r.Title == "Frontend-разработчик");

            var resumeSidorov = await context.Resumes
                .Include(r => r.Applicant)
                .SingleAsync(r => r.Applicant.FullName == "Сидоров Алексей Петрович" && r.Title == "Team Lead");

            var resumeKozlova = await context.Resumes
                .Include(r => r.Applicant)
                .SingleAsync(r => r.Applicant.FullName == "Козлова Елена Дмитриевна" && r.Title == "Бухгалтер");

            var vacancyCSharpDev = await context.Vacancies.SingleAsync(v => v.Position == "C# Developer");
            var vacancyFullStack = await context.Vacancies.SingleAsync(v => v.Position == "Full-stack");
            var vacancySqlDev = await context.Vacancies.SingleAsync(v => v.Position == "SQL Developer");
            var vacancyAccountant = await context.Vacancies.SingleAsync(v => v.Position == "Бухгалтер");

            var interviews = new[]
            {
                new { ResumeId = resumeIvanovJunior.Id, VacancyId = vacancyCSharpDev.Id, Date = ParseRuDateTime("20.03.2026 14:00"), Status = "Назначено", Comments = (string)null },
                new { ResumeId = resumePetrova.Id, VacancyId = vacancyFullStack.Id, Date = ParseRuDateTime("21.03.2026 11:00"), Status = "Назначено", Comments = (string)null },
                new { ResumeId = resumeSidorov.Id, VacancyId = vacancySqlDev.Id, Date = ParseRuDateTime("22.03.2026 15:30"), Status = "Проведено", Comments = "Ожидаем решение" },
                new { ResumeId = resumeKozlova.Id, VacancyId = vacancyAccountant.Id, Date = ParseRuDateTime("19.03.2026 10:00"), Status = "Принят", Comments = (string)null },
            };

            foreach (var i in interviews)
            {
                bool exists = await context.Interviews.AnyAsync(x =>
                    x.ResumeId == i.ResumeId &&
                    x.VacancyId == i.VacancyId &&
                    x.Date == i.Date);

                if (!exists)
                {
                    context.Interviews.Add(new Interview
                    {
                        ResumeId = i.ResumeId,
                        VacancyId = i.VacancyId,
                        Date = i.Date,
                        Status = i.Status,
                        Comments = i.Comments
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private static DateTime ParseRuDate(string date)
        {
            return DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        }

        private static DateTime ParseRuDateTime(string dateTime)
        {
            return DateTime.ParseExact(dateTime, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
        }
    }
}

