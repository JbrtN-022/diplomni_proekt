using MySql.Data.MySqlClient;
using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для форма__насторойки_подключения_бд.xaml
    /// </summary>
    public partial class форма__насторойки_подключения_бд : Window
    {
        private string staryHost;
        private string staryUser;
        private string staryPassword;
        private string staryBD;
        // Свойства для передачи новых параметров подключения в вызывающий код
        public string NovyyHost { get; private set; }
        public string NovyyUser { get; private set; }
        public string NovyyPassword { get; private set; }
        public string NovayaBD { get; private set; }
        public bool Podkluchenie { get; private set; }

        private string vremennayaStroka;

        public форма__насторойки_подключения_бд(string teckHost,
            string teckUser,
            string teckPassword,
            string teckBD)
        {
            InitializeComponent();
            // Подключение обработчика клавиши F1 для вызова справки
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.F1)
                {
                    ShowHelp();
                    e.Handled = true;
                }
            };// Сохранение текущих настроек
            staryHost = teckHost;
            staryUser = teckUser;
            staryPassword = teckPassword;
            staryBD = teckBD;

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл окно настроек подключения к БД");

            LoadCurrentConnection();
        }
        // Показывает справочное сообщение о форме настроек подключения
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА НАСТРОЙКИ ПОДКЛЮЧЕНИЯ К БАЗЕ ДАННЫХ

Назначение формы:
Настройка параметров подключения к серверу MySQL и выбор базы данных.

Что можно сделать на этой форме:
• Изменить хост (адрес сервера)
• Изменить имя пользователя
• Изменить пароль
• Подключиться к серверу
• Выбрать базу данных из списка доступных
• Проверить подключение к выбранной базе данных

Поля для заполнения:

1. ХОСТ (HOST) (обязательное поле)
   • Адрес сервера MySQL
   • Пример: localhost, 127.0.0.1, или удаленный IP
   • По умолчанию: localhost

2. ПОЛЬЗОВАТЕЛЬ (USER) (обязательное поле)
   • Имя пользователя MySQL
   • Пример: root, admin
   • Должен иметь права доступа к БД

3. ПАРОЛЬ (PASSWORD)
   • Пароль от пользователя MySQL
   • Может быть пустым для некоторых пользователей

Кнопки управления:

1. ПОДКЛЮЧИТЬСЯ К СЕРВЕРУ
   • Устанавливает соединение с MySQL сервером
   • Загружает список доступных баз данных
   • Проверяет правильность введенных данных

2. ВЫБОР БАЗЫ ДАННЫХ
   • Становится активным после подключения к серверу
   • Выпадающий список всех доступных БД

3. ПОДКЛЮЧИТЬСЯ К БД
   • Устанавливает соединение с выбранной базой данных
   • Проверяет наличие необходимых таблиц
   • Сохраняет настройки при успешном подключении

4. ОТМЕНА / НАЗАД
   • Закрывает окно без сохранения изменений
   • Возвращает предыдущие настройки

Процесс настройки:

1. Заполните поля Host, User, Password
2. Нажмите ""Подключиться к серверу""
3. Дождитесь загрузки списка баз данных
4. Выберите нужную базу данных из списка
5. Нажмите ""Подключиться к БД""



Примечание:
При успешном подключении настройки сохраняются и используются в приложении.
Неправильные настройки приведут к ошибке подключения.
Для работы приложения необходима база данных с правильной структурой.
Настройки по умолчанию: host=localhost, user=root, password=qwerty, database=rccs",
                "Помощь - Настройка подключения к БД",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        // Загрузка текущих настроек подключения и списка баз данных
        private void LoadCurrentConnection()
        {
            try
            {
                txtHost.Text = staryHost;
                txtUser.Text = staryUser;
                txtPassword.Text = staryPassword;

                string strokaPodkl = $"server={staryHost};user={staryUser};password={staryPassword};charset=utf8;";

                using (MySqlConnection soed = new MySqlConnection(strokaPodkl))
                {
                    soed.Open();

                    DataTable tablicaBD = soed.GetSchema("Databases");

                    cmbDatabase.Items.Clear();

                    foreach (DataRow dr in tablicaBD.Rows)
                    {
                        string imyaBD = dr["database_name"].ToString();
                        cmbDatabase.Items.Add(imyaBD);
                    }

                    soed.Close();
                }

                vremennayaStroka = strokaPodkl;

                cmbDatabase.IsEnabled = true;
                podkluchKBD.IsEnabled = true;

                if (cmbDatabase.Items.Contains(staryBD))
                {
                    cmbDatabase.SelectedItem = staryBD;
                }

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил текущие настройки подключения к БД");
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"ОШИБКА! Не удалось загрузить текущие настройки БД. Ошибка: {ex.Message}");
            }
        }
        // Кнопка "Отмена" — закрытие без сохранения изменений
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NovyyHost = staryHost;
            NovyyUser = staryUser;
            NovyyPassword = staryPassword;
            NovayaBD = staryBD;
            Podkluchenie = false;

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} отменил изменение подключения к БД");

            this.Close();
        }
        // Подключение к выбранной базе данных
        private void podkluchKBD_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDatabase.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать базу данных!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} попытался подключиться к БД без выбора базы");

                return;
            }

            string vybranayaBD = cmbDatabase.SelectedItem.ToString();

            try
            {
                string polnayaStrika = $"{vremennayaStroka}database={vybranayaBD}";

                using (MySqlConnection soed = new MySqlConnection(polnayaStrika))
                {
                    soed.Open();
                    soed.Close();
                }

                NovyyHost = txtHost.Text.Trim();
                NovyyUser = txtUser.Text.Trim();
                NovyyPassword = txtPassword.Text;
                NovayaBD = vybranayaBD;
                Podkluchenie = true;

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно подключился к базе данных '{vybranayaBD}'");

                MessageBox.Show($"Подключение к базе данных '{vybranayaBD}' прошло успешно!",
                    "Успешное выполнение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог подключиться к базе данных '{vybranayaBD}'. Ошибка: {ex.Message}");

                MessageBox.Show("Ошибка подключения к базе данных!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        // Подключение к серверу MySQL и загрузка списка баз данных
        private void podkluchKServeru_Click(object sender, RoutedEventArgs e)
        {
            string host = txtHost.Text.Trim();
            string user = txtUser.Text.Trim();
            string pass = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user))
            {
                MessageBox.Show("Перед подключением к серверу, необходимо ввести данные!",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} попытался подключиться к серверу без заполнения данных");

                return;
            }

            try
            {
                string strokaPodkl = $"server={host};user={user};password={pass};charset=utf8;";

                using (MySqlConnection soed = new MySqlConnection(strokaPodkl))
                {
                    soed.Open();

                    DataTable tablicaBD = soed.GetSchema("Databases");

                    cmbDatabase.Items.Clear();

                    foreach (DataRow dr in tablicaBD.Rows)
                    {
                        string imyaBD = dr["database_name"].ToString();
                        cmbDatabase.Items.Add(imyaBD);
                    }

                    soed.Close();
                }

                vremennayaStroka = strokaPodkl;

                cmbDatabase.IsEnabled = true;
                podkluchKBD.IsEnabled = true;

                if (cmbDatabase.Items.Contains(staryBD))
                {
                    cmbDatabase.SelectedItem = staryBD;
                }

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно подключился к серверу MySQL");

                MessageBox.Show("Подключение к серверу возможно! Выберите базу данных.",
                    "Успешное подключение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог подключиться к серверу MySQL. Ошибка: {ex.Message}");

                MessageBox.Show("Произошла ошибка подключения к серверу. Вероятно данные введены некорректно",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                cmbDatabase.IsEnabled = false;
                podkluchKBD.IsEnabled = false;
                cmbDatabase.Items.Clear();
            }
        }
        // Кнопка "Назад" — закрытие без сохранения
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NovyyHost = staryHost;
            NovyyUser = staryUser;
            NovyyPassword = staryPassword;
            NovayaBD = staryBD;
            Podkluchenie = false;

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} закрыл окно настроек подключения к БД");

            this.Close();
        }
    }
}