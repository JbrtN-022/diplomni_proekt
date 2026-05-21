using MaterialDesignThemes.Wpf;
using rccs.MyClass;
using System;
using System.Windows;
using System.Windows.Input;

namespace rccs_new
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Подсказка по F1
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.F1)
                {
                    ShowHelp();
                    e.Handled = true;
                }
            };

            if (!ConnectionBD.ConnectBD("rccs", "localhost", "root", "qwerty") == true)
            {
                MessageBox.Show("Не удалось подключиться к базе данных!\n\nПриложение будет закрыто.",
                                "Ошибка подключения",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                Application.Current.Shutdown();
            }
        }

        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА АВТОРИЗАЦИИ

Назначение формы:
Форма предназначена для входа в систему с проверкой прав доступа пользователя.

Что можно сделать на этой форме:
• Ввести логин и пароль для входа в систему
• Включить/отключить отображение пароля (глазок/чекбокс)
• Настроить подключение к базе данных

Поля для ввода:
• Логин - ваше уникальное имя пользователя
• Пароль - конфиденциальный ключ доступа (скрыт точками)

Как авторизоваться:
1. Введите ваш логин в поле ""Логин""
2. Введите пароль в поле ""Пароль""
3. При необходимости включите ""Показать пароль"" для проверки ввода
4. Нажмите кнопку ""Авторизоваться""



Дополнительные функции:
• Кнопка ""Настройки подключения"" - позволяет изменить параметры соединения с БД
• Показать пароль - временно отображает вводимый пароль



Примечание:
При неверном вводе логина или пароля система покажет сообщение об ошибке.
При отсутствии подключения к БД приложение закроется автоматически.",
                "Помощь - Форма авторизации",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password;

            if (chkShowPassword.IsChecked == true)
            {
                password = txtVisiblePassword.Text.Trim();
            }
            else
            {
                password = txtPassword.Password.Trim();
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
                        MessageBox.Show("Неверные данные!", "Ошибка",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка авторизации",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void chkShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            txtVisiblePassword.Text = txtPassword.Password;
            txtPassword.Visibility = Visibility.Collapsed;
            txtVisiblePassword.Visibility = Visibility.Visible;
            txtVisiblePassword.Focus();
            txtVisiblePassword.CaretIndex = txtVisiblePassword.Text.Length;
        }

        private void chkShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPassword.Password = txtVisiblePassword.Text;
            txtVisiblePassword.Visibility = Visibility.Collapsed;
            txtPassword.Visibility = Visibility.Visible;
            txtPassword.Focus();
        }

        private void подключение_Click(object sender, RoutedEventArgs e)
        {
            форма__насторойки_подключения_бд openedWindow = null;

            foreach (Window window in Application.Current.Windows)
            {
                if (window is форма__насторойки_подключения_бд)
                {
                    openedWindow = (форма__насторойки_подключения_бд)window;
                    break;
                }
            }

            if (openedWindow == null)
            {
                форма__насторойки_подключения_бд formpodkl =
                    new форма__насторойки_подключения_бд(
                        ConnectionBD.currentDataSource,
                        ConnectionBD.currentUser,
                        ConnectionBD.currentPassword,
                        ConnectionBD.currentDataBase);

                bool? result = formpodkl.ShowDialog();

                if (result == true)
                {
                    if (ConnectionBD.ConnectBD(
                        formpodkl.NovayaBD,
                        formpodkl.NovyyHost,
                        formpodkl.NovyyUser,
                        formpodkl.NovyyPassword))
                    {
                        MessageBox.Show(
                            "Настройки подключения обновлены",
                            "Успешно",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show(
                    "Нельзя открыть больше 1 окна!",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                openedWindow.Activate();
                openedWindow.Focus();
            }
        }
    }
}