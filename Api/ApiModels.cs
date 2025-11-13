using System;
using System.Collections.Generic;

// Переконайся, що namespace правильний
namespace CarServiceAdminClient.Api
{
    // --- Моделі для спілкування (копії з сервера) ---
    // Тепер вони живуть окремо, і їх бачать всі.

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