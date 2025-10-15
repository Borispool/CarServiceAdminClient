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
            if (DataContext is Order order && !string.IsNullOrWhiteSpace(order.Description))
            {
                DialogResult = true;
            }
            else
            {
                var errorBox = new CustomMessageBox("Будь ласка, заповніть опис проблеми.", "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
                errorBox.ShowDialog();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

