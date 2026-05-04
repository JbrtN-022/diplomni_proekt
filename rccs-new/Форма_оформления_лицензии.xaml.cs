using rccs.MyClass;
using rccs_new.MyClass;
using rccs_new.MyClass.document;
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
using static rccs_new.MyClass.licenseAgreement;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для Форма_оформления_лицензии.xaml
    /// </summary>
    public partial class Форма_оформления_лицензии : Window
    {
        private bool isFirstOpen = true;
        public Форма_оформления_лицензии()
        {
            InitializeComponent();
            GenerateLicenseNumber();
            LoadClients();
            LoadProgram();
            LoadServices();
            licenseAgreement.LoadDraftLicensesComboBox(cmbDraft);
        }
        private List<ServiceItem> GetSelectedServices()
        {
            var selected = new List<ServiceItem>();

            foreach (var item in dgAllServices.Items)
            {
                if (item is ServiceItem service && service.IsSelected)
                {
                    selected.Add(service);
                }
            }
            return selected;
        }
        private void LoadServices()
        {
            licenseAgreement.LoadAllServices(dgAllServices);
        }
        private void LoadClients()
        {
            guideBD.selectCounterpartyComboBox();

            cmbClient.ItemsSource = ConnectionBD.dtCounterpartyComboBox.DefaultView;
            cmbClient.DisplayMemberPath = "name";
            cmbClient.SelectedValuePath = "id_counterparty";
        }
        private void LoadProgram()
        {
            guideBD.selectPrograms();

            cmbProgram.ItemsSource = ConnectionBD.dtProgramsComboBox.DefaultView;
            cmbProgram.DisplayMemberPath = "name";
            cmbProgram.SelectedValuePath = "id_program";
        }
        private void GenerateLicenseNumber()
        {
            Random rnd = new Random();
            int licenseValue = rnd.Next(0, 100000); 
            txtLicenseNumber.Text = licenseValue.ToString("D5");
        }
        



        private void cmbClient_DropDownOpened(object sender, EventArgs e)
        {
            if (cmbClient.Items.Count == 0)
            {
                var result = MessageBox.Show(
                    "В базе нет ни одного контрагента.\n\nДобавить нового контрагента сейчас?",
                    "Нет контрагентов",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    OpenAddClientForm();
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            форма_менеджера formMemedjer = new форма_менеджера();
            Application.Current.MainWindow = formMemedjer;
            formMemedjer.Show();
            this.Close();
        }
        private void OpenAddClientForm()
        {
            var addForm = new добавление_контрагента();
            addForm.ShowDialog();
            LoadClients();
            if (cmbClient.Items.Count > 0)
            {
                cmbClient.SelectedIndex = cmbClient.Items.Count - 1;
            }
        }
        private void btnAddNewClient_Click(object sender, RoutedEventArgs e)
        {
            OpenAddClientForm();
        }
        private void ClearForm()
        {

            GenerateLicenseNumber();

            cmbClient.SelectedIndex = -1;
            cmbProgram.SelectedIndex = -1;

            dpStart.SelectedDate = null;
            dpEnd.SelectedDate = null;

            if (dgAllServices.ItemsSource is List<ServiceItem> services)
            {
                foreach (var service in services)
                {
                    service.IsSelected = false;
                }
                dgAllServices.Items.Refresh();
            }
            cmbClient.Focus();

        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            
            if (cmbClient.SelectedValue == null)
            {
                MessageBox.Show("Выберите контрагента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbClient.Focus();
                return;
            }

            if (cmbProgram.SelectedValue == null)
            {
                MessageBox.Show("Выберите программу!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbProgram.Focus();
                return;
            }

            if (dpStart.SelectedDate == null || dpEnd.SelectedDate == null)
            {
                MessageBox.Show("Укажите даты начала и окончания действия лицензии!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedServices = GetSelectedServices();
            if (selectedServices.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну услугу!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int idCounterparty = Convert.ToInt32(cmbClient.SelectedValue);
                int idProgram = Convert.ToInt32(cmbProgram.SelectedValue);
                string licenseNumber = txtLicenseNumber.Text.Trim();

               
                ConnectionBD.mycommand.CommandText = "SELECT id_workers FROM rccs.workers WHERE name = @fio";
                ConnectionBD.mycommand.Parameters.Clear();
                ConnectionBD.mycommand.Parameters.AddWithValue("@fio", ConnectionBD.resFio);
                object workerResult = ConnectionBD.mycommand.ExecuteScalar();
                int idWorker = workerResult != null ? Convert.ToInt32(workerResult) : 1;

                int licenseId;
                bool isDraftMode = false;
                // ==================== 1. УТВЕРЖДЕНИЕ ЧЕРНОВИКА ====================
                if (cmbDraft.SelectedItem is DataRowView row)
                {
                    licenseId = Convert.ToInt32(row["id_license_agreement"]);
                    isDraftMode = true;
                }

                if (isDraftMode)
        {
                    var draftRow = (DataRowView)cmbDraft.SelectedItem;
                    licenseId = Convert.ToInt32(draftRow["id_license_agreement"]);

                    bool approved = licenseAgreement.ApproveDraft(licenseId);
                    if (!approved)
                    {
                        MessageBox.Show("Не удалось утвердить черновик.", "Ошибка");
                        return;
                    }

                    MessageBox.Show($"Черновик №{licenseNumber} успешно утверждён как полноценная лицензия!", "Успех");
                }
                // ==================== 2. СОЗДАНИЕ НОВОЙ ЛИЦЕНЗИИ ====================
                else
                {

                    licenseId = licenseAgreement.AddlicenseAgreement(
                        id: licenseNumber,
                        IDcounterparty: idCounterparty,
                        IDprogram: idProgram,
                        dateFrom: dpStart.SelectedDate.Value,
                        dateUntil: dpEnd.SelectedDate.Value,
                        idWorker: idWorker
                    );

                    if (licenseId <= 0)
                    {
                        MessageBox.Show($"Не удалось создать новую лицензию. {isDraftMode.ToString()}", "Ошибка");
                        return;
                    }

                    licenseAgreement.AddServicesToLicense(licenseId, selectedServices);
                    MessageBox.Show($"Новая лицензия №{licenseNumber} успешно создана!", "Успех");
                }

                // ==================== ОБЩАЯ ЧАСТЬ====================
              
                ConnectionBD.mycommand.CommandText = "SELECT cont_person_name FROM rccs.counterparty WHERE id_counterparty = @id";
                ConnectionBD.mycommand.Parameters.Clear();
                ConnectionBD.mycommand.Parameters.AddWithValue("@id", idCounterparty);
                object contResult = ConnectionBD.mycommand.ExecuteScalar();
                string contPersonFio = contResult?.ToString() ?? "Не указано";

                string counterpartName = cmbClient.Text;

               
                string filePath = $@"E:\крошик\Договор_№{licenseNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.docx";
                document_license_agreement.CreateLicenseAgreement(filePath, licenseNumber, counterpartName, contPersonFio, dpEnd.SelectedDate.Value.ToString());

                MessageBox.Show($"Лицензия №{licenseNumber} успешно обработана!\nДокумент сохранён.",
                                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearForm();
                
                licenseAgreement.LoadDraftLicensesComboBox(cmbDraft);   // обновляем список черновиков
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при создании/утверждении лицензии:\n" + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClient.SelectedValue == null)
            {
                MessageBox.Show("Выберите контрагента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbClient.Focus();
                return;
            }

            if (cmbProgram.SelectedValue == null)
            {
                MessageBox.Show("Выберите программу!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbProgram.Focus();
                return;
            }

            if (dpStart.SelectedDate == null || dpEnd.SelectedDate == null)
            {
                MessageBox.Show("Укажите даты начала и окончания действия лицензии!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedServices = GetSelectedServices();

            if (selectedServices.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну услугу!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int idCounterparty = Convert.ToInt32(cmbClient.SelectedValue);
                int idProgram = Convert.ToInt32(cmbProgram.SelectedValue);
                string licenseNumber = txtLicenseNumber.Text.Trim();

              
                ConnectionBD.mycommand.CommandText = "SELECT id_workers FROM rccs.workers WHERE name = @fio";
                ConnectionBD.mycommand.Parameters.Clear();
                ConnectionBD.mycommand.Parameters.AddWithValue("@fio", ConnectionBD.resFio);

                object workerResult = ConnectionBD.mycommand.ExecuteScalar();
                int idWorker = workerResult != null ? Convert.ToInt32(workerResult) : 1;

                int newLicenseId = licenseAgreement.SaveAddlicenseAgreement(
                    id: licenseNumber,
                    IDcounterparty: idCounterparty,
                    IDprogram: idProgram,
                    dateFrom: dpStart.SelectedDate.Value,
                    dateUntil: dpEnd.SelectedDate.Value,
                    idWorker: idWorker
                );

                if (newLicenseId > 0)
                {
                    licenseAgreement.AddServicesToLicense(newLicenseId, selectedServices);

                    MessageBox.Show($"Черновик лицензии №{licenseNumber} успешно сохранён!", "Успех");

                    
                    licenseAgreement.LoadDraftLicensesComboBox(cmbDraft);

                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Не удалось сохранить черновик.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении черновика:\n" + ex.Message, "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmbDraft_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDraft.SelectedItem == null || e.AddedItems.Count == 0)
            {
                
                return;
            }

            int licenseId;

            try
            {
                licenseId = Convert.ToInt32(cmbDraft.SelectedValue);
            }
            catch
            {
                return; 
            }

            LoadClients();
            LoadProgram();

            LoadDraftIntoForm(licenseId);
          
        }


        private void LoadDraftIntoForm(int licenseId)
        {
            DataRow row = licenseAgreement.LoadDraftById(licenseId);

            if (row == null)
            {
                MessageBox.Show("Не удалось загрузить данные черновика.", "Ошибка");
                return;
            }

            txtLicenseNumber.Text = row["id_license_agreement"].ToString();

            if (row["id_counterparty"] != DBNull.Value)
                cmbClient.SelectedValue = Convert.ToInt32(row["id_counterparty"]);

            if (row["id_program"] != DBNull.Value)
                cmbProgram.SelectedValue = Convert.ToInt32(row["id_program"]);

            if (row["rental_date_from"] != DBNull.Value)
                dpStart.SelectedDate = Convert.ToDateTime(row["rental_date_from"]);

            if (row["rental_date_until"] != DBNull.Value)
                dpEnd.SelectedDate = Convert.ToDateTime(row["rental_date_until"]);

            LoadSelectedServicesForLicense(licenseId);
        }
        private void LoadSelectedServicesForLicense(int licenseId)
        {
            // Снимаем все галочки
            if (dgAllServices.ItemsSource is List<ServiceItem> allServices)
            {
                foreach (var service in allServices)
                    service.IsSelected = false;

                // Загружаем, какие услуги уже выбраны в этом черновике
                string sql = @"
            SELECT id_services 
            FROM rccs.service_in_agreement 
            WHERE id_license_agreement = @id";

                ConnectionBD.mycommand.CommandText = sql;
                ConnectionBD.mycommand.Parameters.Clear();
                ConnectionBD.mycommand.Parameters.AddWithValue("@id", licenseId);

                ConnectionBD.dtTemp.Clear();
                ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

                foreach (DataRow row in ConnectionBD.dtTemp.Rows)
                {
                    int serviceId = Convert.ToInt32(row["id_services"]);
                    var service = allServices.FirstOrDefault(s => s.IdServices == serviceId);
                    if (service != null)
                        service.IsSelected = true;
                }

                dgAllServices.Items.Refresh();
            }
        }
    }
}
