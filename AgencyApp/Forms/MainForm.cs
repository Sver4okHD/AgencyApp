using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace AgencyApp.Forms
{
    /// <summary>
    /// Главное окно приложения с навигацией и простыми модулями анимации/звука.
    /// </summary>
    public class MainForm : Form
    {
        private Button _btnApplicants;
        private Button _btnResumes;
        private Button _btnSkills;
        private Button _btnEmployers;
        private Button _btnVacancies;
        private Button _btnInterviews;
        private Button _btnMatchCandidates;

        private Panel _animationPanel;
        private Timer _animationTimer;
        private int _animationAngle;

        private Button _btnPlaySound;
        private Button _btnRandomObjects;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Кадровое агентство";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 900;
            Height = 600;

            var menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Файл");
            var exitItem = new ToolStripMenuItem("Выход");
            exitItem.Click += (_, _) => Close();
            fileMenu.DropDownItems.Add(exitItem);

            var helpMenu = new ToolStripMenuItem("Справка");
            var aboutItem = new ToolStripMenuItem("О программе");
            aboutItem.Click += (_, _) =>
            {
                MessageBox.Show(
                    "Кадровое агентство\nДемонстрационное WinForms-приложение с использованием EF Core и SQLite.",
                    "О программе",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };
            helpMenu.DropDownItems.Add(aboutItem);

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(helpMenu);
            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);

            var navPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                FlowDirection = FlowDirection.TopDown,
                Width = 200,
                Padding = new Padding(10),
                AutoScroll = true
            };

            _btnApplicants = CreateNavButton("Соискатели", (_, _) => new ApplicantsForm().ShowDialog(this));
            _btnResumes = CreateNavButton("Резюме", (_, _) => new ResumesForm().ShowDialog(this));
            _btnSkills = CreateNavButton("Навыки", (_, _) => new SkillsForm().ShowDialog(this));
            _btnEmployers = CreateNavButton("Работодатели", (_, _) => new EmployersForm().ShowDialog(this));
            _btnVacancies = CreateNavButton("Вакансии", (_, _) => new VacanciesForm().ShowDialog(this));
            _btnInterviews = CreateNavButton("Собеседования", (_, _) => new InterviewsForm().ShowDialog(this));
            _btnMatchCandidates = CreateNavButton("Подбор кандидатов", (_, _) => new MatchCandidatesForm().ShowDialog(this));

            navPanel.Controls.Add(_btnApplicants);
            navPanel.Controls.Add(_btnResumes);
            navPanel.Controls.Add(_btnSkills);
            navPanel.Controls.Add(_btnEmployers);
            navPanel.Controls.Add(_btnVacancies);
            navPanel.Controls.Add(_btnInterviews);
            navPanel.Controls.Add(_btnMatchCandidates);

            Controls.Add(navPanel);

            // Панель анимации (простая "крутилка загрузки")
            _animationPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            _animationPanel.Paint += AnimationPanel_Paint;
            Controls.Add(_animationPanel);

            _animationTimer = new Timer
            {
                Interval = 100
            };
            _animationTimer.Tick += (_, _) =>
            {
                _animationAngle = (_animationAngle + 15) % 360;
                _animationPanel.Invalidate();
            };
            _animationTimer.Start();

            // Кнопка воспроизведения звука
            _btnPlaySound = new Button
            {
                Text = "Воспроизвести звук",
                Width = 160,
                Height = 30,
                Top = 40,
                Left = 220
            };
            _btnPlaySound.Click += (_, _) =>
            {
                // Базовый системный звук (без внешних файлов)
                SystemSounds.Asterisk.Play();
            };
            Controls.Add(_btnPlaySound);

            // Кнопка генерации случайных объектов (анимация фигур)
            _btnRandomObjects = new Button
            {
                Text = "Случайные объекты",
                Width = 160,
                Height = 30,
                Top = 80,
                Left = 220
            };
            _btnRandomObjects.Click += (_, _) => _animationPanel.Invalidate();
            Controls.Add(_btnRandomObjects);
        }

        private Button CreateNavButton(string text, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Width = 160,
                Height = 35,
                Margin = new Padding(3)
            };
            btn.Click += onClick;
            return btn;
        }

        private void AnimationPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int size = 80;
            int x = (_animationPanel.Width - size) / 2;
            int y = (_animationPanel.Height - size) / 2;

            var rect = new Rectangle(x, y, size, size);

            using var pen = new Pen(Color.SteelBlue, 8);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            g.DrawArc(pen, rect, _animationAngle, 270);

            // Дополнительно рисуем несколько случайных кругов при каждом перерисовании
            var rnd = new Random();
            for (int i = 0; i < 5; i++)
            {
                int r = rnd.Next(10, 30);
                int rx = rnd.Next(0, Math.Max(1, _animationPanel.Width - r));
                int ry = rnd.Next(0, Math.Max(1, _animationPanel.Height - r));
                var color = Color.FromArgb(rnd.Next(100, 255), rnd.Next(255), rnd.Next(255), rnd.Next(255));
                using var brush = new SolidBrush(color);
                g.FillEllipse(brush, rx, ry, r, r);
            }
        }
    }
}

