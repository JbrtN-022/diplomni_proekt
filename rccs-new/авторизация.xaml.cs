using MaterialDesignThemes.Wpf;
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
                        форма_менеджера formMEN = new форма_менеджера();
                        Application.Current.MainWindow = formMEN;
                        formMEN.Show();

                        this.Close();
                        break;
                    case "2":
                        форма_бухгалтера formBuch = new форма_бухгалтера();
                        Application.Current.MainWindow = formBuch;
                        formBuch.Show();

                        this.Close();
                        break;
                    case "3":
                        форма_администратора formadm = new форма_администратора();
                        Application.Current.MainWindow = formadm;
                        formadm.Show();

                        this.Close();
                        break;
                    default:
                        MessageBox.Show("Неверные данные!");
                        break;
                }
            }
        }

        private void chkShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            bool isChecked = chkShowPassword.IsChecked == true;

            
            var parent = txtPassword.Parent as Grid;
            if (parent == null) return;

            if (isChecked)
            {
                
                _visiblePasswordBox = new TextBox
                {
                    Text = txtPassword.Password,
                    Height = 50,
                    FontSize = 15,
                    Padding = new Thickness(10, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = txtPassword.Background,
                    BorderBrush = txtPassword.BorderBrush,
                    BorderThickness = txtPassword.BorderThickness,

                   
                };


                int index = parent.Children.IndexOf(txtPassword);
                parent.Children.Remove(txtPassword);
                parent.Children.Insert(index, _visiblePasswordBox);

                _visiblePasswordBox.Focus();
                _visiblePasswordBox.CaretIndex = _visiblePasswordBox.Text.Length;
            }
            else
            {

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

        private void chkShowPassword_Click(object sender, RoutedEventArgs e)
        {
            bool show = chkShowPassword.IsChecked == true;

            var parentGrid = txtPassword.Parent as Grid;
            if (parentGrid == null) return;

            if (show)
            {
                // Показываем пароль как обычный TextBox
                _visiblePasswordBox = new TextBox
                {
                    Text = txtPassword.Password,
                    Height = txtPassword.Height,
                    FontSize = txtPassword.FontSize,
                    Padding = txtPassword.Padding,
                    VerticalAlignment = txtPassword.VerticalAlignment,
                    HorizontalAlignment = txtPassword.HorizontalAlignment,
                    Background = txtPassword.Background,
                    BorderBrush = txtPassword.BorderBrush,
                    BorderThickness = txtPassword.BorderThickness,
                    FontFamily = txtPassword.FontFamily,
                    Margin = txtPassword.Margin
                };

                int index = parentGrid.Children.IndexOf(txtPassword);
                parentGrid.Children.Remove(txtPassword);
                parentGrid.Children.Insert(index, _visiblePasswordBox);

                _visiblePasswordBox.Focus();
                _visiblePasswordBox.CaretIndex = _visiblePasswordBox.Text.Length;
            }
            else
            {
                // Возвращаем PasswordBox обратно
                if (_visiblePasswordBox != null)
                {
                    txtPassword.Password = _visiblePasswordBox.Text;

                    int index = parentGrid.Children.IndexOf(_visiblePasswordBox);
                    parentGrid.Children.Remove(_visiblePasswordBox);
                    parentGrid.Children.Insert(index, txtPassword);

                    _visiblePasswordBox = null;
                    txtPassword.Focus();
                }
            }
        }
    }
}
