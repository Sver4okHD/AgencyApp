using System;
using System.Windows.Forms;

namespace AgencyApp.Forms.Appointments
{
    /// <summary>
    /// Базовая форма для типичных CRUD-операций с DataGridView.
    /// Содержит общую обработку ошибок.
    /// </summary>
    public class BaseCrudForm : Form
    {
        protected DataGridView Grid;
        protected Button BtnAdd;
        protected Button BtnEdit;
        protected Button BtnDelete;
        protected Button BtnRefresh;

        protected void InitializeBaseComponents(string title)
        {
            Text = title;
            StartPosition = FormStartPosition.CenterParent;
            Width = 800;
            Height = 500;

            Grid = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 320,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                MultiSelect = false
            };
            Controls.Add(Grid);

            var panelButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10)
            };

            BtnAdd = new Button { Text = "Добавить", Width = 100 };
            BtnEdit = new Button { Text = "Изменить", Width = 100 };
            BtnDelete = new Button { Text = "Удалить", Width = 100 };
            BtnRefresh = new Button { Text = "Обновить", Width = 100 };

            panelButtons.Controls.Add(BtnAdd);
            panelButtons.Controls.Add(BtnEdit);
            panelButtons.Controls.Add(BtnDelete);
            panelButtons.Controls.Add(BtnRefresh);

            Controls.Add(panelButtons);
        }

        /// <summary>
        /// Универсальный помощник для отображения ошибок пользователю.
        /// </summary>
        protected void ShowError(Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}

