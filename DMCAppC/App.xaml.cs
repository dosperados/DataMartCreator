using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace DMCApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Построение конфигурации
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Указывает, где искать файл
                .AddJsonFile("settings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // Теперь Configuration.GetConnectionString("имя_строки") доступно во всем приложении
            // Например, при создании главного окна и его ViewModel:
            // var mainWindow = new MainWindow();
            // var viewModel = new MainViewModel(); // ViewModel сам создаст DbContext, используя App.Configuration
            // mainWindow.DataContext = viewModel;
            // mainWindow.Show();
        }
    }

}
