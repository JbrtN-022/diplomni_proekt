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
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Поле 'Название контрагента' обязательно для заполнения!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
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

            if (string.IsNullOrWhiteSpace(txtINN.Text))
            {
                MessageBox.Show("Поле 'ИНН' обязательно для заполнения!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                txtINN.Focus();
                return;
            }

            // Проверка на дублирование (вызывается из класса)
            if (counterparty.IsDuplicate(txtName.Text.Trim(), txtINN.Text.Trim(), txtLegalAddress.Text.Trim()))
            {
                MessageBox.Show("Контрагент с таким названием или ИНН уже существует!",
                                "Дубликат", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
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
                    MessageBox.Show("Контрагент успешно добавлен!", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении контрагента:\n{ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
