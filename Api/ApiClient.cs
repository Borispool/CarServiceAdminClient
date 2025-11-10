using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows; // Для MessageBox

namespace CarServiceAdminClient.Api
{
    // Клас, що відповідає за ВСЮ комунікацію з сервером
    public class ApiClient
    {
        private const string ServerAddress = "127.0.0.1"; // Адреса сервера (localhost)
        private const int ServerPort = 8888;             // Порт сервера

        // Головний асинхронний метод для відправки запитів
        public async Task<Response> SendRequestAsync(Request request)
        {
            try
            {
                // 1. Конвертуємо наш об'єкт запиту в JSON-рядок
                string jsonRequest = JsonConvert.SerializeObject(request);
                byte[] data = Encoding.UTF8.GetBytes(jsonRequest);

                // 2. Створюємо нове TCP-підключення
                using (TcpClient client = new TcpClient())
                {
                    // Підключаємось до сервера
                    await client.ConnectAsync(ServerAddress, ServerPort);

                    // 3. Отримуємо потік для читання/запису
                    using (NetworkStream stream = client.GetStream())
                    {
                        // 4. Відправляємо дані (JSON) на сервер
                        await stream.WriteAsync(data, 0, data.Length);

                        // 5. Отримуємо відповідь від сервера
                        byte[] buffer = new byte[8192]; // Збільшимо буфер про всяк випадок
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        string jsonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        // 6. Конвертуємо JSON-відповідь назад в об'єкт Response
                        return JsonConvert.DeserializeObject<Response>(jsonResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                // Якщо сервер впав або недоступний, покажемо помилку
                ShowConnectionError(ex.Message);
                return new Response { IsSuccess = false, Message = ex.Message };
            }
        }

        private void ShowConnectionError(string message)
        {
            // Використовуємо наш кастомний MessageBox
            var errorBox = new CustomMessageBox(
                $"Не вдалося підключитися до сервера.\nПереконайтеся, що сервер запущено.\n\nПомилка: {message}",
                "Помилка з'єднання",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            errorBox.ShowDialog();
        }
    }

    // --- Моделі для спілкування (копії з сервера) ---
    // Нам потрібні ці "чисті" класи для серіалізації/десеріалізації

    public class Request
    {
        public string Command { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }

    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }

    // Клас-контейнер для завантаження всіх даних
    public class AllDataDto
    {
        public List<ClientDto> Clients { get; set; } = new List<ClientDto>();
        public List<CarDto> Cars { get; set; } = new List<CarDto>();
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
    }

    // DTO (Data Transfer Object) - "чисті" версії твоїх моделей
    // без логіки BaseViewModel

    public class ClientDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class CarDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;
        public int Year { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public bool IsRecurringProblem { get; set; }
        public decimal Cost { get; set; }
    }
}