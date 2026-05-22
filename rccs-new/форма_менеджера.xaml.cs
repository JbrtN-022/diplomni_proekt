using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для форма_менеджера.xaml
    /// </summary>
    public partial class форма_менеджера : Window
    {
        public форма_менеджера()
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
            };

            // Установка приветствия в заголовке окна
            if (ConnectionBD.resFio.Length == 0)
            {
                title.Text = "ошибка";
            }
            title.Text = $@"С возвращением, {ConnectionBD.resFio}";

            // Логирование входа пользователя
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} авторизовался как менеджер");
        }

        // Показывает справочное сообщение о форме менеджера
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА МЕНЕДЖЕРА
Назначение формы:
Главная панель управления для менеджера по работе с контрагентами и лицензиями.
Что можно сделать на этой форме:
• Управлять контрагентами
• Просматривать лицензии
• Оформлять новые лицензии
• Просматривать данные текущего пользователя
• Выход из системы
Доступные разделы:
1. КОНТРАГЕНТЫ
   • Просмотр списка контрагентов
   • Добавление новых контрагентов
   • Редактирование данных контрагентов
   • Поиск и фильтрация контрагентов
2. ПРОСМОТР ЛИЦЕНЗИЙ
   • Просмотр всех оформленных лицензий
   • Поиск лицензий по различным критериям
   • Просмотр детальной информации по лицензиям
3. ОФОРМЛЕНИЕ ЛИЦЕНЗИИ
   • Создание новых лицензий
   • Выбор контрагента
   • Выбор программного обеспечения
   • Установка сроков действия
   • Расчет стоимости
4. ДАННЫЕ ПОЛЬЗОВАТЕЛЯ
   • Просмотр информации о текущем пользователе
5. НАСТРОЙКИ ПОДКЛЮЧЕНИЯ
   • Изменение параметров подключения к базе данных
   • Смена сервера, пользователя, пароля
6. ВЫХОД
   • Завершение текущей сессии и возврат к форме авторизации
",
                "Помощь - Форма менеджера",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // Открытие формы контрагентов
        private void контрагенты_Click(object sender, RoutedEventArgs e)
        {
            форма_контрагентов spravochnik = new форма_контрагентов();
            Application.Current.MainWindow = spravochnik;
            spravochnik.Show();
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму контрагентов");
            this.Close();
        }

        // Выход из системы и возврат на форму авторизации
        private void выйти_Click(object sender, RoutedEventArgs e)
        {
            MainWindow autoriz = new MainWindow();
            Application.Current.MainWindow = autoriz;
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} вышел из профиля");
            autoriz.Show();
            this.Close();
            ConnectionBD.roll = null;
            ConnectionBD.resFio = null;
        }

        // Открытие/печать формы заявки на лицензию
        private void печать_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} нажал кнопку печати");

            try
            {
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = Path.GetFullPath(Path.Combine(exeDirectory, "..", ".."));
                string docPath = Path.Combine(projectDirectory, "document");
                string templatePath = Path.Combine(docPath, "ФОРМА ЗАЯВКИ КЛИЕНТА НА ПРИОБРЕТЕНИЕ ЛИЦЕНЗИИ НА ПРОГРАММНОЕ ОБЕСПЕЧЕНИЕ.docx");

                if (!File.Exists(templatePath))
                {
                    MessageBox.Show($"Файл шаблона не найден:\n{templatePath}", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBoxResult result = MessageBox.Show(
                    "Открыть форму заявки для просмотра?",
                    "Выбор действия",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    string tempFile = Path.Combine(Path.GetTempPath(), $"Форма_заявки_{DateTime.Now:yyyyMMdd_HHmmss}.docx");
                    File.Copy(templatePath, tempFile, true);
                    Process.Start(tempFile);
                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму заявки для просмотра");
                }
                else
                {
                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} отправил форму заявки на печать");
                }
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"Ошибка при открытии формы заявки: {ex.Message}");
                MessageBox.Show($"Ошибка при открытии документа:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Открытие формы данных пользователя
        private void пользователь_Click(object sender, RoutedEventArgs e)
        {
            UserOverlap overlap = new UserOverlap();
            Application.Current.MainWindow = overlap;
            overlap.Show();
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл окно пользователя");
            this.Close();
        }

        // Открытие формы просмотра лицензий
        private void viewingLicense_Click(object sender, RoutedEventArgs e)
        {
            просмотр_лицензий prosmotrlicens = new просмотр_лицензий();
            Application.Current.MainWindow = prosmotrlicens;
            prosmotrlicens.Show();
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл просмотр лицензий");
            this.Close();
        }

        // Открытие формы оформления новой лицензии
        private void addLicense_Click(object sender, RoutedEventArgs e)
        {
            Форма_оформления_лицензии oformlenieLicens = new Форма_оформления_лицензии();
            Application.Current.MainWindow = oformlenieLicens;
            oformlenieLicens.Show();
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму оформления лицензии");
            this.Close();
        }

        // Открытие окна настроек подключения к БД
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
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму подключения к БД");

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
                        MessageBox.Show("Настройки подключения обновлены", "Успешно",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                        HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} обновил настройки подключения к БД");
                    }
                    else
                    {
                        HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог обновить подключение к БД");
                    }
                }
                else
                {
                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} закрыл окно подключения к БД без изменений");
                }
            }
            else
            {
                MessageBox.Show("Нельзя открыть больше 1 окна!", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                HistoryLogger.Log($"Пользователю {ConnectionBD.resFio} не получилось дважды открыть форму подключения");
                openedWindow.Activate();
                openedWindow.Focus();
            }
        }
    }
}