using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows;
using CarServiceAdminClient;

namespace CarServiceAdminClient
{
    public partial class CustomMessageBox : Window
    {
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        public CustomMessageBox(string message, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow; // Робимо його модальним до головного вікна

            TitleTextBlock.Text = title;
            MessageTextBlock.Text = message;

            // Налаштування іконок
            switch (icon)
            {
                case MessageBoxImage.Information:
                    IconTextBlock.Text = "ℹ️"; // Інформація
                    break;
                case MessageBoxImage.Question:
                    IconTextBlock.Text = "❓"; // Питання
                    break;
                case MessageBoxImage.Warning:
                    IconTextBlock.Text = "⚠️"; // Попередження
                    break;
                case MessageBoxImage.Error:
                    IconTextBlock.Text = "❌"; // Помилка
                    break;
                default:
                    IconTextBlock.Visibility = Visibility.Collapsed;
                    break;
            }

            // Налаштування кнопок
            switch (buttons)
            {
                case MessageBoxButton.OK:
                    OkButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    OkButton.Visibility = Visibility.Collapsed;
                    break;
                    // Можна додати інші комбінації, якщо потрібно
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            this.Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            this.Close();
        }
    }
}
