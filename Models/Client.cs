using CarServiceAdminClient.ViewModels; // Додаємо, щоб отримати доступ до BaseViewModel

namespace CarServiceAdminClient.Models
{
    // Наслідуємось від BaseViewModel, щоб отримати магію сповіщень
    public class Client : BaseViewModel
    {
        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); OnPropertyChanged(nameof(FullName)); }
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); OnPropertyChanged(nameof(FullName)); }
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string FullName => $"{LastName} {FirstName}";
    }
}
