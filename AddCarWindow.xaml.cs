using CarServiceAdminClient.Models;
using CarServiceAdminClient.ViewModels;
using System.Windows;

namespace CarServiceAdminClient
{
    public partial class AddCarWindow : Window
    {
        public AddCarWindow(Car car)
        {
            InitializeComponent();
            // Створюємо та прив'язуємо ViewModel
            DataContext = new AddCarViewModel(car);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Тепер валідація трохи простіша
            if (DataContext is AddCarViewModel vm &&
                !string.IsNullOrWhiteSpace(vm.Brand) &&
                !string.IsNullOrWhiteSpace(vm.Model) &&
                !string.IsNullOrWhiteSpace(vm.VIN) &&
                vm.Year > 1900)
            {
                DialogResult = true;
            }
            else
            {
                var errorBox = new CustomMessageBox("Будь ласка, заповніть усі поля з зірочкою (*) коректно.", "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
                errorBox.ShowDialog();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
