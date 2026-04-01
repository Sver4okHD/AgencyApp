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
    /// Форма для управления резюме.
    /// </summary>
    public class ResumesForm : BaseCrudForm
    {
        private TextBox _txtTitle;
        private TextBox _txtDescription;
        private TextBox _txtStatus;
        private ComboBox _cmbApplicant;

        private AgencyContext _context;

        public ResumesForm()
        {
            InitializeBaseComponents("Резюме");
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
                RowCount = 4,
                Padding = new Padding(10)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var lblTitle = new Label { Text = "Название:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblDescription = new Label { Text = "Описание:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblStatus = new Label { Text = "Статус:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblApplicant = new Label { Text = "Соискатель:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };

            _txtTitle = new TextBox { Dock = DockStyle.Fill };
            _txtDescription = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 60 };
            _txtStatus = new TextBox { Dock = DockStyle.Fill };
            _cmbApplicant = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };

            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            panel.Controls.Add(lblTitle, 0, 0);
            panel.Controls.Add(_txtTitle, 1, 0);
            panel.Controls.Add(lblDescription, 0, 1);
            panel.Controls.Add(_txtDescription, 1, 1);
            panel.Controls.Add(lblStatus, 0, 2);
            panel.Controls.Add(_txtStatus, 1, 2);
            panel.Controls.Add(lblApplicant, 0, 3);
            panel.Controls.Add(_cmbApplicant, 1, 3);

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
                var resumes = await _context.Resumes
                    .Include(r => r.Applicant)
                    .AsNoTracking()
                    .ToListAsync();

                Grid.DataSource = resumes
                    .Select(r => new
                    {
                        r.Id,
                        r.Title,
                        r.Status,
                        r.CreatedAt,
                        Applicant = r.Applicant != null ? r.Applicant.FullName : string.Empty
                    })
                    .ToList();

                var applicants = await _context.Applicants
                    .AsNoTracking()
                    .OrderBy(a => a.FullName)
                    .ToListAsync();

                _cmbApplicant.DataSource = applicants;
                _cmbApplicant.DisplayMember = "FullName";
                _cmbApplicant.ValueMember = "Id";
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
                if (_cmbApplicant.SelectedItem == null)
                    throw new InvalidOperationException("Не выбран соискатель.");

                var resume = new Resume
                {
                    Title = _txtTitle.Text.Trim(),
                    Description = _txtDescription.Text.Trim(),
                    Status = _txtStatus.Text.Trim(),
                    ApplicantId = (int)_cmbApplicant.SelectedValue
                };

                _context.Resumes.Add(resume);
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
                var resume = await _context.Resumes.FindAsync(id);
                if (resume == null)
                    return;

                resume.Title = _txtTitle.Text.Trim();
                resume.Description = _txtDescription.Text.Trim();
                resume.Status = _txtStatus.Text.Trim();
                resume.ApplicantId = (int)_cmbApplicant.SelectedValue;

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

            if (MessageBox.Show("Удалить выбранное резюме?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                int id = (int)Grid.CurrentRow.Cells["Id"].Value;
                var resume = await _context.Resumes.FindAsync(id);
                if (resume == null)
                    return;

                _context.Resumes.Remove(resume);
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

