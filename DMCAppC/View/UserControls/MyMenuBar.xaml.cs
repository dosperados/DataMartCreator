using System.Windows.Controls;

namespace DMCApp.View.UserControls
{
    /// <summary>
    /// Interaction logic for MainMenuBar.xaml
    /// </summary>
    public partial class MyMenuBar : UserControl
    {
        public MyMenuBar()
        {
            InitializeComponent();

            /*
             // При клике на кнопку выбора другой БД
private void OnChangeConnectionButtonClick(object sender, RoutedEventArgs e)
{
    App.CurrentConnectionString = App.Configuration.GetConnectionString("DestDbWindowsAuth");

    using var context = new AppDbContext();
    var tables = context.Tables.ToList();

    // Пример вывода
    MessageBox.Show($"Найдено таблиц: {tables.Count}");
}
             */
        }
    }
}
