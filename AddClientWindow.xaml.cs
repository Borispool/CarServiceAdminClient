using CarServiceAdminClient.Models;
using CarServiceAdminClient.ViewModels;
using System.Windows;

namespace CarServiceAdminClient
{
    public partial class AddClientWindow : Window
    {
        public AddClientWindow(Client client)
        {
            InitializeComponent();
            // Створюємо та прив'язуємо ViewModel
            DataContext = new AddClientViewModel(client);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валідація тепер перевіряє дані з ViewModel
            if (DataContext is AddClientViewModel vm &&
                !string.IsNullOrWhiteSpace(vm.FirstName) &&
                !string.IsNullOrWhiteSpace(vm.LastName) &&
                !string.IsNullOrWhiteSpace(vm.PhoneNumber))
            {
                DialogResult = true;
            }
            else
            {
                var errorBox = new CustomMessageBox("Будь ласка, заповніть поля: Ім'я, Прізвище та Телефон.", "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
                errorBox.ShowDialog();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
