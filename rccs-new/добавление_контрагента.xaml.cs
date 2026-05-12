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
    /// Логика взаимодействия для добавление_контрагента.xaml
    /// </summary>
    public partial class добавление_контрагента : Window
    {
        public добавление_контрагента()
        {
            InitializeComponent();

            HistoryLogger.Log("Открыта форма добавления контрагента");
            LoadComboBoxes();
        }
        private void LoadComboBoxes()
        {
            // Загрузка городов
            guideBD.selectGoroda();
            cmbCity.ItemsSource = ConnectionBD.dtGoroda.DefaultView;
            cmbCity.DisplayMemberPath = "city";
            cmbCity.SelectedValuePath = "id_city";

            // Загрузка типов лица (Вид лица)
            guideBD.selectVidLica();
            cmbTypeFace.ItemsSource = ConnectionBD.dtVidLica.DefaultView;
            cmbTypeFace.DisplayMemberPath = "type_of_face";
            cmbTypeFace.SelectedValuePath = "id_type_of_face";
            HistoryLogger.Log(
               "Загружены комбобоксы городов и типов лица");
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Название контрагента
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                HistoryLogger.Log(
                   "Ошибка добавления контрагента: не заполнено название");
                MessageBox.Show(
                    "Поле 'Название контрагента' обязательно для заполнения!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtName.Focus();
                return;
            }

            // Проверка названия
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                txtName.Text.Trim(),
                @"^[а-яА-Яa-zA-Z0-9\s\.\-""«»()]+$"))
            {
                HistoryLogger.Log(
                   $"Ошибка: недопустимые символы в названии '{txtName.Text}'");

                MessageBox.Show(
                    "Название контрагента содержит недопустимые символы!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtName.Focus();
                return;
            }

            // Город
            if (cmbCity.SelectedValue == null)
            {
                HistoryLogger.Log(
                   "Ошибка добавления контрагента: не выбран город");

                MessageBox.Show(
                    "Выберите город!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbCity.Focus();
                return;
            }

            // Тип лица
            if (cmbTypeFace.SelectedValue == null)
            {
                HistoryLogger.Log(
                    "Ошибка добавления контрагента: не выбран тип лица");

                MessageBox.Show(
                    "Выберите тип лица!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbTypeFace.Focus();
                return;
            }

            // Фактический адрес
            if (string.IsNullOrWhiteSpace(txtActualAddress.Text))
            {
                HistoryLogger.Log(
                   "Ошибка добавления контрагента: не заполнен фактический адрес");
                MessageBox.Show(
                    "Поле 'Фактический адрес' обязательно для заполнения!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtActualAddress.Focus();
                return;
            }

            // Проверка адреса
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                txtActualAddress.Text.Trim(),
                @"^[а-яА-Яa-zA-Z0-9\s\.,\-\/№]+$"))
            {
                HistoryLogger.Log(
                    $"Ошибка: недопустимые символы в адресе '{txtActualAddress.Text}'");

                MessageBox.Show(
                    "Фактический адрес содержит недопустимые символы!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtActualAddress.Focus();
                return;
            }

            // ИНН
            if (string.IsNullOrWhiteSpace(txtINN.Text))
            {
                HistoryLogger.Log(
                   "Ошибка добавления контрагента: не заполнен ИНН");
                MessageBox.Show(
                    "Поле 'ИНН' обязательно для заполнения!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtINN.Focus();
                return;
            }

            // Проверка ИНН
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                txtINN.Text.Trim(),
                @"^\d{10}(\d{2})?$"))
            {
               
                HistoryLogger.Log(
                  $"Ошибка: неверный ИНН '{txtINN.Text}'");
                MessageBox.Show(
                    "ИНН должен содержать 10 или 12 цифр!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtINN.Focus();
                return;
            }

            // БИК
            if (string.IsNullOrWhiteSpace(txtBIC.Text))
            {
                HistoryLogger.Log (
                    "Ошибка добавления контрагента: не заполнен БИК");
                MessageBox.Show(
                    "Поле 'БИК' обязательно для заполнения!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtBIC.Focus();
                return;
            }

            // Проверка БИК
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                txtBIC.Text.Trim(),
                @"^\d{9}$"))
            {
                HistoryLogger.Log(
                    $"Ошибка: неверный БИК '{txtBIC.Text}'");

                MessageBox.Show(
                    "БИК должен содержать 9 цифр!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtBIC.Focus();
                return;
            }

            // Email
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(
                    txtEmail.Text.Trim(),
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    HistoryLogger.Log(
                       $"Ошибка: неверный email '{txtEmail.Text}'");
                    MessageBox.Show(
                        "Некорректный Email!",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    txtEmail.Focus();
                    return;
                }
            }

            // Телефон контактного лица
            if (!string.IsNullOrWhiteSpace(txtContPhone.Text))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(
                    txtContPhone.Text.Trim(),
                    @"^[0-9\+\-\(\)\s]+$"))
                {
                    HistoryLogger.Log(
                       $"Ошибка: неверный телефон '{txtContPhone.Text}'");
                    MessageBox.Show(
                        "Телефон содержит недопустимые символы!",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    txtContPhone.Focus();
                    return;
                }
            }

          
            if (counterparty.IsDuplicate(
                txtName.Text.Trim(),
                txtINN.Text.Trim(),
                txtLegalAddress.Text.Trim()))
            {
                HistoryLogger.Log(
                   $"Попытка добавить дубликат контрагента '{txtName.Text}'");
                MessageBox.Show(
                    "Контрагент с таким названием или ИНН уже существует!",
                    "Дубликат",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                HistoryLogger.Log(
                    $"Начато добавление контрагента '{txtName.Text}'");
                bool success = counterparty.AddCounterparty(
                    name: txtName.Text.Trim(),
                    id_city: Convert.ToInt32(cmbCity.SelectedValue),
                    actual_address: txtActualAddress.Text.Trim(),
                    legal_address: txtLegalAddress.Text.Trim(),
                    email: txtEmail.Text.Trim(),
                    web_page: txtWebPage?.Text?.Trim() ?? "",
                    inn: txtINN.Text.Trim(),
                    bic: txtBIC.Text.Trim(),
                    id_type_of_face: Convert.ToInt32(cmbTypeFace.SelectedValue),
                    cont_person_name: txtContName.Text.Trim(),
                    cont_person_phone: txtContPhone.Text.Trim()
                );

                if (success)
                {
                    HistoryLogger.Log(
                        $"Контрагент успешно добавлен: '{txtName.Text.Trim()}', ИНН: {txtINN.Text.Trim()}");
                    MessageBox.Show(
                        "Контрагент успешно добавлен!",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                HistoryLogger.Log(
                    $"Ошибка при добавлении контрагента: {ex.Message}");
                MessageBox.Show(
                    $"Ошибка при добавлении контрагента:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        
        }
    }
}
