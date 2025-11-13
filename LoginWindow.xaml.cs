using System.Windows;
using System.Threading.Tasks;
using CarServiceAdminClient.Api;
using System.Text.Json;

namespace CarServiceAdminClient
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Отримуємо єдиний екземпляр ApiClient
            var apiClient = ApiClient.Instance;

            // 2. Створюємо Payload (словник) з логіном і паролем
            var loginData = new Dictionary<string, string>
    {
        { "Login", LoginTextBox.Text },
        { "Password", PasswordBox.Password }
    };

            // 3. Створюємо запит
            var request = new Request
            {
                Command = "LOGIN", // Назва методу на SignalR хабі
                Payload = JsonSerializer.Serialize(loginData) // Аргумент для методу
            };

            // 4. Відправляємо запит на сервер
            try
            {
                // Блокуємо кнопку, поки йде запит
                LoginButton.IsEnabled = false;

                // 4.1. *** НОВЕ: Спершу стартуємо з'єднання ***
                // Якщо воно вже запущено, метод просто пропустить це.
                await apiClient.StartConnectionAsync();

                // 4.2. Відправляємо запит
                var response = await apiClient.SendRequestAsync(request);

                // 5. Аналізуємо відповідь сервера
                if (response.IsSuccess)
                {
                    // Успіх! Сервер дозволив вхід.
                    DialogResult = true;
                }
                else
                {
                    // Невдача! Сервер відхилив вхід.
                    var errorBox = new CustomMessageBox(response.Message, "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Error);
                    errorBox.ShowDialog();
                    LoginButton.IsEnabled = true; // Розблокувати кнопку
                }
            }
            catch (Exception ex)
            {
                // Помилка з'єднання (StartConnectionAsync кинув виняток)
                // (ApiClient вже показав помилку через ShowConnectionError)
                // Просто розблокуємо кнопку
                LoginButton.IsEnabled = true;
            }
        
            // 4. Відправляємо запит на сервер
            // (Використовуємо try/catch на випадок, якщо сервер вимкнено)
            try
            {
                // Блокуємо кнопку, поки йде запит
                LoginButton.IsEnabled = false;
                var response = await apiClient.SendRequestAsync(request);

                // 5. Аналізуємо відповідь сервера
                if (response.IsSuccess)
                {
                    // Успіх! Сервер дозволив вхід.
                    DialogResult = true;
                }
                else
                {
                    // Невдача! Сервер відхилив вхід.
                    var errorBox = new CustomMessageBox(response.Message, "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Error);
                    errorBox.ShowDialog();
                    LoginButton.IsEnabled = true; // Розблокувати кнопку
                }
            }
            catch (Exception ex)
            {
                // Помилка з'єднання (сервер не запущено або інша помилка TCP)
                var errorBox = new CustomMessageBox($"Не вдалося підключитися до сервера.\nПереконайтеся, що сервер запущено.\n\nДеталі: {ex.Message}", "Помилка з'єднання", MessageBoxButton.OK, MessageBoxImage.Error);
                errorBox.ShowDialog();
                LoginButton.IsEnabled = true; // Розблокувати кнопку
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // При натисканні "Вихід" встановлюємо результат на "false".
            // Це сигнал для App.xaml.cs, що користувач хоче закрити програму.
            DialogResult = false;
        }
    }
}

