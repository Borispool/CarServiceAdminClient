using CarServiceAdminClient.ViewModels;
using System.Windows;
using System.Threading.Tasks; // Додано для асинхронних операцій

namespace CarServiceAdminClient
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
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
                // *** НОВЕ: Зберігаємо ID користувача та запускаємо Heartbeat ***
                int userId = loginWindow.LoggedInUserId;
                _viewModel.StartHeartbeat(userId); // Запускаємо періодичну відправку пульсу на сервер

                // *** Завантажуємо дані ***
                await _viewModel.InitializeAsync();

                this.Visibility = Visibility.Visible;
            }
        }

        // *** НОВИЙ МЕТОД: Перехоплення закриття вікна ***
        // Це критично важливо, щоб зняти мітку часу (Logout)
        private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 1. Запобігаємо негайному закриттю, доки ми не відправимо LOGOUT
            e.Cancel = true;

            // 2. Викликаємо очищення (що відправить LOGOUT)
            await _viewModel.CleanupAsync();

            // 3. Знімаємо запобіжник і закриваємо вікно остаточно
            e.Cancel = false;

            // Якщо ми тут, програма сама закриється.
        }
    }
}