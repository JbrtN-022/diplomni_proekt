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
    /// Логика взаимодействия для Редактирование_договоров__аренды.xaml
    /// </summary>
    public partial class Редактирование_договоров__аренды : Window
    {
        private int _idLeaseAgreement;
        public Редактирование_договоров__аренды(int idLeaseAgreement)
        {
            _idLeaseAgreement = idLeaseAgreement;

            InitializeComponent();
            LoadClients();
            LoadFloor();
          

            LoadData();
        }
        private void LoadData()
        {
            DataRow row =
                leaseAgreement.LoadDraftById(
                    _idLeaseAgreement);
           
            if (row == null)
            {
                MessageBox.Show(
                    "Договор не найден!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Close();
                return;
            }

            txtLicenseNumber.Text =
                row["id_lease_agreement"].ToString();
            cmbClient.SelectedValue =
                row["id_counterparty"].ToString();
            cmbFloor.Text =
                row["floor_name"].ToString();
           

            cmbRoom.Text =
                row["office_name"].ToString();
            if (row["rental_date_from"] != DBNull.Value)
            {
                dpStart.SelectedDate =
                    Convert.ToDateTime(
                        row["rental_date_from"]);
            }

            if (row["rental_date_until"] != DBNull.Value)
            {
                dpEnd.SelectedDate =
                    Convert.ToDateTime(
                        row["rental_date_until"]);
            }
            chkApproved.IsChecked =
                row["approved"].ToString() == "Утверждён";
        }
        private void LoadClients()
        {
            guideBD.selectCounterpartyComboBox();

            cmbClient.ItemsSource = ConnectionBD.dtCounterpartyComboBox.DefaultView;
            cmbClient.DisplayMemberPath = "name";
            cmbClient.SelectedValuePath = "id_counterparty";
        }
       
        private void LoadFloor()
        {
            guideBD.selectEtaj();
            cmbFloor.ItemsSource = ConnectionBD.dtEtaj.DefaultView;
            cmbFloor.DisplayMemberPath = "floor";
            cmbFloor.SelectedValuePath = "id_floor";
        }
        private void LoadRoom(int? room)
        {
            leaseAgreement.SelectComboBoxRoom(room);
            cmbRoom.ItemsSource = ConnectionBD.dtFloorForLeaseComboBox.DefaultView;
            cmbRoom.DisplayMemberPath = "office";
            cmbRoom.SelectedValuePath = "id_room";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClient.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите контрагента!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbClient.Focus();

                return;
            }
            if (cmbFloor.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите этаж!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbFloor.Focus();

                return;
            }
            if (cmbRoom.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите помещение!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbRoom.Focus();

                return;
            }
            if (dpStart.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите дату начала!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                dpStart.Focus();

                return;
            }
            if (dpEnd.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите дату окончания!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                dpEnd.Focus();

                return;
            }
            string approved =
                chkApproved.IsChecked == true
                ? "Утверждён"
                : "Не утверждён";

            bool result =
                leaseAgreement.UpdateLeaseAgreement(
                    _idLeaseAgreement,
                    Convert.ToInt32(cmbClient.SelectedValue),
                    Convert.ToInt32(cmbRoom.SelectedValue),
                    dpStart.SelectedDate.Value,
                    dpEnd.SelectedDate.Value,
                    approved);

            if (result)
            {
                MessageBox.Show(
                    "Изменения сохранены!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Close();
            }
            else
            {
                MessageBox.Show(
                    "Ошибка сохранения!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmbFloor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFloor.SelectedItem is DataRowView row)
            {
                int floorNumber = Convert.ToInt32(row["floor"]);

                MessageBox.Show(floorNumber.ToString());
                LoadRoom(floorNumber);
              
            }
            else
            {
                LoadRoom(null);
                
            }
        }
    }
}
