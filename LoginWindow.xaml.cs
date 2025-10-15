using System.Windows;

namespace CarServiceAdminClient
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Перевіряємо логін та пароль
            if (LoginTextBox.Text == "admin" && PasswordBox.Password == "1234")
            {
                // Якщо все правильно, встановлюємо результат діалогу на "true"
                // Це сигнал для App.xaml.cs, що вхід успішний.
                DialogResult = true;
            }
            else
            {
                // Якщо дані неправильні, показуємо помилку
                var errorBox = new CustomMessageBox("Неправильний логін або пароль.", "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Error);
                errorBox.ShowDialog();
                // DialogResult залишається без змін (не "true"),
                // тому вікно залишається відкритим.
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // При натисканні "Вихід" встановлюємо результат на "false".
            // Це сигнал для App.xaml.cs, що користувач хоче закрити програму.
            DialogResult = false;
        }
    }
}

