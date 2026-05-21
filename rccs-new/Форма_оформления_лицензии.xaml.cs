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
    
    public partial class Форма_оформления_лицензии : Window
    {
        private bool isFirstOpen = true;
        private bool isDraftLoaded = false;
        public Форма_оформления_лицензии()
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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму оформления лицензии на ПО");

            GenerateLicenseNumber();
            LoadClients();
            LoadProgram();
            LoadServices();
            licenseAgreement.LoadDraftLicensesComboBox(cmbDraft);
        }
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА ОФОРМЛЕНИЯ ЛИЦЕНЗИИ

Назначение формы:
Оформление лицензионных договоров на программное обеспечение.

Что можно сделать на этой форме:
• Создать новую лицензию
• Выбрать контрагента (клиента)
• Выбрать программу лицензирования
• Выбрать дополнительные услуги
• Установить срок действия лицензии
• Сохранить черновик лицензии
• Загрузить ранее сохраненный черновик
• Создать и сохранить договор в формате Word

Поля для заполнения:

1. НОМЕР ЛИЦЕНЗИИ
   • Генерируется автоматически
   • Уникальный 5-значный номер

2. КОНТРАГЕНТ
   • Выбор из списка существующих
   • Кнопка ""+"" для добавления нового

3. ПРОГРАММА
   • Выбор лицензируемого ПО из списка

4. СРОК ДЕЙСТВИЯ
   • Дата начала лицензии
   • Дата окончания лицензии

5. УСЛУГИ
   • Таблица с доступными услугами
   • Выберите нужные услуги (галочка)
   • Укажите количество для каждой услуги

Кнопки управления:

• СОЗДАТЬ - создает лицензию и формирует Word-договор
• СОХРАНИТЬ - сохраняет черновик лицензии
• ЗАГРУЗИТЬ - загружает сохраненный черновик из списка
• НАЗАД - возврат в главное меню



Примечание:
Лицензионный договор создается в формате Word и сохраняется в выбранную папку.
Черновики позволяют отложить оформление и завершить позже.
Перед созданием лицензии убедитесь, что все обязательные поля заполнены.
Все услуги с отмеченными галочками и количеством будут включены в договор.",
                "Помощь - Форма оформления лицензии",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        private List<ServiceItem> GetSelectedServices()
        {
            dgAllServices.CommitEdit(DataGridEditingUnit.Cell, true);
            dgAllServices.CommitEdit(DataGridEditingUnit.Row, true);

            var list = dgAllServices.ItemsSource as List<ServiceItem>;

            if (list == null)
                return new List<ServiceItem>();

            return list
                .Where(s => s.IsSelected && s.Quantity > 0)
                .ToList();
        }
        private void LoadServices()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список всех услуг");

            licenseAgreement.LoadAllServices(dgAllServices, dgAllServices);
        }
        private void LoadClients()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список контрагентов");

            guideBD.selectCounterpartyComboBox();

            cmbClient.ItemsSource = ConnectionBD.dtCounterpartyComboBox.DefaultView;
            cmbClient.DisplayMemberPath = "name";
            cmbClient.SelectedValuePath = "id_counterparty";
        }
        private void LoadProgram()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список программ");

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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} сгенерировал новый номер лицензии: {txtLicenseNumber.Text}");
        }




        private void cmbClient_DropDownOpened(object sender, EventArgs e)
        {
            if (cmbClient.Items.Count == 0)
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} попытался открыть список контрагентов — список пуст");

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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму добавления нового контрагента");

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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} нажал кнопку добавления нового контрагента");
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
                    service.Quantity = 0;     
                    service.IdWorker = null;
                }
                dgAllServices.Items.Refresh();
            }
            cmbClient.Focus();
            isDraftLoaded = false;
        }
       
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} начал создание лицензии. Номер: {txtLicenseNumber.Text}");

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
                MessageBox.Show("Укажите даты!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            dgAllServices.CommitEdit(DataGridEditingUnit.Cell, true);
            dgAllServices.CommitEdit(DataGridEditingUnit.Row, true);

            var selectedServices = GetSelectedServices();

            if (selectedServices.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну услугу!", "Ошибка");
                return;
            }

            foreach (var s in selectedServices)
            {
                if (s.Quantity <= 0)
                {
                    MessageBox.Show($"У услуги \"{s.Name}\" не указано количество!");
                    return;
                }
            }

            try
            {
                int idCounterparty = Convert.ToInt32(cmbClient.SelectedValue);
                int idProgram = Convert.ToInt32(cmbProgram.SelectedValue);
                string licenseNumber = txtLicenseNumber.Text.Trim();

                ConnectionBD.mycommand.CommandText =
                    "SELECT id_workers FROM rccs.workers WHERE name = @fio";

                ConnectionBD.mycommand.Parameters.Clear();
                ConnectionBD.mycommand.Parameters.AddWithValue("@fio", ConnectionBD.resFio);

                object workerResult = ConnectionBD.mycommand.ExecuteScalar();

                int idWorker =
                    workerResult != null
                    ? Convert.ToInt32(workerResult)
                    : 1;

                string licenseId = "";

                // ЕСЛИ ЗАГРУЖЕН ЧЕРНОВИК
                if (isDraftLoaded)
                {
                    bool updated =
                        licenseAgreement.UpdateLicenseAgreement(
                            idLicenseAgreement: Convert.ToInt32(licenseNumber),
                            idCounterparty: idCounterparty,
                            idProgram: idProgram,
                            dateFrom: dpStart.SelectedDate.Value,
                            dateUntil: dpEnd.SelectedDate.Value,
                            approved: "Утверждён"
                        );

                    if (!updated)
                    {
                        MessageBox.Show("Не удалось обновить черновик!");
                        return;
                    }

                    // удаляем старые услуги
                    licenseAgreement.DeleteServicesFromLicense(
                        Convert.ToInt32(licenseNumber));

                    licenseId = licenseNumber;
                }
                // ЕСЛИ НОВАЯ ЛИЦЕНЗИЯ
                else
                {
                    licenseId =
                        licenseAgreement.AddlicenseAgreement(
                            id: licenseNumber,
                            IDcounterparty: idCounterparty,
                            IDprogram: idProgram,
                            dateFrom: dpStart.SelectedDate.Value,
                            dateUntil: dpEnd.SelectedDate.Value,
                            idWorker: idWorker
                        );

                    if (string.IsNullOrEmpty(licenseId))
                    {
                        MessageBox.Show("Ошибка создания лицензии");
                        return;
                    }
                }

                // добавляем услуги
                licenseAgreement.AddServicesToLicense(
                    int.Parse(licenseId),
                    selectedServices);

                DataRow licenseRow =
                    licenseAgreement.GetLicenseAgreementForPrint(
                        int.Parse(licenseId));

                DataTable servicesTable =
                    licenseAgreement.GetLicenseAgreementForPrintTable(
                        int.Parse(licenseId));

                var saveDialog = new Microsoft.Win32.SaveFileDialog();

                saveDialog.Filter =
                    "Word документ (*.docx)|*.docx";

                saveDialog.FileName =
                    $"Договор_{licenseNumber}.docx";

                if (saveDialog.ShowDialog() != true)
                    return;

                string filePath = saveDialog.FileName;

                string baseDirectory =
                    AppDomain.CurrentDomain.BaseDirectory;

                string projectDirectory =
                    System.IO.Path.GetFullPath(
                        System.IO.Path.Combine(
                            baseDirectory,
                            @"..\..\"));


                string documentPath =
                    System.IO.Path.Combine(
                        projectDirectory,
                        "document");

                string templatePath =
                    System.IO.Path.Combine(
                        documentPath,
                        "Шаблон_ТС_А0_2026_с_НДС.docx");

                bool result =
                    document_license_agreement.CreateLicenseAgreement(
                        templatePath,
                        filePath,
                        licenseNumber,
                        licenseRow["counterparty_name"]?.ToString() ?? "",
                        licenseRow["cont_person_name"]?.ToString() ?? "",
                        licenseRow["program_name"]?.ToString() ?? "",
                        Convert.ToDateTime(
                            licenseRow["rental_date_from"])
                            .ToShortDateString(),

                        Convert.ToDateTime(
                            licenseRow["rental_date_until"])
                            .ToShortDateString(),

                        licenseRow["conclusion_date"] != DBNull.Value
                        ? Convert.ToDateTime(
                            licenseRow["conclusion_date"])
                            .ToShortDateString()
                        : DateTime.Today.ToShortDateString(),

                        licenseId
                    );

                if (result)
                {
                    HistoryLogger.Log(
                        $"Пользователь {ConnectionBD.resFio} успешно создал и сохранил лицензию №{licenseNumber} (ID: {licenseId})");

                    MessageBox.Show("Документ успешно создан!");

                    string folder =
                        System.IO.Path.GetDirectoryName(filePath);

                    System.Diagnostics.Process.Start(
                        "explorer.exe",
                        folder);
                }

                ClearForm();

                licenseAgreement.LoadDraftLicensesComboBox(cmbDraft);
            }
            catch (Exception ex)
            {
                HistoryLogger.Log(
                    $"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог создать лицензию. Ошибка: {ex.Message}");

                MessageBox.Show("Ошибка:\n" + ex.Message);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} начал сохранение черновика лицензии №{txtLicenseNumber.Text}");

            
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
            foreach (var s in selectedServices)
            {
                if (s.Quantity <= 0)
                {
                    MessageBox.Show($"У услуги \"{s.Name}\" не указано количество!",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
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

                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно сохранил черновик лицензии №{licenseNumber} (ID: {newLicenseId})");
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
                HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог сохранить черновик. Ошибка: {ex.Message}");
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

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} выбрал для загрузки черновик лицензии ID: {licenseId}");

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

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно загрузил черновик лицензии ID: {licenseId}");

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
            isDraftLoaded = true;
        }
        private void LoadSelectedServicesForLicense(int licenseId)
        {
            if (dgAllServices.ItemsSource is List<ServiceItem> allServices)
            {
                
                foreach (var service in allServices)
                {
                    service.IsSelected = false;
                    service.Quantity = 0;
                    service.IdWorker = null;
                }

                string sql = @"
        SELECT id_services, kolvo, id_workers
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
                    {
                        service.IsSelected = true;

                        service.Quantity = row["kolvo"] != DBNull.Value
                            ? Convert.ToInt32(row["kolvo"])
                            : 1;

                        service.IdWorker = row["id_workers"] != DBNull.Value
                            ? Convert.ToInt32(row["id_workers"])
                            : (int?)null;
                    }
                }

                dgAllServices.Items.Refresh();
            }
        }
    }
}
