using rccs.MyClass;
using rccs_new.MyClass;
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
using System.Windows.Shapes;

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
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.F1)
                {
                    ShowHelp();
                    e.Handled = true;
                }
            };

            if (ConnectionBD.resFio.Length == 0)
            {
                title.Text = "ошибка";
            }

            title.Text = $@"С возвращением, {ConnectionBD.resFio}";

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} авторизовался как менеджер");
        }
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
        private void контрагенты_Click(object sender, RoutedEventArgs e)
        {
            форма_контрагентов spravochnik = new форма_контрагентов();

            Application.Current.MainWindow = spravochnik;
            spravochnik.Show();

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму контрагентов");

            this.Close();
        }

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

        private void ReplaceText(DocumentFormat.OpenXml.Wordprocessing.Body body, string placeholder, string newText)
        {
            foreach (var text in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, newText);
                }
            }
        }

        private void печать_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} нажал кнопку печати");

            try
            {
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(exeDirectory, "..", ".."));
                string docPath = System.IO.Path.Combine(projectDirectory, "document");
                string templatePath = System.IO.Path.Combine(docPath, "ФОРМА ЗАЯВКИ КЛИЕНТА НА ПРИОБРЕТЕНИЕ ЛИЦЕНЗИИ НА ПРОГРАММНОЕ ОБЕСПЕЧЕНИЕ.docx");

                if (!System.IO.File.Exists(templatePath))
                {
                    MessageBox.Show($"Файл шаблона не найден:\n{templatePath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBoxResult result = MessageBox.Show(
                    "Открыть форму заявки для просмотра?",
                    "Выбор действия",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    string tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"Форма_заявки_{DateTime.Now:yyyyMMdd_HHmmss}.docx");
                    System.IO.File.Copy(templatePath, tempFile, true);
                    System.Diagnostics.Process.Start(tempFile);
                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму заявки для просмотра");
                }
                else
                {
                    //System.Diagnostics.Process.Start(templatePath);
                    //System.Windows.Forms.SendKeys.SendWait("^p");
                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} отправил форму заявки на печать");
                }
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"Ошибка при открытии формы заявки: {ex.Message}");
                MessageBox.Show($"Ошибка при открытии документа:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void пользователь_Click(object sender, RoutedEventArgs e)
        {
            UserOverlap overlap = new UserOverlap();

            Application.Current.MainWindow = overlap;
            overlap.Show();

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл окно пользователя");
        }

        private void viewingLicense_Click(object sender, RoutedEventArgs e)
        {
            просмотр_лицензий prosmotrlicens = new просмотр_лицензий();

            Application.Current.MainWindow = prosmotrlicens;
            prosmotrlicens.Show();

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл просмотр лицензий");
        }

        private void addLicense_Click(object sender, RoutedEventArgs e)
        {
            Форма_оформления_лицензии oformlenieLicens = new Форма_оформления_лицензии();

            Application.Current.MainWindow = oformlenieLicens;
            oformlenieLicens.Show();

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму оформления лицензии");
        }

        private void подключение_Click(object sender, RoutedEventArgs e)
        {
            форма__насторойки_подключения_бд openedWindow = null;

            foreach (Window window in Application.Current.Windows)
            {
                if (window is форма__насторойки_подключения_бд)
                {
                    openedWindow =
                        (форма__насторойки_подключения_бд)window;

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
                        MessageBox.Show(
                            "Настройки подключения обновлены",
                            "Успешно",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

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
                MessageBox.Show(
                    "Нельзя открыть больше 1 окна!",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                HistoryLogger.Log($"Пользователю {ConnectionBD.resFio} не получилось дважды открыть форму подключения");

                openedWindow.Activate();
                openedWindow.Focus();
            }
        }
    }
}