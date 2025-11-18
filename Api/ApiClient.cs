using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading; // *** НОВЕ: Для Task.Delay ***
using System.Collections.Generic;

namespace CarServiceAdminClient.Api
{
    // Клас, що відповідає за ВСЮ комунікацію з сервером
    public class ApiClient
    {
        private const string ServerAddress = "127.0.0.1"; // Адреса сервера (localhost)
        private const int ServerPort = 8888;             // Порт сервера
        // *** НОВЕ: Пауза між запитами, щоб уникнути Connection Refused у TCP ***
        private const int TcpDelayMs = 100;

        // Головний асинхронний метод для відправки запитів
        public async Task<Response> SendRequestAsync(Request request)
        {
            try
            {
                string jsonRequest = JsonConvert.SerializeObject(request);
                byte[] data = Encoding.UTF8.GetBytes(jsonRequest);

                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(ServerAddress, ServerPort);

                    using (NetworkStream stream = client.GetStream())
                    {
                        await stream.WriteAsync(data, 0, data.Length);

                        byte[] buffer = new byte[8192];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        string jsonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        // 6. Конвертуємо JSON-відповідь назад в об'єкт Response
                        var response = JsonConvert.DeserializeObject<Response>(jsonResponse);

                        // *** НОВЕ: Робимо паузу перед поверненням ***
                        // Це дає TCP-стеку час звільнити порт.
                        await Task.Delay(TcpDelayMs);

                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                // Якщо сервер впав або недоступний, покажемо помилку
                ShowConnectionError(ex.Message);
                // Робимо паузу і тут, щоб не спамити спробами з'єднання
                await Task.Delay(TcpDelayMs);
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
    // (Твої існуючі DTO залишаються без змін)
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

    public class AllDataDto
    {
        public List<ClientDto> Clients { get; set; } = new List<ClientDto>();
        public List<CarDto> Cars { get; set; } = new List<CarDto>();
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
    }

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