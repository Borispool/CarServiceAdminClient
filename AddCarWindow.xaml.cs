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
            try
            {
                if (DataContext is AddCarViewModel vm)
                {
                    
                    if (string.IsNullOrWhiteSpace(vm.Brand) ||
                        string.IsNullOrWhiteSpace(vm.Model) ||
                        string.IsNullOrWhiteSpace(vm.VIN))
                    {
                        var errorBox = new CustomMessageBox("Будь ласка, заповніть усі поля з зірочкою (*).", "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
                        errorBox.ShowDialog();
                        return; 
                    }

                    
                    if (vm.Year <= 1900 || vm.Year > DateTime.Now.Year)
                    {
                        var errorBox = new CustomMessageBox($"Рік випуску має бути коректним (наприклад, від 1901 до {DateTime.Now.Year}).", "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
                        errorBox.ShowDialog();
                        return; 
                    }

                    
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                
                var errorBox = new CustomMessageBox($"Сталася помилка: {ex.Message}. Перевірте введені дані.", "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                errorBox.ShowDialog();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
