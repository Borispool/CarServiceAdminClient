using CarServiceAdminClient.Models;

namespace CarServiceAdminClient.ViewModels
{
    public class AddClientViewModel : BaseViewModel
    {
        public Client Client { get; }
        public string Title { get; }

        public string FirstName
        {
            get => Client.FirstName;
            set { Client.FirstName = value; OnPropertyChanged(); }
        }

        public string LastName
        {
            get => Client.LastName;
            set { Client.LastName = value; OnPropertyChanged(); }
        }

        public string PhoneNumber
        {
            get => Client.PhoneNumber;
            set { Client.PhoneNumber = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => Client.Email;
            set { Client.Email = value; OnPropertyChanged(); }
        }

        public AddClientViewModel(Client client)
        {
            Client = client;
            Title = (client.Id == 0) ? "Додати нового Клієнта" : "Редагувати Клієнта";
        }
    }
}