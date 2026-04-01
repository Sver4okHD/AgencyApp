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
    /// Форма для управления навыками.
    /// </summary>
    public class SkillsForm : BaseCrudForm
    {
        private TextBox _txtName;
        private TextBox _txtCategory;
        private AgencyContext _context;

        public SkillsForm()
        {
            InitializeBaseComponents("Навыки");
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
                RowCount = 2,
                Padding = new Padding(10)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var lblName = new Label { Text = "Название:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
            var lblCategory = new Label { Text = "Категория:", Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };

            _txtName = new TextBox { Dock = DockStyle.Fill };
            _txtCategory = new TextBox { Dock = DockStyle.Fill };

            panel.Controls.Add(lblName, 0, 0);
            panel.Controls.Add(_txtName, 1, 0);
            panel.Controls.Add(lblCategory, 0, 1);
            panel.Controls.Add(_txtCategory, 1, 1);

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
                var data = await _context.Skills.AsNoTracking().ToListAsync();
                Grid.DataSource = data.Select(s => new { s.Id, s.Name, s.CategoryDescription }).ToList();
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
                var skill = new Skill
                {
                    Name = _txtName.Text.Trim(),
                    CategoryDescription = _txtCategory.Text.Trim()
                };
                _context.Skills.Add(skill);
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
                var skill = await _context.Skills.FindAsync(id);
                if (skill == null)
                    return;

                skill.Name = _txtName.Text.Trim();
                skill.CategoryDescription = _txtCategory.Text.Trim();

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

            if (MessageBox.Show("Удалить выбранный навык?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                int id = (int)Grid.CurrentRow.Cells["Id"].Value;
                var skill = await _context.Skills.FindAsync(id);
                if (skill == null)
                    return;

                _context.Skills.Remove(skill);
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

