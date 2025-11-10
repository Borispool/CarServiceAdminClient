using CarServiceAdminClient.Models;
using System.Windows;

namespace CarServiceAdminClient
{
    public partial class AddOrderWindow : Window
    {
        public AddOrderWindow(Order order)
        {
            InitializeComponent();
            DataContext = order;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Перевіряємо, чи DataContext є об'єктом Order
            if (DataContext is Order order)
            {
                // 1. Перевіряємо опис
                if (string.IsNullOrWhiteSpace(order.Description))
                {
                    var errorBox = new CustomMessageBox("Будь ласка, заповніть опис проблеми.", "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
                    errorBox.ShowDialog();
                    return; // Виходимо, якщо опис порожній
                }

                // 2. Перевіряємо вартість
                if (order.Cost <= 0)
                {
                    var errorBox = new CustomMessageBox("Будь ласка, вкажіть коректну вартість (більше 0).", "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
                    errorBox.ShowDialog();
                    return; // Виходимо, якщо вартість некоректна
                }
                DialogResult = true;
            }
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

