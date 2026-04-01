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
    /// Форма для управления вакансиями.
    /// </summary>
    public class VacanciesForm : BaseCrudForm
    {
        private TextBox _txtPosition;
        private TextBox _txtDescription;
        private TextBox _txtRequirements;
        private TextBox _txtSalary;
        private ComboBox _cmbEmployer;
        private AgencyContext _context;

        public VacanciesForm()
        {
            InitializeBaseComponents("Вакансии");
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

            var lblPosition = new Label { Text = "Должность:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblDescription = new Label { Text = "Описание:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblRequirements = new Label { Text = "Требования:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblSalary = new Label { Text = "Зарплата:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblEmployer = new Label { Text = "Работодатель:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };

            _txtPosition = new TextBox { Dock = DockStyle.Fill };
            _txtDescription = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 60 };
            _txtRequirements = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 60 };
            _txtSalary = new TextBox { Dock = DockStyle.Fill };
            _cmbEmployer = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };

            panel.Controls.Add(lblPosition, 0, 0);
            panel.Controls.Add(_txtPosition, 1, 0);
            panel.Controls.Add(lblDescription, 0, 1);
            panel.Controls.Add(_txtDescription, 1, 1);
            panel.Controls.Add(lblRequirements, 0, 2);
            panel.Controls.Add(_txtRequirements, 1, 2);
            panel.Controls.Add(lblSalary, 0, 3);
            panel.Controls.Add(_txtSalary, 1, 3);
            panel.Controls.Add(lblEmployer, 0, 4);
            panel.Controls.Add(_cmbEmployer, 1, 4);

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
                var vacancies = await _context.Vacancies
                    .Include(v => v.Employer)
                    .AsNoTracking()
                    .ToListAsync();

                Grid.DataSource = vacancies
                    .Select(v => new
                    {
                        v.Id,
                        v.Position,
                        v.Salary,
                        Employer = v.Employer != null ? v.Employer.CompanyName : string.Empty
                    })
                    .ToList();

                var employers = await _context.Employers.AsNoTracking().OrderBy(e => e.CompanyName).ToListAsync();
                _cmbEmployer.DataSource = employers;
                _cmbEmployer.DisplayMember = "CompanyName";
                _cmbEmployer.ValueMember = "Id";
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
                if (_cmbEmployer.SelectedItem == null)
                    throw new InvalidOperationException("Не выбран работодатель.");

                var vacancy = new Vacancy
                {
                    Position = _txtPosition.Text.Trim(),
                    Description = _txtDescription.Text.Trim(),
                    Requirements = _txtRequirements.Text.Trim(),
                    Salary = _txtSalary.Text.Trim(),
                    EmployerId = (int)_cmbEmployer.SelectedValue
                };

                _context.Vacancies.Add(vacancy);
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
                var vacancy = await _context.Vacancies.FindAsync(id);
                if (vacancy == null)
                    return;

                vacancy.Position = _txtPosition.Text.Trim();
                vacancy.Description = _txtDescription.Text.Trim();
                vacancy.Requirements = _txtRequirements.Text.Trim();
                vacancy.Salary = _txtSalary.Text.Trim();
                vacancy.EmployerId = (int)_cmbEmployer.SelectedValue;

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

            if (MessageBox.Show("Удалить выбранную вакансию?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                int id = (int)Grid.CurrentRow.Cells["Id"].Value;
                var vacancy = await _context.Vacancies.FindAsync(id);
                if (vacancy == null)
                    return;

                _context.Vacancies.Remove(vacancy);
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

