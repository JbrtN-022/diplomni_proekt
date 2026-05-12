using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для Редактирование__лицензий_на_ПО.xaml
    /// </summary>
    public partial class Редактирование__лицензий_на_ПО : Window
    {
        private int _idLicenseAgreement;

        public Редактирование__лицензий_на_ПО(int idLicenseAgreement)
        {
            InitializeComponent();

            _idLicenseAgreement = idLicenseAgreement;

            LoadClients();
            LoadProgram();
            LoadServices();

            LoadLicenseData();
        }

        private void LoadServices()
        {
            licenseAgreement.LoadAllServices(
                dgAllServices,
                dgAllServices);
        }

        private void LoadClients()
        {
            guideBD.selectCounterpartyComboBox();

            cmbClient.ItemsSource =
                ConnectionBD.dtCounterpartyComboBox.DefaultView;

            cmbClient.DisplayMemberPath = "name";

            cmbClient.SelectedValuePath =
                "id_counterparty";
        }

        private void LoadProgram()
        {
            guideBD.selectPrograms();

            cmbProgram.ItemsSource =
                ConnectionBD.dtProgramsComboBox.DefaultView;

            cmbProgram.DisplayMemberPath = "name";

            cmbProgram.SelectedValuePath =
                "id_program";
        }

        private void LoadLicenseData()
        {
            DataRow row =
                licenseAgreement.LoadDraftById(
                    _idLicenseAgreement);

            if (row == null)
            {
                MessageBox.Show(
                    "Лицензия не найдена!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Close();

                return;
            }

            txtLicenseNumber.Text =
                row["id_license_agreement"]
                .ToString();

            if (row["id_counterparty"] != DBNull.Value)
            {
                cmbClient.SelectedValue =
                    Convert.ToInt32(
                        row["id_counterparty"]);
            }

            if (row["id_program"] != DBNull.Value)
            {
                cmbProgram.SelectedValue =
                    Convert.ToInt32(
                        row["id_program"]);
            }

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
                row["approved"].ToString()
                == "Утверждён";

            LoadSelectedServicesForLicense(
                _idLicenseAgreement);
        }

        private void LoadSelectedServicesForLicense(
            int licenseId)
        {
            if (dgAllServices.ItemsSource
                is List<licenseAgreement.ServiceItem>
                allServices)
            {
                foreach (var service in allServices)
                {
                    service.IsSelected = false;
                    service.Quantity = 0;
                    service.IdWorker = null;
                }

                string sql = @"
SELECT
    id_services,
    kolvo,
    id_workers
FROM rccs.service_in_agreement
WHERE id_license_agreement = @id";

                ConnectionBD.mycommand.CommandText =
                    sql;

                ConnectionBD.mycommand.Parameters.Clear();

                ConnectionBD.mycommand.Parameters
                    .AddWithValue(
                        "@id",
                        licenseId);

                ConnectionBD.dtTemp.Clear();

                ConnectionBD.myDataAdapter.Fill(
                    ConnectionBD.dtTemp);

                foreach (DataRow row
                    in ConnectionBD.dtTemp.Rows)
                {
                    int serviceId =
                        Convert.ToInt32(
                            row["id_services"]);

                    var service =
                        allServices.FirstOrDefault(
                            s =>
                            s.IdServices
                            == serviceId);

                    if (service != null)
                    {
                        service.IsSelected = true;

                        service.Quantity =
                            row["kolvo"]
                            != DBNull.Value
                            ? Convert.ToInt32(
                                row["kolvo"])
                            : 1;

                        service.IdWorker =
                            row["id_workers"]
                            != DBNull.Value
                            ? Convert.ToInt32(
                                row["id_workers"])
                            : (int?)null;
                    }
                }

                dgAllServices.Items.Refresh();
            }
        }

        private List<licenseAgreement.ServiceItem>
            GetSelectedServices()
        {
            dgAllServices.CommitEdit(
                DataGridEditingUnit.Cell,
                true);

            dgAllServices.CommitEdit(
                DataGridEditingUnit.Row,
                true);

            var list =
                dgAllServices.ItemsSource
                as List<
                    licenseAgreement.ServiceItem>;

            if (list == null)
            {
                return
                    new List<
                        licenseAgreement.ServiceItem>();
            }

            return list
                .Where(s =>
                    s.IsSelected
                    && s.Quantity > 0)
                .ToList();
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

                return;
            }

            if (cmbProgram.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите программу!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (dpStart.SelectedDate == null
                || dpEnd.SelectedDate == null)
            {
                MessageBox.Show(
                    "Укажите даты!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var selectedServices =
                GetSelectedServices();

            if (selectedServices.Count == 0)
            {
                MessageBox.Show(
                    "Выберите услуги!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            foreach (var s in selectedServices)
            {
                if (s.Quantity <= 0)
                {
                    MessageBox.Show(
                        $"У услуги \"{s.Name}\" не указано количество!",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }
            }

            string approved =
                chkApproved.IsChecked == true
                ? "Утверждён"
                : "Не утверждён";

            bool result =
                licenseAgreement.UpdateLicenseAgreement(
                    _idLicenseAgreement,
                    Convert.ToInt32(
                        cmbClient.SelectedValue),
                    Convert.ToInt32(
                        cmbProgram.SelectedValue),
                    dpStart.SelectedDate.Value,
                    dpEnd.SelectedDate.Value,
                    approved);

            if (result)
            {
                licenseAgreement
                    .DeleteServicesFromLicense(
                        _idLicenseAgreement);

                licenseAgreement
                    .AddServicesToLicense(
                        _idLicenseAgreement,
                        selectedServices);

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

        private void Back_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}