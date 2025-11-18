using System.Windows;
using System.Threading.Tasks;
using CarServiceAdminClient.Api;
using Newtonsoft.Json;
using System.Collections.Generic; // Додано для Dictionary

namespace CarServiceAdminClient
{
    public partial class LoginWindow : Window
    {
        // *** НОВА ВЛАСТИВІСТЬ: Зберігаємо ID користувача після успішного входу ***
        public int LoggedInUserId { get; private set; } = 0;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var apiClient = new ApiClient();

            // 1. Створюємо Payload (словник) з логіном і паролем
            var loginData = new Dictionary<string, string>
            {
                { "Login", LoginTextBox.Text },
                { "Password", PasswordBox.Password }
            };

            // 2. Створюємо запит
            var request = new Request
            {
                Command = "LOGIN",
                Payload = JsonConvert.SerializeObject(loginData)
            };

            try
            {
                // Блокуємо кнопку, поки йде запит
                LoginButton.IsEnabled = false;
                var response = await apiClient.SendRequestAsync(request);

                // 3. Аналізуємо відповідь сервера
                if (response.IsSuccess)
                {
                    // Успіх! Сервер дозволив вхід.
                    if (int.TryParse(response.Payload, out int userId))
                    {
                        LoggedInUserId = userId; // Зберігаємо ID
                        DialogResult = true;
                    }
                    else
                    {
                        // Якщо ID не повернувся, це помилка
                        throw new Exception("Сервер не повернув ID користувача.");
                    }
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
                // Помилка з'єднання
                var errorBox = new CustomMessageBox($"Не вдалося підключитися до сервера.\nПереконайтеся, що сервер запущено.\n\nДеталі: {ex.Message}", "Помилка з'єднання", MessageBoxButton.OK, MessageBoxImage.Error);
                errorBox.ShowDialog();
                LoginButton.IsEnabled = true; // Розблокувати кнопку
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}