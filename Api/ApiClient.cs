using Microsoft.AspNetCore.SignalR.Client;
// using Microsoft.AspNetCore.SignalR.Protocols.NewtonsoftJson; // <--- ВИДАЛЕНО (Наш обхід)
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Windows; // Для CustomMessageBox
using System.Collections.Generic; // <--- ДОДАНО (для LoginWindow)

namespace CarServiceAdminClient.Api
{
    // Клас, що відповідає за ВСЮ комунікацію з сервером (SignalR версія)
    public class ApiClient
    {
        // *** ОНОВЛЕНО: Використовуємо 'http' адресу з консолі сервера ***
        private const string ServerUrl = "http://localhost:5116/carservice";

        private readonly HubConnection _connection;

        // --- Singleton Pattern ---
        private static readonly Lazy<ApiClient> _instance = new Lazy<ApiClient>(() => new ApiClient());
        public static ApiClient Instance => _instance.Value;
        // ---

        private ApiClient()
        {
            // 1. Будуємо з'єднання
            _connection = new HubConnectionBuilder()
                .WithUrl(ServerUrl) // Вказуємо оновлений URL
                                    // .AddNewtonsoftJsonProtocol() // <--- ЗАЛИШЕНО ВИДАЛЕНИМ (Наш обхід)
                .Build();

            // *** ДОДАНО: Обробник для автоматичного перепідключення ***
            _connection.Closed += async (error) =>
            {
                // Якщо сервер вимкнувся, чекаємо 5 секунд і пробуємо знову
                await Task.Delay(5000);
                await StartConnectionAsync();
            };
        }

        public async Task StartConnectionAsync()
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                return;
            }

            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                ShowConnectionError(ex.Message);
                throw;
            }
        }

        public async Task<Response> SendRequestAsync(Request request)
        {
            if (_connection.State != HubConnectionState.Connected)
            {
                // Спробуємо перепідключитися, якщо раптом втратили зв'язок
                try
                {
                    await StartConnectionAsync();
                }
                catch
                {
                    // Якщо не вийшло, кидаємо помилку
                    throw new InvalidOperationException("З'єднання з сервером не встановлено.");
                }
            }

            try
            {
                return await _connection.InvokeAsync<Response>(request.Command, request.Payload);
            }
            catch (Exception ex)
            {
                ShowConnectionError(ex.Message);
                return new Response { IsSuccess = false, Message = ex.Message };
            }
        }

        // --- Методи для реєстрації real-time оновлень ---
        public void RegisterClientUpdate(Action<ClientDto> onClientAdded, Action<ClientDto> onClientUpdated, Action<int> onClientDeleted)
        {
            _connection.On("ClientAdded", onClientAdded);
            _connection.On("ClientUpdated", onClientUpdated);
            _connection.On("ClientDeleted", onClientDeleted);
        }

        // (Тут можна додати аналогічні для Car та Order)

        private void ShowConnectionError(string message)
        {
            var errorBox = new CustomMessageBox(
                $"Не вдалося підключитися до сервера.\nПереконайтеся, що сервер запущено.\n\nПомилка: {message}",
                "Помилка з'єднання",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Важливо: Показуємо вікно в потоці UI
            Application.Current?.Dispatcher.Invoke(() =>
            {
                errorBox.ShowDialog();
            });
        }
    }
}