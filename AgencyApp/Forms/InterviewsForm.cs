using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgencyApp.Database;
using AgencyApp.Forms.Appointments;
using AgencyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AgencyApp.Forms
{
    /// <summary>
    /// Форма для управления собеседованиями / откликами.
    /// </summary>
    public class InterviewsForm : BaseCrudForm
    {
        private DateTimePicker _dtDate;
        private ComboBox _cmbStatus;
        private TextBox _txtComments;
        private ComboBox _cmbResume;
        private ComboBox _cmbVacancy;

        private AgencyContext _context;

        public InterviewsForm()
        {
            InitializeBaseComponents("Собеседования / отклики");
            InitializeFields();
            _context = new AgencyContext();
            _ = LoadDataAsync();
        }

        private void InitializeFields()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(10)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var lblDate = new Label { Text = "Дата:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblStatus = new Label { Text = "Статус:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblComments = new Label { Text = "Комментарии:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblResume = new Label { Text = "Резюме:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblVacancy = new Label { Text = "Вакансия:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };

            _dtDate = new DateTimePicker { Dock = DockStyle.Fill, Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy HH:mm" };
            _cmbStatus = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbStatus.Items.AddRange(new object[]
            {
                "Назначено",
                "Проведено",
                "Принят",
                "Отказ"
            });

            _txtComments = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 60 };
            _cmbResume = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbVacancy = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };

            panel.Controls.Add(lblDate, 0, 0);
            panel.Controls.Add(_dtDate, 1, 0);
            panel.Controls.Add(lblStatus, 0, 1);
            panel.Controls.Add(_cmbStatus, 1, 1);
            panel.Controls.Add(lblComments, 0, 2);
            panel.Controls.Add(_txtComments, 1, 2);
            panel.Controls.Add(lblResume, 0, 3);
            panel.Controls.Add(_cmbResume, 1, 3);
            panel.Controls.Add(lblVacancy, 0, 4);
            panel.Controls.Add(_cmbVacancy, 1, 4);

            Controls.Add(panel);

            BtnAdd.Click += async (_, _) => await AddAsync();
            BtnEdit.Click += async (_, _) => await EditAsync();
            BtnDelete.Click += async (_, _) => await DeleteAsync();
            BtnRefresh.Click += async (_, _) => await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var interviews = await _context.Interviews
                    .Include(i => i.Resume).ThenInclude(r => r.Applicant)
                    .Include(i => i.Vacancy).ThenInclude(v => v.Employer)
                    .AsNoTracking()
                    .ToListAsync();

                Grid.DataSource = interviews
                    .Select(i => new
                    {
                        i.Id,
                        Date = i.Date,
                        i.Status,
                        Resume = i.Resume != null
                            ? $"{i.Resume.Title} ({i.Resume.Applicant?.FullName})"
                            : string.Empty,
                        Vacancy = i.Vacancy != null
                            ? $"{i.Vacancy.Position} ({i.Vacancy.Employer?.CompanyName})"
                            : string.Empty
                    })
                    .ToList();

                var resumes = await _context.Resumes
                    .Include(r => r.Applicant)
                    .AsNoTracking()
                    .ToListAsync();

                _cmbResume.DataSource = resumes;
                _cmbResume.DisplayMember = "Title";
                _cmbResume.ValueMember = "Id";

                var vacancies = await _context.Vacancies
                    .Include(v => v.Employer)
                    .AsNoTracking()
                    .ToListAsync();

                _cmbVacancy.DataSource = vacancies;
                _cmbVacancy.DisplayMember = "Position";
                _cmbVacancy.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task AddAsync()
        {
            try
            {
                if (_cmbStatus.SelectedItem == null)
                    throw new InvalidOperationException("Не выбран статус.");
                if (_cmbResume.SelectedItem == null)
                    throw new InvalidOperationException("Не выбрано резюме.");
                if (_cmbVacancy.SelectedItem == null)
                    throw new InvalidOperationException("Не выбрана вакансия.");

                var interview = new Interview
                {
                    Date = _dtDate.Value,
                    Status = _cmbStatus.SelectedItem.ToString() ?? "Назначено",
                    Comments = _txtComments.Text.Trim(),
                    ResumeId = (int)_cmbResume.SelectedValue,
                    VacancyId = (int)_cmbVacancy.SelectedValue
                };

                _context.Interviews.Add(interview);
                await _context.SaveChangesAsync();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task EditAsync()
        {
            if (Grid.CurrentRow == null)
                return;

            try
            {
                int id = (int)Grid.CurrentRow.Cells["Id"].Value;
                var interview = await _context.Interviews.FindAsync(id);
                if (interview == null)
                    return;

                interview.Date = _dtDate.Value;
                interview.Status = _cmbStatus.SelectedItem?.ToString() ?? "Назначено";
                interview.Comments = _txtComments.Text.Trim();
                interview.ResumeId = (int)_cmbResume.SelectedValue;
                interview.VacancyId = (int)_cmbVacancy.SelectedValue;

                await _context.SaveChangesAsync();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task DeleteAsync()
        {
            if (Grid.CurrentRow == null)
                return;

            if (MessageBox.Show("Удалить выбранное собеседование?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                int id = (int)Grid.CurrentRow.Cells["Id"].Value;
                var interview = await _context.Interviews.FindAsync(id);
                if (interview == null)
                    return;

                _context.Interviews.Remove(interview);
                await _context.SaveChangesAsync();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
    }
}

