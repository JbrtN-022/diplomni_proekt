using rccs.MyClass;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace rccs_new
{
    public partial class UserOverlap : Window
    {
        public UserOverlap()
        {
            InitializeComponent();
            LoadUserData();
        }

        // Загрузка текущих логина и пароля
        private void LoadUserData()
        {
            if (string.IsNullOrEmpty(ConnectionBD.resFio))
            {
                MessageBox.Show("Не удалось определить текущего пользователя.", "Ошибка");
                return;
            }

            try
            {
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
                }
                else
                {
                    MessageBox.Show("Данные пользователя не найдены.", "Информация");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message}", "Ошибка");
            }
        }

        // Кнопка "Обновить данные"
        private void пользователь_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Пожалуйста, заполните логин и пароль!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Получаем id_workers по ФИО
                ConnectionBD.mycommand.CommandText = "SELECT id_workers FROM rccs.workers WHERE name = @fio";
                ConnectionBD.mycommand.Parameters.Clear();
                ConnectionBD.mycommand.Parameters.AddWithValue("@fio", ConnectionBD.resFio);

                object result = ConnectionBD.mycommand.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show("Не удалось найти пользователя.", "Ошибка");
                    return;
                }

                int id_workers = Convert.ToInt32(result);

                // Обновляем логин и пароль
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
                    MessageBox.Show("Данные успешно обновлены!", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось обновить данные.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления:\n{ex.Message}", "Ошибка");
            }
        }

        private void закрыть_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}