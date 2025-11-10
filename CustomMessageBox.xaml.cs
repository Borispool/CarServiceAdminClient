using System.Windows;

namespace CarServiceAdminClient
{
    public partial class CustomMessageBox : Window
    {

        public MessageBoxResult CustomResult { get; private set; } = MessageBoxResult.None; // Зберігаємо результат для Yes/No/Cancel

        public CustomMessageBox(string message, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;

            TitleTextBlock.Text = title;
            MessageTextBlock.Text = message;

            // Налаштування іконок (без змін)
            switch (icon)
            {
                case MessageBoxImage.Information: IconTextBlock.Text = "ℹ️"; break;
                case MessageBoxImage.Question: IconTextBlock.Text = "❓"; break;
                case MessageBoxImage.Warning: IconTextBlock.Text = "⚠️"; break;
                case MessageBoxImage.Error: IconTextBlock.Text = "❌"; break;
                default: IconTextBlock.Visibility = Visibility.Collapsed; break;
            }

            // Налаштування кнопок (без змін)
            switch (buttons)
            {
                case MessageBoxButton.OK: OkButton.Visibility = Visibility.Visible; break;
                case MessageBoxButton.YesNo:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    OkButton.Visibility = Visibility.Collapsed; // Ховаємо OK для Yes/No
                    break;
                    // Можна додати інші комбінації
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // *** ЗМІНЕНО: Встановлюємо стандартний DialogResult ***
            this.DialogResult = true;
            CustomResult = MessageBoxResult.OK; // Зберігаємо тип кнопки
            // this.Close(); // Close викликається автоматично при встановленні DialogResult
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            // *** ЗМІНЕНО: Встановлюємо стандартний DialogResult ***
            this.DialogResult = true;
            CustomResult = MessageBoxResult.Yes; // Зберігаємо тип кнопки
            // this.Close(); // Close викликається автоматично
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            // *** ЗМІНЕНО: Встановлюємо стандартний DialogResult ***
            this.DialogResult = false; // ShowDialog() поверне false
            CustomResult = MessageBoxResult.No; // Зберігаємо тип кнопки
            // this.Close(); // Close викликається автоматично
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // *** ЗМІНЕНО: Встановлюємо стандартний DialogResult ***
            this.DialogResult = false; // ShowDialog() поверне false
            CustomResult = MessageBoxResult.Cancel; // Зберігаємо тип кнопки
                                                    // this.Close(); // Close викликається автоматично
        }
    }
}