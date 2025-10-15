using CarServiceAdminClient.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CarServiceAdminClient.ViewModels
{
    public class AddCarViewModel : BaseViewModel
    {
        // Словник для зберігання марок та їх моделей
        private readonly Dictionary<string, List<string>> _carData = new Dictionary<string, List<string>>
        {
            { "Toyota", new List<string> { "Camry", "Corolla", "RAV4", "Land Cruiser", "Highlander" } },
            { "Volkswagen", new List<string> { "Golf", "Passat", "Tiguan", "Touareg", "Polo" } },
            { "Ford", new List<string> { "Focus", "Mustang", "Explorer", "Fiesta", "Kuga" } },
            { "Skoda", new List<string> { "Octavia", "Superb", "Kodiaq", "Fabia", "Kamiq" } },
            { "BMW", new List<string> { "3 Series", "5 Series", "X3", "X5", "7 Series" } },
            { "Mercedes-Benz", new List<string> { "C-Class", "E-Class", "S-Class", "GLE", "GLC" } },
            { "Audi", new List<string> { "A4", "A6", "Q5", "Q7", "A8" } },
            { "Hyundai", new List<string> { "Sonata", "Tucson", "Santa Fe", "Elantra", "Kona" } },
            { "Kia", new List<string> { "Sportage", "Optima", "Sorento", "Ceed", "Rio" } },
            { "Lexus", new List<string> { "RX", "ES", "NX", "LX", "GX" } }
        };

        public Car Car { get; }
        public string Title { get; }

        public ObservableCollection<string> AllBrands { get; }
        public ObservableCollection<string> AvailableModels { get; } = new ObservableCollection<string>();

        private string _brand;
        public string Brand
        {
            get => _brand;
            set
            {
                if (_brand != value)
                {
                    _brand = value;
                    Car.Brand = value; // Оновлюємо модель
                    OnPropertyChanged();
                    UpdateModels(); // Оновлюємо список моделей
                }
            }
        }

        public string Model { get => Car.Model; set { Car.Model = value; OnPropertyChanged(); } }
        public string VIN { get => Car.VIN; set { Car.VIN = value; OnPropertyChanged(); } }
        public int Year { get => Car.Year; set { Car.Year = value; OnPropertyChanged(); } }

        public AddCarViewModel(Car car)
        {
            Car = car;
            Title = (car.Id == 0) ? "Додати новий Автомобіль" : "Редагувати Автомобіль";

            AllBrands = new ObservableCollection<string>(_carData.Keys.OrderBy(b => b));

            // Ініціалізація полів
            _brand = car.Brand;
            UpdateModels();
        }

        private void UpdateModels()
        {
            AvailableModels.Clear();
            if (!string.IsNullOrEmpty(Brand) && _carData.ContainsKey(Brand))
            {
                foreach (var model in _carData[Brand])
                {
                    AvailableModels.Add(model);
                }
            }
        }
    }
}