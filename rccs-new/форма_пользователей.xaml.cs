using MySql.Data.MySqlClient;
using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для форма_пользователей.xaml
    /// </summary>
    public partial class форма_пользователей : Window
    {
        public форма_пользователей()
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

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл окно управления сотрудниками");
            LoadWorkers();
        }

        // Показывает справочное сообщение о форме управления пользователями
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА УПРАВЛЕНИЯ ПОЛЬЗОВАТЕЛЯМИ
Назначение формы:
Управление учетными записями сотрудников и пользователями системы.
Что можно сделать на этой форме:
• Просматривать список всех сотрудников
• Добавлять новых сотрудников
• Редактировать данные существующих сотрудников
• Удалять сотрудников (с проверкой связей)
Функциональные возможности:
1. ДОБАВЛЕНИЕ СОТРУДНИКА
   • Кнопка ""+""
   • Открывает форму для ввода данных нового сотрудника
   • Заполните все обязательные поля: ФИО, телефон, дата устройства, логин, пароль
2. РЕДАКТИРОВАНИЕ СОТРУДНИКА
   • Выберите сотрудника в таблице
   • Нажмите кнопку ""✎"" (карандаш)
   • Измените необходимые данные
   • Данные пользователя (логин/пароль) также можно изменить
3. УДАЛЕНИЕ СОТРУДНИКА
   • Выберите сотрудника в таблице
   • Нажмите кнопку ""🗑"" (корзина)
   • Подтвердите удаление
4. ТАБЛИЦА СОТРУДНИКОВ
   • Отображает всех сотрудников системы
   • Столбцы: ФИО, Телефон, Компания, Роль, Логин
Ограничения на удаление:
Сотрудника нельзя удалить, если он:
• Участвует в лицензионных договорах
• Участвует в договорах аренды
• Имеет другие связанные документы
Перед удалением сотрудника необходимо:
1. Переназначить или удалить все его документы
2. Либо передать документы другому сотруднику
Примечание:
Некоторые функции (например, удаление) доступны только администраторам.
При удалении сотрудника его учетная запись удаляется из системы полностью.
Рекомендуется перед удалением проверить все связанные документы.",
                "Помощь - Управление пользователями",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // Загрузка списка всех сотрудников в таблицу
        public void LoadWorkers()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список сотрудников");
            ClassWorkers.SelectWorkers();
            dgSpravochnik.ItemsSource = ConnectionBD.dtWorkers.DefaultView;
        }

        // Возврат в главное окно администратора
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} вернулся в панель администратора из окна управления сотрудниками");

            форма_администратора formadm = new форма_администратора();
            Application.Current.MainWindow = formadm;
            formadm.Show();
            this.Close();
        }

        // Удаление выбранного сотрудника с проверкой связанных документов
        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dgSpravochnik.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления!", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = dgSpravochnik.SelectedItem as DataRowView;
            if (row == null) return;

            int id_workers = Convert.ToInt32(row["id_workers"]);

            try
            {
                ConnectionBD.myconnection.Open();

                // Проверка наличия связанных лицензионных договоров
                string queryLicense = $"SELECT count(*) FROM rccs.license_agreement WHERE id_workers = {id_workers}";
                MySqlCommand cmdLicense = new MySqlCommand(queryLicense, ConnectionBD.myconnection);
                int licenseCount = Convert.ToInt32(cmdLicense.ExecuteScalar());

                // Проверка наличия связанных договоров аренды
                string queryLease = $"SELECT count(*) FROM rccs.lease_agreement WHERE id_workers = {id_workers}";
                MySqlCommand cmdLease = new MySqlCommand(queryLease, ConnectionBD.myconnection);
                int leaseCount = Convert.ToInt32(cmdLease.ExecuteScalar());

                ConnectionBD.myconnection.Close();

                if (licenseCount > 0 || leaseCount > 0)
                {
                    string message = "Нельзя удалить сотрудника, так как он участвует в следующих документах:\n\n";
                    if (licenseCount > 0)
                        message += $"• Лицензионные договоры: {licenseCount} шт.\n";
                    if (leaseCount > 0)
                        message += $"• Договоры аренды: {leaseCount} шт.\n";
                    message += "\nСначала удалите или переназначьте все связанные документы.";

                    MessageBox.Show(message, "Невозможно удалить",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Запрос подтверждения удаления
                var result = MessageBox.Show(
                    $@"Вы действительно хотите удалить сотрудника? ФИО: {row["name"]}",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    ClassWorkers.DeleteWorker(id_workers);
                    MessageBox.Show("Сотрудник успешно удалён!", "Успешно",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadWorkers();
                }
            }
            catch (Exception ex)
            {
                if (ConnectionBD.myconnection.State == ConnectionState.Open)
                    ConnectionBD.myconnection.Close();

                MessageBox.Show($"Ошибка при проверке или удалении:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Открытие формы редактирования выбранного сотрудника
        private void UppBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dgSpravochnik.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования!", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = dgSpravochnik.SelectedItem as DataRowView;
            if (row == null) return;

            int id_workers = Convert.ToInt32(row["id_workers"]);
            string workerName = row["name"]?.ToString() ?? "Неизвестно";

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму редактирования сотрудника ID {id_workers} ({workerName})");

            редактирование_пользователей editForm = new редактирование_пользователей(id_workers);
            editForm.Owner = this;

            if (editForm.ShowDialog() == true)
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно отредактировал сотрудника ID {id_workers} ({workerName})");
                LoadWorkers();
            }
            else
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} отменил редактирование сотрудника ID {id_workers}");
            }
        }

        // Открытие формы добавления нового сотрудника
        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму добавления нового сотрудника");

            добавление_пользователей addform = new добавление_пользователей();
            addform.Owner = this;

            if (addform.ShowDialog() == true)
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно добавил нового сотрудника");
                LoadWorkers();
            }
            else
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} отменил добавление сотрудника");
            }
        }

        private void dgSpravochnik_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}