using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для редактирование_контрагентов.xaml
    /// </summary>
    public partial class редактирование_контрагентов : Window
    {
        private int currenCounterpartId;
        public редактирование_контрагентов(int id_workers)
        {
            InitializeComponent();
            currenCounterpartId = id_workers;
            LoadComboBoxes();
            LoadDataForEdit();
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
            //if (counterparty.IsDuplicate(txtName.Text.Trim(), txtINN.Text.Trim(), txtLegalAddress.Text.Trim()))
            //{
            //    MessageBox.Show("Контрагент с таким названием и ИНН уже существует!",
            //                    "Дубликат", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            try
            {
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
                    MessageBox.Show("Данные контрагента успешно обновлены!", "Успех",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось обновить данные.", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
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

                // Тип лица
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
                MessageBox.Show("Не удалось загрузить данные контрагента.", "Ошибка");
            }
        
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
    }
}
