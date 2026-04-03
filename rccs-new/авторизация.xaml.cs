using rccs.MyClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextBox _visiblePasswordBox;
        public MainWindow()
        {
            InitializeComponent();
            if (!ConnectionBD.ConnectBD())
            {
                MessageBox.Show("Не удалось подключиться к базе данных!\n\nПриложение будет закрыто.",
                                "Ошибка подключения",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                Application.Current.Shutdown(); 
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (chkShowPassword.IsChecked == true && _visiblePasswordBox != null)
            {
                password = _visiblePasswordBox.Text.Trim();
            }

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Внимание",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            Autorization.AutorizationBD(login, password);
            Autorization.GetFioUser(login, password);

            if (!string.IsNullOrEmpty(ConnectionBD.roll))
            {
                
                switch (ConnectionBD.roll)
                {
                    case null:
                        MessageBox.Show("Неверные данные!");
                        break;
                    case "1":

                        break;
                    case "2":

                        break;
                    case "3":
                        форма_администратора formadm = new форма_администратора();
                        Application.Current.MainWindow = formadm;
                        formadm.Show();

                        this.Close();
                        break;
                }
            }
        }

        private void chkShowPassword_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkShowPassword_Click(object sender, RoutedEventArgs e)
        {
            bool showPassword = chkShowPassword.IsChecked == true;
            var parent = txtPassword.Parent as Grid;

            if (parent == null) return;

            if (showPassword)
            {
                // Создаём TextBox с правильным стилем
                _visiblePasswordBox = new TextBox
                {
                    Text = txtPassword.Password,
                    Style = (Style)FindResource("MaterialDesignOutlinedTextBox"),
                    FontSize = 15,
                    Height = 50,
                    Margin = new Thickness(0),
                    Padding = new Thickness(10, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                };

                // Заменяем PasswordBox на TextBox
                int index = parent.Children.IndexOf(txtPassword);
                parent.Children.Remove(txtPassword);
                parent.Children.Insert(index, _visiblePasswordBox);

                _visiblePasswordBox.Focus();
                _visiblePasswordBox.CaretIndex = _visiblePasswordBox.Text.Length;
            }
            else
            {
                // Возвращаем обратно PasswordBox
                if (_visiblePasswordBox != null)
                {
                    txtPassword.Password = _visiblePasswordBox.Text;

                    int index = parent.Children.IndexOf(_visiblePasswordBox);
                    parent.Children.Remove(_visiblePasswordBox);
                    parent.Children.Insert(index, txtPassword);

                    _visiblePasswordBox = null;
                    txtPassword.Focus();
                }
            }
        }
    }
}
