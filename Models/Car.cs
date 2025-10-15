using CarServiceAdminClient.ViewModels;

namespace CarServiceAdminClient.Models
{
    // Наслідуємось від BaseViewModel, щоб отримати магію сповіщень про зміни
    public class Car : BaseViewModel
    {
        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private int _clientId;
        public int ClientId
        {
            get => _clientId;
            set { _clientId = value; OnPropertyChanged(); }
        }

        private string _brand = string.Empty;
        public string Brand
        {
            get => _brand;
            set { _brand = value; OnPropertyChanged(); }
        }

        private string _model = string.Empty;
        public string Model
        {
            get => _model;
            set { _model = value; OnPropertyChanged(); }
        }

        private string _vin = string.Empty;
        public string VIN
        {
            get => _vin;
            set { _vin = value; OnPropertyChanged(); }
        }

        private int _year;
        public int Year
        {
            get => _year;
            set { _year = value; OnPropertyChanged(); }
        }
    }
}