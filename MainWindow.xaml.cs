using CarServiceAdminClient.ViewModels;
using System.Windows;

namespace CarServiceAdminClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            // Показуємо вікно входу одразу після завантаження головного вікна
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Робимо головне вікно невидимим, поки йде логін
            this.Visibility = Visibility.Hidden;

            var loginWindow = new LoginWindow();
            // Встановлюємо власника, щоб вікно логіну з'явилось по центру головного
            loginWindow.Owner = this;
            bool? result = loginWindow.ShowDialog();

            // Якщо вхід не успішний (користувач закрив вікно або натиснув "Вихід")
            if (result != true)
            {
                // Закриваємо головне вікно (і всю програму)
                this.Close();
            }
            else
            {
                // Якщо вхід успішний, робимо головне вікно видимим
                this.Visibility = Visibility.Visible;
            }
        }
    }
}