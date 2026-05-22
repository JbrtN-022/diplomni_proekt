using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Collections.Generic;
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
using static rccs.MyClass.ClassWorkers;

namespace rccs_new
{
    public partial class добавление_пользователей : Window
    {
        public добавление_пользователей()
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

            LoadRoles();

            dpDate.SelectedDateFormat = DatePickerFormat.Short;

            // Подключение обработчиков валидации при изменении текста
            txtFIO.TextChanged += txtFIO_TextChanged;
            txtPhone.TextChanged += txtPhone_TextChanged;
            txtLogin.TextChanged += txtLogin_TextChanged;
            txtPassword.TextChanged += txtPassword_TextChanged;
        }

        // Показывает справочное сообщение о форме добавления пользователя
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА ДОБАВЛЕНИЯ СОТРУДНИКА
Назначение формы:
Добавление нового сотрудника в систему с созданием учетной записи.
Что можно сделать на этой форме:
• Ввести личные данные сотрудника
• Установить дату трудоустройства
• Назначить роль и компанию
• Создать учетную запись (логин/пароль)
Поля для заполнения:
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
   • Определяет права доступа
5. КОМПАНИЯ
   • Выбор из выпадающего списка
   • Определяет принадлежность к компании
6. ЛОГИН
   • 3-20 символов
   • Разрешены: буквы, цифры, ., -, _
   • Должен быть уникальным
7. ПАРОЛЬ
   • Минимум 6 символов
   • Рекомендуется использовать сложный пароль
Примечание:
Логин должен быть уникальным в системе.
После успешного добавления сотрудник сможет войти в систему.",
                "Помощь - Добавление сотрудника",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // Валидация ФИО в реальном времени
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

        // Валидация телефона в реальном времени
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

        // Валидация логина в реальном времени
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

        // Валидация пароля в реальном времени
        private void txtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                if (txtPassword.Text.Length < 6)
                {
                    txtPassword.BorderBrush = Brushes.Red;
                    txtPassword.ToolTip = "Пароль должен содержать минимум 6 символов";
                }
                else
                {
                    txtPassword.BorderBrush = Brushes.Green;
                    txtPassword.ToolTip = null;
                }
            }
            else
            {
                txtPassword.BorderBrush = Brushes.Transparent;
                txtPassword.ToolTip = null;
            }
        }

        // Проверка корректности выбранной даты трудоустройства
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

        // Загрузка ролей и компаний в комбобоксы
        private void LoadRoles()
        {
            guideBD.selectRoll();
            cmbRole.ItemsSource = ConnectionBD.dtRoll.DefaultView;
            cmbRole.DisplayMemberPath = "roll";
            cmbRole.SelectedValuePath = "id_roll";

            guideBD.selectCompany();
            cmbComp.ItemsSource = ConnectionBD.dtCompanyCombobox.DefaultView;
            cmbComp.DisplayMemberPath = "company";
            cmbComp.SelectedValuePath = "id_company";
        }

        // Комплексная проверка всех полей перед добавлением сотрудника
        private bool ValidateAllFields()
        {
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

            if (cmbRole.SelectedValue == null)
            {
                MessageBox.Show("Выберите роль сотрудника!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbRole.Focus();
                return false;
            }

            if (cmbComp.SelectedValue == null)
            {
                MessageBox.Show("Выберите компанию!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbComp.Focus();
                return false;
            }

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

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Поле 'Пароль' обязательно для заполнения!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }
            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }
            return true;
        }

        // Обработка нажатия кнопки "Добавить"
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAllFields())
                return;

            try
            {
                int workerId = ClassWorkers.AddWorker(
                    txtFIO.Text.Trim(),
                    dpDate.SelectedDate.Value,
                    txtPhone.Text.Trim(),
                    Convert.ToInt32(cmbComp.SelectedValue)
                );

                ClassWorkers.AddUser(
                    txtLogin.Text.Trim(),
                    txtPassword.Text.Trim(),
                    workerId,
                    Convert.ToInt32(cmbRole.SelectedValue)
                );

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} добавил нового сотрудника: {txtFIO.Text.Trim()}");
                MessageBox.Show("Работник успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог добавить сотрудника. Ошибка: {ex.Message}");
                MessageBox.Show($"Ошибка при добавлении:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}