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
    /// Логика взаимодействия для редактирование_контрагентов.xaml
    /// </summary>
    public partial class редактирование_контрагентов : Window
    {
        private int currenCounterpartId;

        public редактирование_контрагентов(int id_workers)
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
            currenCounterpartId = id_workers;

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл редактирование контрагента ID {currenCounterpartId}");

            LoadComboBoxes();
            LoadDataForEdit();
        }
        // Показывает справочное сообщение о форме
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА РЕДАКТИРОВАНИЯ КОНТРАГЕНТА

Назначение формы:
Редактирование информации о существующем контрагенте (партнере, клиенте, поставщике).

Что можно сделать на этой форме:
• Изменить название контрагента
• Изменить город и тип лица
• Обновить адреса (фактический и юридический)
• Изменить контактные данные (телефон, email, веб-сайт)
• Обновить реквизиты (ИНН, БИК)
• Изменить контактное лицо

Поля для заполнения:

1. НАЗВАНИЕ КОНТРАГЕНТА (обязательное поле)
   • Полное наименование организации или ФИО
   • Должно быть уникальным

2. ГОРОД (обязательное поле)
   • Выбор из списка городов
   • Определяет местонахождение контрагента

3. ТИП ЛИЦА (обязательное поле)
   • Юридическое лицо
   • Физическое лицо

4. ФАКТИЧЕСКИЙ АДРЕС
   • Адрес фактического местонахождения
   • Необязательное поле

5. ЮРИДИЧЕСКИЙ АДРЕС
   • Юридический адрес организации
   • Необязательное поле

6. EMAIL
   • Адрес электронной почты
   • Проверяется формат (user@domain.ru)
   • Необязательное поле

7. ВЕБ-САЙТ
   • Адрес сайта контрагента
   • Проверяется корректность URL
   • Необязательное поле

8. ИНН
   • Идентификационный номер налогоплательщика
   • Необязательное поле

9. БИК
   • Банковский идентификационный код
   • Необязательное поле

10. КОНТАКТНОЕ ЛИЦО
    • ФИО контактного сотрудника
    • Только буквы, пробелы, дефисы, точки
    • Необязательное поле

11. ТЕЛЕФОН КОНТАКТНОГО ЛИЦА
    • Контактный телефон
    • Проверяется формат номера
    • Необязательное поле

Валидация:
• Обязательные поля выделены красным при пустом значении
• При корректном вводе рамка становится зеленой
• Необязательные поля проверяются только если заполнены



Примечание:
Перед сохранением проверьте правильность введенных данных.
ИНН и БИК рекомендуется заполнять для юридических лиц.
После сохранения изменения применятся во всех связанных документах.",
                "Помощь - Редактирование контрагента",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Поле 'Название контрагента' обязательно для заполнения!",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }

            if (cmbCity.SelectedValue == null)
            {
                MessageBox.Show("Выберите город!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbCity.Focus();
                return;
            }

            if (cmbTypeFace.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип лица!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbTypeFace.Focus();
                return;
            }

            // Проверка email (если заполнен)
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (!IsValidEmail(txtEmail.Text.Trim()))
                {
                    MessageBox.Show("Введите корректный email адрес!\nПример: user@domain.ru",
                                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtEmail.Focus();
                    return;
                }
            }

            // Проверка телефона (если заполнен)
            if (!string.IsNullOrWhiteSpace(txtContPhone.Text))
            {
                if (!IsValidPhone(txtContPhone.Text.Trim()))
                {
                    MessageBox.Show("Введите корректный номер телефона!\n" +
                                    "Допустимые форматы:\n" +
                                    "+7 (123) 456-78-90\n" +
                                    "8 (123) 456-78-90\n" +
                                    "1234567890\n" +
                                    "+1234567890",
                                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtContPhone.Focus();
                    return;
                }
            }

            // Проверка веб-сайта (если заполнен)
            if (!string.IsNullOrWhiteSpace(txtWebPage.Text))
            {
                if (!IsValidWebsite(txtWebPage.Text.Trim()))
                {
                    MessageBox.Show("Введите корректный URL веб-сайта!\n" +
                                    "Примеры:\n" +
                                    "https://example.com\n" +
                                    "http://example.com\n" +
                                    "www.example.com\n" +
                                    "example.com",
                                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtWebPage.Focus();
                    return;
                }
            }

            // Проверка контактного лица (только буквы, пробелы, дефисы, точки - если заполнен)
            if (!string.IsNullOrWhiteSpace(txtContName.Text))
            {
                if (!IsValidContactPerson(txtContName.Text.Trim()))
                {
                    MessageBox.Show("Контактное лицо должно содержать только буквы, пробелы, дефисы и точки!\n" +
                                    "Пример: Иванов И.И.",
                                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtContName.Focus();
                    return;
                }
            }

            try
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} пытается обновить контрагента ID {currenCounterpartId}");

                bool success = counterparty.UpdateCounterparty(
                    id_counterparty: currenCounterpartId,
                    name: txtName.Text.Trim(),
                    id_city: Convert.ToInt32(cmbCity.SelectedValue),
                    actual_address: txtActualAddress.Text.Trim(),
                    legal_address: txtLegalAddress.Text.Trim(),
                    email: txtEmail.Text.Trim(),
                    web_page: txtWebPage.Text.Trim(),
                    inn: txtINN.Text.Trim(),
                    bic: txtBIC.Text.Trim(),
                    id_type_of_face: Convert.ToInt32(cmbTypeFace.SelectedValue),
                    cont_person_name: txtContName.Text.Trim(),
                    cont_person_phone: txtContPhone.Text.Trim()
                );

                if (success)
                {
                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно обновил контрагента ID {currenCounterpartId}");

                    MessageBox.Show("Данные контрагента успешно обновлены!", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} не смог обновить контрагента ID {currenCounterpartId}");

                    MessageBox.Show("Не удалось обновить данные.", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} получил ошибку при обновлении контрагента ID {currenCounterpartId}. Ошибка: {ex.Message}");

                MessageBox.Show($"Ошибка при обновлении:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Валидация email
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true; // Пустое поле допустимо

            try
            {
                var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        // Валидация телефона
        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true; // Пустое поле допустимо

            // Удаляем все нецифровые символы для проверки
            string digitsOnly = Regex.Replace(phone, @"[^\d+]", "");

            // Проверяем различные форматы телефонов
            // Допустимые форматы: 
            // 10-15 цифр, может начинаться с +
            var regex = new Regex(@"^\+?\d{10,15}$");
            return regex.IsMatch(digitsOnly);
        }

        // Валидация веб-сайта
        private bool IsValidWebsite(string website)
        {
            if (string.IsNullOrWhiteSpace(website))
                return true; // Пустое поле допустимо

            // Добавляем http:// если нет протокола
            string urlToCheck = website;
            if (!website.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !website.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                urlToCheck = "http://" + website;
            }

            try
            {
                Uri uriResult;
                bool result = Uri.TryCreate(urlToCheck, UriKind.Absolute, out uriResult) &&
                             (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                // Дополнительная проверка на домен верхнего уровня
                if (result)
                {
                    var regex = new Regex(@"^(https?://)?([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}(/.*)?$");
                    result = regex.IsMatch(website);
                }

                return result;
            }
            catch
            {
                return false;
            }
        }

        // Валидация контактного лица (только буквы, пробелы, дефисы, точки)
        private bool IsValidContactPerson(string contactPerson)
        {
            if (string.IsNullOrWhiteSpace(contactPerson))
                return true; // Пустое поле допустимо

            // Разрешаем: буквы (русские и английские), пробелы, дефисы, точки, запятые
            var regex = new Regex(@"^[a-zA-Zа-яА-ЯёЁ\s\-\.\,]+$");
            return regex.IsMatch(contactPerson);
        }
        // Загрузка текущих данных контрагента для редактирования
        private void LoadDataForEdit()
        {
            ConnectionBD.mycommand.CommandText = @"
        SELECT 
            counterparty.id_counterparty,
            counterparty.name,
            counterparty.id_city,
            city.city,
            counterparty.id_type_of_face,
            type_of_face.type_of_face,
            counterparty.actual_address,
            counterparty.legal_address,
            counterparty.`e-mail`,
            counterparty.web_page,
            counterparty.INN,
            counterparty.BIC,
            counterparty.cont_person_name,
            counterparty.cont_person_phone
        FROM rccs.counterparty
        JOIN rccs.city ON city.id_city = counterparty.id_city
        JOIN rccs.type_of_face ON type_of_face.id_type_of_face = counterparty.id_type_of_face
        WHERE counterparty.id_counterparty = @id_counterparty";

            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_counterparty", currenCounterpartId);

            ConnectionBD.dtTemp.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            if (ConnectionBD.dtTemp.Rows.Count > 0)
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил данные контрагента ID {currenCounterpartId}");

                DataRow row = ConnectionBD.dtTemp.Rows[0];

                txtName.Text = row["name"]?.ToString() ?? "";
                txtContName.Text = row["cont_person_name"]?.ToString() ?? "";
                txtContPhone.Text = row["cont_person_phone"]?.ToString() ?? "";
                txtLegalAddress.Text = row["legal_address"]?.ToString() ?? "";
                txtActualAddress.Text = row["actual_address"]?.ToString() ?? "";
                txtBIC.Text = row["BIC"]?.ToString() ?? "";
                txtINN.Text = row["INN"]?.ToString() ?? "";
                txtWebPage.Text = row["web_page"]?.ToString() ?? "";
                txtEmail.Text = row["e-mail"]?.ToString() ?? "";

                if (row["id_city"] != DBNull.Value && row["id_city"] != null)
                {
                    int cityId = Convert.ToInt32(row["id_city"]);
                    cmbCity.SelectedValue = cityId;

                    if (cmbCity.SelectedValue == null)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            cmbCity.SelectedValue = cityId;
                        }), System.Windows.Threading.DispatcherPriority.Loaded);
                    }
                }

                if (row["id_type_of_face"] != DBNull.Value && row["id_type_of_face"] != null)
                {
                    int typeId = Convert.ToInt32(row["id_type_of_face"]);
                    cmbTypeFace.SelectedValue = typeId;

                    if (cmbTypeFace.SelectedValue == null)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            cmbTypeFace.SelectedValue = typeId;
                        }), System.Windows.Threading.DispatcherPriority.Loaded);
                    }
                }
            }
            else
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} не смог загрузить данные контрагента ID {currenCounterpartId}");

                MessageBox.Show("Не удалось загрузить данные контрагента.", "Ошибка");
            }
        }
        // Загрузка справочников (города и типы лица)
        private void LoadComboBoxes()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил справочники для редактирования контрагента");

            guideBD.selectGoroda();
            cmbCity.ItemsSource = ConnectionBD.dtGoroda.DefaultView;
            cmbCity.DisplayMemberPath = "city";
            cmbCity.SelectedValuePath = "id_city";

            guideBD.selectVidLica();
            cmbTypeFace.ItemsSource = ConnectionBD.dtVidLica.DefaultView;
            cmbTypeFace.DisplayMemberPath = "type_of_face";
            cmbTypeFace.SelectedValuePath = "id_type_of_face";
        }

       
    }
}