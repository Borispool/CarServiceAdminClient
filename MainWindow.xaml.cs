using CarServiceAdminClient.ViewModels;
using System.Windows;

namespace CarServiceAdminClient
{
    public partial class MainWindow : Window
    {
        // *** НОВЕ ***
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // *** ЗМІНЕНО ***
            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            // Показуємо вікно входу одразу після завантаження головного вікна
            this.Loaded += MainWindow_Loaded;
        }

        // *** ЗМІНЕНО: Метод тепер 'async' ***
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Робимо головне вікно невидимим, поки йде логін
            this.Visibility = Visibility.Hidden;

            var loginWindow = new LoginWindow();
            loginWindow.Owner = this;
            bool? result = loginWindow.ShowDialog();

            if (result != true)
            {
                this.Close();
            }
            else
            {
                // Якщо вхід успішний...

                // *** НОВЕ: Асинхронно завантажуємо дані з сервера ***
                await _viewModel.InitializeAsync();

                // ...тільки ПІСЛЯ завантаження даних робимо вікно видимим
                this.Visibility = Visibility.Visible;
            }
        }
    }
}