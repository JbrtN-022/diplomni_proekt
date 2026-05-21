using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace rccs_new
{
    public partial class UserOverlap : Window
    {
        public UserOverlap()
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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл окно изменения данных аккаунта");

            LoadUserData();
        }

        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА ИЗМЕНЕНИЯ ДАННЫХ АККАУНТА

Назначение формы:
Изменение логина и пароля для входа в систему текущего пользователя.

Что можно сделать на этой форме:
• Изменить логин для входа в систему
• Изменить пароль для входа в систему
• Сохранить изменения
• Закрыть форму без сохранения

Поля для заполнения:

1. ЛОГИН (обязательное поле)
   • Уникальное имя пользователя
   • Используется для входа в систему
   • 3-20 символов
   • Разрешены: буквы, цифры, ., -, _

2. ПАРОЛЬ (обязательное поле)
   • Конфиденциальный ключ доступа
   • Минимум 6 символов
   • Рекомендуется использовать сложный пароль

Кнопки управления:

• СОХРАНИТЬ - сохраняет изменения логина и пароля
• ЗАКРЫТЬ - закрывает форму без сохранения

Важные моменты:

1. При смене логина:
   • Новый логин должен быть уникальным
   • При следующем входе используйте новый логин

2. При смене пароля:
   • Пароль должен содержать минимум 6 символов
   • Рекомендуется использовать буквы, цифры и символы
   • Не сообщайте пароль другим лицам

3. После сохранения:
   • Изменения вступают в силу немедленно
   • При следующем входе используйте новые данные



Примечание:
Логин должен быть уникальным в системе.
Рекомендуется периодически менять пароль для безопасности.
Не используйте простые пароли (123456, qwerty и т.д.).
При утере пароля обратитесь к администратору системы.",
                "Помощь - Изменение данных аккаунта",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        private void LoadUserData()
        {
            if (string.IsNullOrEmpty(ConnectionBD.resFio))
            {
                HistoryLogger.Log("Ошибка загрузки данных пользователя: resFio пустой");

                MessageBox.Show("Не удалось определить текущего пользователя.", "Ошибка");

                return;
            }

            try
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загружает данные аккаунта");

                string sql = @"
                    SELECT users.login, users.password
                    FROM rccs.users
                    JOIN rccs.workers ON workers.id_workers = users.id_workers
                    WHERE workers.name = @fio";

                ConnectionBD.mycommand.CommandText = sql;

                ConnectionBD.mycommand.Parameters.Clear();

                ConnectionBD.mycommand.Parameters.AddWithValue("@fio", ConnectionBD.resFio);

                ConnectionBD.dtTemp.Clear();

                ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

                if (ConnectionBD.dtTemp.Rows.Count > 0)
                {
                    DataRow row = ConnectionBD.dtTemp.Rows[0];

                    txtLogin.Text = row["login"]?.ToString() ?? "";

                    txtPassword.Text = row["password"]?.ToString() ?? "";

                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно загрузил данные аккаунта");
                }
                else
                {
                    HistoryLogger.Log($"Данные пользователя {ConnectionBD.resFio} не найдены");

                    MessageBox.Show("Данные пользователя не найдены.", "Информация");
                }
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"Ошибка загрузки данных пользователя {ConnectionBD.resFio}: {ex.Message}");

                MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message}", "Ошибка");
            }
        }

    
        private void пользователь_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} попытался обновить данные с пустыми полями");

                MessageBox.Show("Пожалуйста, заполните логин и пароль!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            try
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} пытается обновить данные аккаунта");

                
                ConnectionBD.mycommand.CommandText = "SELECT id_workers FROM rccs.workers WHERE name = @fio";

                ConnectionBD.mycommand.Parameters.Clear();

                ConnectionBD.mycommand.Parameters.AddWithValue("@fio", ConnectionBD.resFio);

                object result = ConnectionBD.mycommand.ExecuteScalar();

                if (result == null)
                {
                    HistoryLogger.Log($"Не удалось найти пользователя {ConnectionBD.resFio} для обновления");

                    MessageBox.Show("Не удалось найти пользователя.", "Ошибка");

                    return;
                }

                int id_workers = Convert.ToInt32(result);

                
                ConnectionBD.mycommand.CommandText = @"
                    UPDATE rccs.users 
                    SET login = @login, 
                        password = @password 
                    WHERE id_workers = @id_workers";

                ConnectionBD.mycommand.Parameters.Clear();

                ConnectionBD.mycommand.Parameters.AddWithValue("@login", txtLogin.Text.Trim());

                ConnectionBD.mycommand.Parameters.AddWithValue("@password", txtPassword.Text.Trim());

                ConnectionBD.mycommand.Parameters.AddWithValue("@id_workers", id_workers);

                int affected = ConnectionBD.mycommand.ExecuteNonQuery();

                if (affected > 0)
                {
                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно обновил данные аккаунта");

                    MessageBox.Show("Данные успешно обновлены!", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
                else
                {
                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} не смог обновить данные аккаунта");

                    MessageBox.Show("Не удалось обновить данные.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"Ошибка обновления данных пользователя {ConnectionBD.resFio}: {ex.Message}");

                MessageBox.Show($"Ошибка обновления:\n{ex.Message}", "Ошибка");
            }
        }

        private void закрыть_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} закрыл окно изменения данных аккаунта");

            this.Close();
        }
    }
}