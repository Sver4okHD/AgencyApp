using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgencyApp.Database;
using AgencyApp.Forms;

namespace AgencyApp
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static async Task Main()
        {
            try
            {
                // Автоматическое создание БД при первом запуске
                using (var context = new AgencyContext())
                {
                    context.Database.EnsureCreated();
                    await SeedData.SeedAsync(context);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка инициализации БД:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}

