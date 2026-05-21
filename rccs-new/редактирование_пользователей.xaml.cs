using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для редактирование_пользователей.xaml
    /// </summary>
    public partial class редактирование_пользователей : Window
    {
        private int currentWorkerId;

        public редактирование_пользователей(int id_workers)
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
            currentWorkerId = id_workers;
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму редактирования пользователя ID {id_workers}");

            LoadDataForEdit();
            LoadRoles();

            // Добавляем обработчики для валидации
            txtFIO.TextChanged += txtFIO_TextChanged;
            txtPhone.TextChanged += txtPhone_TextChanged;
            txtLogin.TextChanged += txtLogin_TextChanged;
        }
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА РЕДАКТИРОВАНИЯ СОТРУДНИКА

Назначение формы:
Редактирование личных данных и учетной записи сотрудника.

Что можно сделать на этой форме:
• Изменить ФИО сотрудника
• Изменить контактный телефон
• Изменить дату трудоустройства
• Изменить должность/роль
• Изменить компанию
• Изменить логин для входа в систему

Поля для редактирования:

1. ФИО 
   • Только буквы (русские/английские)
   • Пробелы и дефисы разрешены
   • Пример: Иванов Иван Иванович

2. ТЕЛЕФОН 
   • Формат: +7XXXXXXXXXX
   • Обязательно 10 цифр после +7
   • Пример: +79123456789

3. ДАТА ТРУДОУСТРОЙСТВА 
   • Не может быть больше сегодняшней
   • Не может быть раньше 01.01.1900
   • Формат: ДД.ММ.ГГГГ

4. ДОЛЖНОСТЬ/РОЛЬ 
   • Выбор из выпадающего списка
   • Определяет права доступа в системе

5. КОМПАНИЯ 
   • Выбор из выпадающего списка
   • Определяет принадлежность к компании

6. ЛОГИН 
   • 3-20 символов
   • Разрешены: буквы, цифры, ., -, _
   • Должен быть уникальным

Валидация полей:
• При некорректном вводе рамка поля становится красной
• При корректном вводе - зеленой
• Подсказки появляются при наведении на поле

Кнопки управления:

• СОХРАНИТЬ - сохраняет все изменения
• ОТМЕНА/НАЗАД - закрывает форму без сохранения



Примечание:
Логин должен быть уникальным в системе.
Изменение роли влияет на доступные функции в системе.
При изменении компании обновляются связанные документы.
Пароль можно изменить только через администратора или функцию восстановления.",
                "Помощь - Редактирование сотрудника",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        private void txtFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtFIO.Text))
            {
                if (!Regex.IsMatch(txtFIO.Text, @"^[а-яА-ЯёЁa-zA-Z\s\-]+$"))
                {
                    txtFIO.BorderBrush = Brushes.Red;
                    txtFIO.ToolTip = "ФИО должно содержать только буквы, пробелы и дефисы";
                }
                else
                {
                    txtFIO.BorderBrush = Brushes.Green;
                    txtFIO.ToolTip = null;
                }
            }
            else
            {
                txtFIO.BorderBrush = Brushes.Transparent;
                txtFIO.ToolTip = null;
            }
        }

        private void txtPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                string digitsOnly = Regex.Replace(txtPhone.Text, @"[^\d+]", "");

                if (!Regex.IsMatch(digitsOnly, @"^\+7\d{10}$"))
                {
                    txtPhone.BorderBrush = Brushes.Red;
                    txtPhone.ToolTip = "Телефон должен быть в формате +7XXXXXXXXXX (10 цифр после +7)";
                }
                else
                {
                    txtPhone.BorderBrush = Brushes.Green;
                    txtPhone.ToolTip = null;
                }
            }
            else
            {
                txtPhone.BorderBrush = Brushes.Transparent;
                txtPhone.ToolTip = null;
            }
        }

        private void txtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                if (!Regex.IsMatch(txtLogin.Text, @"^[a-zA-Z0-9._\-]{3,20}$"))
                {
                    txtLogin.BorderBrush = Brushes.Red;
                    txtLogin.ToolTip = "Логин должен содержать 3-20 символов (буквы, цифры, ., -, _)";
                }
                else
                {
                    txtLogin.BorderBrush = Brushes.Green;
                    txtLogin.ToolTip = null;
                }
            }
            else
            {
                txtLogin.BorderBrush = Brushes.Transparent;
                txtLogin.ToolTip = null;
            }
        }

        private void dpDate_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (dpDate.SelectedDate.HasValue)
            {
                if (dpDate.SelectedDate > DateTime.Now)
                {
                    MessageBox.Show("Дата не может быть больше сегодняшней!", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpDate.SelectedDate = null;
                }
                else if (dpDate.SelectedDate < new DateTime(1900, 1, 1))
                {
                    MessageBox.Show("Дата не может быть раньше 01.01.1900!", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpDate.SelectedDate = null;
                }
                else
                {
                    dpDate.BorderBrush = Brushes.Green;
                }
            }
        }

        private void LoadDataForEdit()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загружает данные пользователя ID {currentWorkerId}");

            ConnectionBD.mycommand.CommandText = @"
            SELECT 
                workers.id_workers,
                workers.name,
                workers.work_device_data,
                workers.number,
                workers.id_company,
                users.login,
                users.password,
                users.id_roll
            FROM rccs.workers
            JOIN rccs.users ON users.id_workers = workers.id_workers
            WHERE workers.id_workers = @id_workers";

            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_workers", currentWorkerId);

            ConnectionBD.dtTemp.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            if (ConnectionBD.dtTemp.Rows.Count > 0)
            {
                DataRow row = ConnectionBD.dtTemp.Rows[0];

                txtFIO.Text = row["name"]?.ToString() ?? "";
                txtPhone.Text = row["number"]?.ToString() ?? "";
                txtLogin.Text = row["login"]?.ToString() ?? "";

                if (row["work_device_data"] != DBNull.Value && row["work_device_data"] != null)
                {
                    dpDate.SelectedDate = Convert.ToDateTime(row["work_device_data"]);
                }

                if (row["id_roll"] != DBNull.Value && row["id_roll"] != null)
                {
                    int roleId = Convert.ToInt32(row["id_roll"]);

                    cmbRole.SelectedValue = roleId;

                    if (cmbRole.SelectedValue == null)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            cmbRole.SelectedValue = roleId;
                        }), System.Windows.Threading.DispatcherPriority.Loaded);
                    }
                }

                if (row["id_company"] != DBNull.Value && row["id_company"] != null)
                {
                    int companyId = Convert.ToInt32(row["id_company"]);

                    cmbComp.SelectedValue = companyId;

                    if (cmbComp.SelectedValue == null)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            cmbComp.SelectedValue = companyId;
                        }), System.Windows.Threading.DispatcherPriority.Loaded);
                    }
                }

                // Устанавливаем зеленые границы для корректных полей
                txtFIO.BorderBrush = Brushes.Green;
                txtPhone.BorderBrush = Brushes.Green;
                txtLogin.BorderBrush = Brushes.Green;
            }
            else
            {
                HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог загрузить данные пользователя ID {currentWorkerId}");
                MessageBox.Show("Не удалось загрузить данные сотрудника.", "Ошибка");
            }
        }

        private void LoadRoles()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список ролей и компаний");

            guideBD.selectRoll();
            cmbRole.ItemsSource = ConnectionBD.dtRoll.DefaultView;
            cmbRole.DisplayMemberPath = "roll";
            cmbRole.SelectedValuePath = "id_roll";

            guideBD.selectCompany();
            cmbComp.ItemsSource = ConnectionBD.dtCompanyCombobox.DefaultView;
            cmbComp.DisplayMemberPath = "company";
            cmbComp.SelectedValuePath = "id_company";
        }

        // Валидация всех полей перед сохранением
        private bool ValidateAllFields()
        {
            // Проверка ФИО
            if (string.IsNullOrWhiteSpace(txtFIO.Text))
            {
                MessageBox.Show("Поле 'ФИО' обязательно для заполнения!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFIO.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtFIO.Text, @"^[а-яА-ЯёЁa-zA-Z\s\-]+$"))
            {
                MessageBox.Show("ФИО должно содержать только буквы, пробелы и дефисы!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFIO.Focus();
                return false;
            }

            // Проверка телефона
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Поле 'Телефон' обязательно для заполнения!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            string digitsOnly = Regex.Replace(txtPhone.Text, @"[^\d+]", "");
            if (!Regex.IsMatch(digitsOnly, @"^\+7\d{10}$"))
            {
                MessageBox.Show("Телефон должен быть в формате +7XXXXXXXXXX (10 цифр после +7)!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            // Проверка даты
            if (dpDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату трудоустройства!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                dpDate.Focus();
                return false;
            }

            if (dpDate.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Дата не может быть больше сегодняшней!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                dpDate.Focus();
                return false;
            }

            // Проверка роли
            if (cmbRole.SelectedValue == null)
            {
                MessageBox.Show("Выберите роль сотрудника!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbRole.Focus();
                return false;
            }

            // Проверка компании
            if (cmbComp.SelectedValue == null)
            {
                MessageBox.Show("Выберите компанию!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbComp.Focus();
                return false;
            }

            // Проверка логина
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Поле 'Логин' обязательно для заполнения!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLogin.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtLogin.Text, @"^[a-zA-Z0-9._\-]{3,20}$"))
            {
                MessageBox.Show("Логин должен содержать 3-20 символов (буквы, цифры, ., -, _)!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLogin.Focus();
                return false;
            }

            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Выполняем все проверки
            if (!ValidateAllFields())
                return;

            try
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} обновляет данные пользователя ID {currentWorkerId}");

                // Используем параметризованные запросы для безопасности
                ConnectionBD.mycommand.CommandText = @"
                    UPDATE rccs.workers 
                    SET name = @name,
                        work_device_data = @work_date,
                        number = @phone,
                        id_company = @company_id
                    WHERE id_workers = @worker_id";

                ConnectionBD.mycommand.Parameters.Clear();
                ConnectionBD.mycommand.Parameters.AddWithValue("@name", txtFIO.Text.Trim());
                ConnectionBD.mycommand.Parameters.AddWithValue("@work_date", dpDate.SelectedDate?.ToString("yyyy-MM-dd"));
                ConnectionBD.mycommand.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                ConnectionBD.mycommand.Parameters.AddWithValue("@company_id", Convert.ToInt32(cmbComp.SelectedValue));
                ConnectionBD.mycommand.Parameters.AddWithValue("@worker_id", currentWorkerId);

                ConnectionBD.mycommand.ExecuteNonQuery();

                ConnectionBD.mycommand.CommandText = @"
                    UPDATE rccs.users 
                    SET login = @login,
                        id_roll = @role_id
                    WHERE id_workers = @worker_id";

                ConnectionBD.mycommand.Parameters.Clear();
                ConnectionBD.mycommand.Parameters.AddWithValue("@login", txtLogin.Text.Trim());
                ConnectionBD.mycommand.Parameters.AddWithValue("@role_id", Convert.ToInt32(cmbRole.SelectedValue));
                ConnectionBD.mycommand.Parameters.AddWithValue("@worker_id", currentWorkerId);

                ConnectionBD.mycommand.ExecuteNonQuery();

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно обновил пользователя ID {currentWorkerId}");

                MessageBox.Show("Данные сотрудника успешно обновлены!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог обновить пользователя ID {currentWorkerId}. Ошибка: {ex.Message}");

                MessageBox.Show($"Ошибка при обновлении:\n{ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}