using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgencyApp.Database;
using AgencyApp.Forms.Appointments;
using AgencyApp.Models;
using AgencyApp.Repositories;
using AgencyApp.Services;

namespace AgencyApp.Forms
{
    /// <summary>
    /// Форма для управления соискателями (CRUD).
    /// Демонстрирует работу с текстовыми компонентами, обработчиками событий и репозиторием.
    /// </summary>
    public class ApplicantsForm : BaseCrudForm
    {
        private TextBox _txtFullName;
        private TextBox _txtContacts;
        private DateTimePicker _dtBirthDate;

        private ApplicantService _service;

        public ApplicantsForm()
        {
            InitializeBaseComponents("Соискатели");
            InitializeFields();
            InitializeDataLayer();
            _ = LoadDataAsync();
        }

        private void InitializeFields()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(10)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            var lblName = new Label { Text = "ФИО:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblContacts = new Label { Text = "Контакты:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblBirthDate = new Label { Text = "Дата рождения:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };

            _txtFullName = new TextBox { Dock = DockStyle.Fill };
            _txtContacts = new TextBox { Dock = DockStyle.Fill };
            _dtBirthDate = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Short };

            panel.Controls.Add(lblName, 0, 0);
            panel.Controls.Add(_txtFullName, 1, 0);
            panel.Controls.Add(lblContacts, 0, 1);
            panel.Controls.Add(_txtContacts, 1, 1);
            panel.Controls.Add(lblBirthDate, 0, 2);
            panel.Controls.Add(_dtBirthDate, 1, 2);

            Controls.Add(panel);

            BtnAdd.Click += async (_, _) => await AddApplicantAsync();
            BtnEdit.Click += async (_, _) => await EditApplicantAsync();
            BtnDelete.Click += async (_, _) => await DeleteApplicantAsync();
            BtnRefresh.Click += async (_, _) => await LoadDataAsync();
        }

        private void InitializeDataLayer()
        {
            var context = new AgencyContext();
            var repo = new GenericRepository<Applicant>(context);
            _service = new ApplicantService(repo);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var data = await _service.GetAllAsync();
                Grid.DataSource = data
                    .Select(a => new
                    {
                        a.Id,
                        a.FullName,
                        a.ContactInfo,
                        BirthDate = a.BirthDate.ToShortDateString()
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task AddApplicantAsync()
        {
            try
            {
                // Проверка заполнения ФИО
                if (string.IsNullOrWhiteSpace(_txtFullName.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните ФИО.", "Ошибка валидации",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtFullName.Focus();
                    return;
                }

                // Проверка заполнения контактов
                if (string.IsNullOrWhiteSpace(_txtContacts.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните контакты.", "Ошибка валидации",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtContacts.Focus();
                    return;
                }

                // Проверка возраста (должен быть >= 14 лет)
                var birthDate = _dtBirthDate.Value.Date;
                var age = CalculateAge(birthDate);
                if (age < 14)
                {
                    MessageBox.Show("Возраст соискателя должен быть не менее 14 лет.", "Ошибка валидации",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var applicant = new Applicant
                {
                    FullName = _txtFullName.Text.Trim(),
                    ContactInfo = _txtContacts.Text.Trim(),
                    BirthDate = birthDate
                };

                await _service.AddAsync(applicant);
                await LoadDataAsync();
                ClearFields();
                MessageBox.Show("Соискатель успешно добавлен!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task EditApplicantAsync()
        {
            if (Grid.CurrentRow == null)
            {
                MessageBox.Show("Пожалуйста, выберите соискателя для редактирования.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                int id = (int)Grid.CurrentRow.Cells["Id"].Value;

                // Проверка заполнения ФИО
                if (string.IsNullOrWhiteSpace(_txtFullName.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните ФИО.", "Ошибка валидации",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtFullName.Focus();
                    return;
                }

                // Проверка заполнения контактов
                if (string.IsNullOrWhiteSpace(_txtContacts.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните контакты.", "Ошибка валидации",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _txtContacts.Focus();
                    return;
                }

                // Проверка возраста (должен быть >= 14 лет)
                var birthDate = _dtBirthDate.Value.Date;
                var age = CalculateAge(birthDate);
                if (age < 14)
                {
                    MessageBox.Show("Возраст соискателя должен быть не менее 14 лет.", "Ошибка валидации",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Простое обновление по текущим полям ввода
                var context = new AgencyContext();
                var repo = new GenericRepository<Applicant>(context);
                var applicant = await repo.GetByIdAsync(id);
                if (applicant == null)
                    return;

                applicant.FullName = _txtFullName.Text.Trim();
                applicant.ContactInfo = _txtContacts.Text.Trim();
                applicant.BirthDate = birthDate;

                await repo.UpdateAsync(applicant);
                await LoadDataAsync();
                ClearFields();
                MessageBox.Show("Данные соискателя обновлены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task DeleteApplicantAsync()
        {
            if (Grid.CurrentRow == null)
            {
                MessageBox.Show("Пожалуйста, выберите соискателя для удаления.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Удалить выбранного соискателя?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                int id = (int)Grid.CurrentRow.Cells["Id"].Value;
                var context = new AgencyContext();
                var repo = new GenericRepository<Applicant>(context);
                var applicant = await repo.GetByIdAsync(id);
                if (applicant == null)
                    return;

                await repo.DeleteAsync(applicant);
                await LoadDataAsync();
                ClearFields();
                MessageBox.Show("Соискатель удалён из списка.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        /// <summary>
        /// Вычисление возраста на основе даты рождения.
        /// </summary>
        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
                age--;
            return age;
        }

        /// <summary>
        /// Очистка полей ввода после операций.
        /// </summary>
        private void ClearFields()
        {
            _txtFullName.Clear();
            _txtContacts.Clear();
            _dtBirthDate.Value = DateTime.Today;
        }
    }
}

