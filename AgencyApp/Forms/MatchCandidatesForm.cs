using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgencyApp.Database;
using Microsoft.EntityFrameworkCore;

namespace AgencyApp.Forms
{
    /// <summary>
    /// Форма подбора кандидатов под вакансию и поиска вакансий по навыкам.
    /// </summary>
    public class MatchCandidatesForm : Form
    {
        private ComboBox _cmbVacancy;
        private Button _btnFindCandidates;
        private DataGridView _gridCandidates;

        private ComboBox _cmbSkill;
        private Button _btnFindVacancies;
        private DataGridView _gridVacancies;

        private AgencyContext _context;

        public MatchCandidatesForm()
        {
            InitializeComponent();
            _context = new AgencyContext();
            _ = LoadLookupsAsync();
        }

        private void InitializeComponent()
        {
            Text = "Подбор кандидатов и поиск вакансий";
            StartPosition = FormStartPosition.CenterParent;
            Width = 900;
            Height = 600;

            var mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 280
            };
            Controls.Add(mainSplit);

            // Верхняя часть: подбор кандидатов под вакансию
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 60 };
            _cmbVacancy = new ComboBox { Left = 10, Top = 20, Width = 400, DropDownStyle = ComboBoxStyle.DropDownList };
            _btnFindCandidates = new Button { Text = "Найти кандидатов", Left = 420, Top = 18, Width = 150 };
            _btnFindCandidates.Click += async (_, _) => await FindCandidatesAsync();
            topPanel.Controls.Add(_cmbVacancy);
            topPanel.Controls.Add(_btnFindCandidates);

            _gridCandidates = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var topContainer = new Panel { Dock = DockStyle.Fill };
            topContainer.Controls.Add(_gridCandidates);
            topContainer.Controls.Add(topPanel);
            mainSplit.Panel1.Controls.Add(topContainer);

            // Нижняя часть: поиск вакансий по навыку
            var bottomPanel = new Panel { Dock = DockStyle.Top, Height = 60 };
            _cmbSkill = new ComboBox { Left = 10, Top = 20, Width = 400, DropDownStyle = ComboBoxStyle.DropDownList };
            _btnFindVacancies = new Button { Text = "Найти вакансии", Left = 420, Top = 18, Width = 150 };
            _btnFindVacancies.Click += async (_, _) => await FindVacanciesAsync();
            bottomPanel.Controls.Add(_cmbSkill);
            bottomPanel.Controls.Add(_btnFindVacancies);

            _gridVacancies = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var bottomContainer = new Panel { Dock = DockStyle.Fill };
            bottomContainer.Controls.Add(_gridVacancies);
            bottomContainer.Controls.Add(bottomPanel);
            mainSplit.Panel2.Controls.Add(bottomContainer);
        }

        private async Task LoadLookupsAsync()
        {
            try
            {
                var vacancies = await _context.Vacancies
                    .Include(v => v.Employer)
                    .AsNoTracking()
                    .ToListAsync();

                _cmbVacancy.DataSource = vacancies;
                _cmbVacancy.DisplayMember = "Position";
                _cmbVacancy.ValueMember = "Id";

                var skills = await _context.Skills.AsNoTracking().OrderBy(s => s.Name).ToListAsync();
                _cmbSkill.DataSource = skills;
                _cmbSkill.DisplayMember = "Name";
                _cmbSkill.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки справочников",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Поиск кандидатов под вакансию по совпадающим навыкам.
        /// </summary>
        private async Task FindCandidatesAsync()
        {
            if (_cmbVacancy.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите вакансию.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int vacancyId = (int)_cmbVacancy.SelectedValue;

                // Ищем резюме, у которых есть хотя бы один навык, совпадающий с навыками вакансии
                var vacancySkills = await _context.Vacancies
                    .Where(v => v.Id == vacancyId)
                    .SelectMany(v => v.Skills.Select(s => s.Id))
                    .ToListAsync();

                // Если у вакансии нет навыков
                if (vacancySkills == null || !vacancySkills.Any())
                {
                    _gridCandidates.DataSource = null;
                    MessageBox.Show("У выбранной вакансии не указаны навыки. Невозможно подобрать кандидатов.", 
                        "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var resumes = await _context.Resumes
                    .Include(r => r.Applicant)
                    .Include(r => r.Skills)
                    .Where(r => r.Skills.Any(s => vacancySkills.Contains(s.Id)))
                    .AsNoTracking()
                    .ToListAsync();

                if (resumes == null || !resumes.Any())
                {
                    _gridCandidates.DataSource = null;
                    MessageBox.Show("Кандидаты не найдены.", "Результат поиска",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _gridCandidates.DataSource = resumes
                    .Select(r => new
                    {
                        Соискатель = r.Applicant != null ? r.Applicant.FullName : string.Empty,
                        Резюме = r.Title,
                        Навыки = string.Join(", ", r.Skills.Select(s => s.Name))
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка поиска кандидатов",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Поиск вакансий по выбранному навыку.
        /// </summary>
        private async Task FindVacanciesAsync()
        {
            if (_cmbSkill.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите навык.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int skillId = (int)_cmbSkill.SelectedValue;

                var vacancies = await _context.Vacancies
                    .Include(v => v.Employer)
                    .Include(v => v.Skills)
                    .Where(v => v.Skills.Any(s => s.Id == skillId))
                    .AsNoTracking()
                    .ToListAsync();

                if (vacancies == null || !vacancies.Any())
                {
                    _gridVacancies.DataSource = null;
                    MessageBox.Show("Вакансии с выбранным навыком не найдены.", "Результат поиска",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _gridVacancies.DataSource = vacancies
                    .Select(v => new
                    {
                        Вакансия = v.Position,
                        Работодатель = v.Employer != null ? v.Employer.CompanyName : string.Empty,
                        Зарплата = v.Salary
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка поиска вакансий",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

