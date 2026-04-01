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
    /// Форма для управления работодателями.
    /// </summary>
    public class EmployersForm : BaseCrudForm
    {
        private TextBox _txtName;
        private TextBox _txtContacts;
        private TextBox _txtDescription;
        private AgencyContext _context;

        public EmployersForm()
        {
            InitializeBaseComponents("Работодатели");
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
                RowCount = 3,
                Padding = new Padding(10)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var lblName = new Label { Text = "Компания:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblContacts = new Label { Text = "Контакты:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblDescription = new Label { Text = "Описание:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };

            _txtName = new TextBox { Dock = DockStyle.Fill };
            _txtContacts = new TextBox { Dock = DockStyle.Fill };
            _txtDescription = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 60 };

            panel.Controls.Add(lblName, 0, 0);
            panel.Controls.Add(_txtName, 1, 0);
            panel.Controls.Add(lblContacts, 0, 1);
            panel.Controls.Add(_txtContacts, 1, 1);
            panel.Controls.Add(lblDescription, 0, 2);
            panel.Controls.Add(_txtDescription, 1, 2);

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
                var data = await _context.Employers.AsNoTracking().ToListAsync();
                Grid.DataSource = data.Select(e => new { e.Id, e.CompanyName, e.ContactInfo, e.Description }).ToList();
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
                var employer = new Employer
                {
                    CompanyName = _txtName.Text.Trim(),
                    ContactInfo = _txtContacts.Text.Trim(),
                    Description = _txtDescription.Text.Trim()
                };
                _context.Employers.Add(employer);
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
                var employer = await _context.Employers.FindAsync(id);
                if (employer == null)
                    return;

                employer.CompanyName = _txtName.Text.Trim();
                employer.ContactInfo = _txtContacts.Text.Trim();
                employer.Description = _txtDescription.Text.Trim();

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

            if (MessageBox.Show("Удалить выбранного работодателя?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                int id = (int)Grid.CurrentRow.Cells["Id"].Value;
                var employer = await _context.Employers.FindAsync(id);
                if (employer == null)
                    return;

                _context.Employers.Remove(employer);
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

