using Microsoft.Win32;
using MySql.Data.MySqlClient;
using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для форма_администратора.xaml
    /// </summary>
    public partial class форма_администратора : Window
    {
        public форма_администратора()
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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} авторизовался как администратор");
        }

        // Показывает справочное сообщение о форме администратора
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА АДМИНИСТРАТОРА
Назначение формы:
Главная панель управления системой с полным доступом ко всем функциям приложения.
Что можно сделать на этой форме:
• Управлять пользователями системы
• Работать со справочниками
• Управлять контрагентами
• Редактировать прайс-листы
• Просматривать и создавать документы (договоры и лицензии)
• Просматривать данные текущего пользователя
• Экспортировать и импортировать базу данных
• Выход из системы
Доступные разделы:
1. ПОЛЬЗОВАТЕЛИ
   • Просмотр списка всех пользователей
   • Добавление новых пользователей
   • Редактирование данных пользователей
   • Удаление пользователей
2. СПРАВОЧНИКИ
   • Управление справочной информацией
   • Редактирование справочных данных
3. КОНТРАГЕНТЫ
   • Просмотр списка контрагентов
   • Добавление новых контрагентов
   • Редактирование данных контрагентов
   • Удаление контрагентов
4. ПРАЙС
   • Просмотр истории прайс-листов
   • Управление ценами
5. ДОКУМЕНТЫ
   • Просмотр договоров и лицензий
   • Создание новых документов
   • Редактирование документов
6. ДАННЫЕ ПОЛЬЗОВАТЕЛЯ
   • Просмотр информации о текущем пользователе
7. ЭКСПОРТ/ИМПОРТ БД
   • Создание резервной копии базы данных (SQL файл)
   • Восстановление базы данных из резервной копии
8. ВЫХОД
   • Завершение текущей сессии и возврат к форме авторизации
",
                        "Помощь - Форма администратора",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
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

        // Открытие формы управления пользователями
        private void пользователи_Click(object sender, RoutedEventArgs e)
        {
            форма_пользователей polzovateli = new форма_пользователей();
            Application.Current.MainWindow = polzovateli;
            polzovateli.Show();
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму списка пользователей");
            this.Close();
        }

        // Открытие формы справочников
        private void справочники_Click(object sender, RoutedEventArgs e)
        {
            форма_справочников spravochnik = new форма_справочников();
            Application.Current.MainWindow = spravochnik;
            spravochnik.Show();
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму списка справочников");
            this.Close();
        }

        // Открытие формы контрагентов
        private void контрагенты_Click(object sender, RoutedEventArgs e)
        {
            форма_контрагентов form = new форма_контрагентов();
            Application.Current.MainWindow = form;
            form.Show();
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму списка контрагентов");
            this.Close();
        }

        // Открытие формы прайс-листов
        private void прайс_Click(object sender, RoutedEventArgs e)
        {
            форма_прайса form = new форма_прайса();
            Application.Current.MainWindow = form;
            form.Show();
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму списка истории прайслиста");
            this.Close();
        }

        // Открытие формы данных текущего пользователя
        private void пользователь_Click(object sender, RoutedEventArgs e)
        {
            UserOverlap overlap = new UserOverlap();
            Application.Current.MainWindow = overlap;
            overlap.Show();
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму данных пользователя");
        }

        // Открытие формы договоров и лицензий
        private void документы_Click(object sender, RoutedEventArgs e)
        {
            форма_договоров__лицензий form = new форма_договоров__лицензий();
            Application.Current.MainWindow = form;
            form.Show();
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму договора лицензий");
            this.Close();
        }

        // Экспорт базы данных в SQL-файл
        private void экспорт_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "SQL файл (*.sql)|*.sql";
                saveDialog.Title = "Сохранение резервной копии БД";
                saveDialog.FileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";

                if (saveDialog.ShowDialog() == true)
                {
                    string connectionString =
                        $@"Database={ConnectionBD.currentDataBase};
                        Data Source={ConnectionBD.currentDataSource};
                        user={ConnectionBD.currentUser};
                        password={ConnectionBD.currentPassword};
                        charset=utf8;";

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup mb = new MySqlBackup(cmd))
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                mb.ExportToFile(saveDialog.FileName);
                                conn.Close();
                            }
                        }
                    }

                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} экспортировал базу данных. Файл: {saveDialog.FileName}");
                    MessageBox.Show("База данных успешно экспортирована!", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"Ошибка экспорта базы данных: {ex.Message}");
                MessageBox.Show($"Ошибка экспорта БД:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Импорт базы данных из SQL-файла
        private void импорт_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Filter = "SQL файл (*.sql)|*.sql";
                openDialog.Title = "Выберите файл базы данных";

                if (openDialog.ShowDialog() == true)
                {
                    string sqlText = File.ReadAllText(openDialog.FileName);

                    List<string> requiredTables = new List<string>()
                    {
                        "city", "company", "counterparty", "floor", "keys_source",
                        "lease_agreement", "license_agreement", "office", "price_meter",
                        "price_programs", "price_services", "program", "room",
                        "service_in_agreement", "services", "type_of_face", "users", "workers"
                    };

                    bool allTablesFound = true;
                    string lowerSql = sqlText.ToLower();

                    foreach (string table in requiredTables)
                    {
                        bool tableExists = lowerSql.Contains($"create table `{table.ToLower()}`") ||
                                           lowerSql.Contains($"create table if not exists `{table.ToLower()}`") ||
                                           lowerSql.Contains($"create table {table.ToLower()}");

                        if (!tableExists)
                        {
                            allTablesFound = false;
                            MessageBox.Show($"В файле отсутствует таблица: {table}",
                                            "Ошибка импорта",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Warning);
                            break;
                        }
                    }

                    if (!allTablesFound)
                    {
                        HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} попытался импортировать неверную базу данных");
                        return;
                    }

                    var result = MessageBox.Show("Текущая база данных будет заменена.\nПродолжить?",
                                                 "Подтверждение",
                                                 MessageBoxButton.YesNo,
                                                 MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes)
                        return;

                    string connectionString =
                        $@"Database={ConnectionBD.currentDataBase};
                        Data Source={ConnectionBD.currentDataSource};
                        user={ConnectionBD.currentUser};
                        password={ConnectionBD.currentPassword};
                        charset=utf8;";

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            using (MySqlBackup mb = new MySqlBackup(cmd))
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                mb.ImportFromFile(openDialog.FileName);
                                conn.Close();
                            }
                        }
                    }

                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} импортировал базу данных из файла: {openDialog.FileName}");
                    MessageBox.Show("База данных успешно импортирована!", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"Ошибка импорта базы данных: {ex.Message}");
                MessageBox.Show($"Ошибка импорта БД:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}