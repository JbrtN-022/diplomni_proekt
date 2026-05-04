using DocumentFormat.OpenXml.Bibliography;
using rccs.MyClass;
using rccs_new.MyClass;
using rccs_new.MyClass.document;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Логика взаимодействия для создание_договора_аренды.xaml
    /// </summary>
    public partial class создание_договора_аренды : Window
    {
        public создание_договора_аренды()
        {
            InitializeComponent();
            LoadClients();
            LoadFloor();
            LoadRoom(null);
            GenerateLieseNumber();
            leaseAgreement.LoadDraftLeases(cmbDraft);
        }
        private void LoadClients()
        {
            guideBD.selectCounterpartyComboBox();

            cmbClient.ItemsSource = ConnectionBD.dtCounterpartyComboBox.DefaultView;
            cmbClient.DisplayMemberPath = "name";
            cmbClient.SelectedValuePath = "id_counterparty";
        }
        private void GenerateLieseNumber()
        {
            Random rnd = new Random();
            int licenseValue = rnd.Next(0, 1000);
            txtLicenseNumber.Text = licenseValue.ToString("D3");
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

        private void btnAddNewClient_Click(object sender, RoutedEventArgs e)
        {
            OpenAddClientForm();
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
       

        private void cmbFloor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFloor.SelectedItem is DataRowView row)
            {
                int floorNumber = Convert.ToInt32(row["floor"]);


                LoadRoom(floorNumber);
                LoadFloorImage(floorNumber);
            }
            else
            {
                LoadRoom(null);
                imgFloorPlan.Source = null;
            }
        }
        private void LoadFloorImage(int floorId)
        {
            string path = "";

            
            switch (floorId)
            {
                case 1:
                    path = @"E:\ДИПЛОМ\rccs\rccs-new\image\1 этаж_page-0001.jpg";
                    break;

                case 2:
                    path = @"E:\ДИПЛОМ\rccs\rccs-new\image\2 этаж_page-0001.jpg";
                    break;

                case 3:
                    path = @"E:\ДИПЛОМ\rccs\rccs-new\image\3 этаж_page-0001.jpg";
                    break;
                case 4:
                    path = @"E:\ДИПЛОМ\rccs\rccs-new\image\4 этаж_page-0001.jpg";
                    break;
                case 6:
                    path = @"E:\ДИПЛОМ\rccs\rccs-new\image\6 этаж_page-0001.jpg";
                    break;

            }

            if (File.Exists(path))
            {
                imgFloorPlan.Source = new BitmapImage(new Uri(path));
            }
            else
            {
                imgFloorPlan.Source = null;
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            форма_бухгалтера formBuch = new форма_бухгалтера();
            Application.Current.MainWindow = formBuch;
            formBuch.Show();

            this.Close();
        }
        private void cmbRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void cmbClient_DropDownOpened(object sender, EventArgs e)
        {

        }

        private void cmbDraft_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDraft.SelectedValue == null) return;

            int leaseId = Convert.ToInt32(cmbDraft.SelectedValue);
            LoadDraftIntoForm(leaseId);
        }
        private void LoadDraftIntoForm(int leaseId)
        {
            DataRow row = leaseAgreement.LoadDraftById(leaseId);
            if (row == null)
            {
                MessageBox.Show("Не удалось загрузить черновик.", "Ошибка");
                return;
            }

            txtLicenseNumber.Text = row["id_lease_agreement"].ToString();

            // Контрагент
            if (row["id_counterparty"] != DBNull.Value)
                cmbClient.SelectedValue = Convert.ToInt32(row["id_counterparty"]);

            // Этаж
            if (row["floor_name"] != DBNull.Value)
            {
                string floorName = row["floor_name"].ToString();
                cmbFloor.Text = floorName;

                int? floorId = GetFloorIdByName(floorName);
                if (floorId.HasValue)
                {
                    LoadRoom(floorId.Value);

                    
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (row["office_name"] != DBNull.Value)
                            cmbRoom.Text = row["office_name"].ToString();
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
            }


            if (row["rental_date_from"] != DBNull.Value)
            {
                if (DateTime.TryParse(row["rental_date_from"].ToString(), out DateTime startDate))
                    dpStart.SelectedDate = startDate;
            }

            if (row["rental_date_until"] != DBNull.Value)
            {
                if (DateTime.TryParse(row["rental_date_until"].ToString(), out DateTime endDate))
                    dpEnd.SelectedDate = endDate;
            }


        }
        private int? GetFloorIdByName(string floorName)
        {
            if (string.IsNullOrEmpty(floorName))
                return null;

            string sql = "SELECT id_floor FROM rccs.floor WHERE floor = @floorName";

            ConnectionBD.mycommand.CommandText = sql;
            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@floorName", floorName);

            object result = ConnectionBD.mycommand.ExecuteScalar();

            if (result != null && result != DBNull.Value)
                return Convert.ToInt32(result);

            return null;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            int leaseId = leaseAgreement.SaveAsDraft(
                txtLicenseNumber.Text.Trim(),
                Convert.ToInt32(cmbClient.SelectedValue),
                Convert.ToInt32(cmbRoom.SelectedValue),
                dpStart.SelectedDate.Value,
                dpEnd.SelectedDate.Value,
                1 
            );

            if (leaseId > 0)
            {
                MessageBox.Show($"Черновик №{txtLicenseNumber.Text} успешно сохранён!", "Успех");
                ClearForm();
                leaseAgreement.LoadDraftLeases(cmbDraft);
            }
            else
            {
                MessageBox.Show("Не удалось сохранить черновик.", "Ошибка");
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                string leaseNumber = txtLicenseNumber.Text.Trim();
                int idCounterparty = Convert.ToInt32(cmbClient.SelectedValue);
                int idRoom = Convert.ToInt32(cmbRoom.SelectedValue);

                int leaseId;

               
                if (cmbDraft.SelectedValue != null)
                {
                    leaseId = Convert.ToInt32(cmbDraft.SelectedValue);
                    if (!leaseAgreement.ApproveDraft(leaseId))
                    {
                        MessageBox.Show("Не удалось утвердить договор.", "Ошибка");
                        return;
                    }
                }
                else
                {
                    leaseId = leaseAgreement.AddLeaseAgreement(
                        leaseNumber, idCounterparty, idRoom,
                        dpStart.SelectedDate.Value, dpEnd.SelectedDate.Value, 1);

                    if (leaseId <= 0)
                    {
                        MessageBox.Show("Не удалось создать договор.", "Ошибка");
                        return;
                    }
                }

               
                Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Word Document (*.docx)|*.docx",
                    FileName = $"Договор_аренды_№{leaseNumber}_{DateTime.Now:yyyyMMdd}",
                    InitialDirectory = @"C:\"
                };

                if (saveDialog.ShowDialog() != true) return; // пользователь отменил сохранение

                string filePath = saveDialog.FileName;

                DataRow data = leaseAgreement.GetLeaseDataForPrint(leaseId);

                if (data == null)
                {
                    MessageBox.Show("Не удалось получить данные для печати договора.", "Ошибка");
                    return;
                }

               
                bool success = document_lease_premises.CreateLeasePremises(
                    templatePath: @"E:\ДИПЛОМ\rccs\rccs-new\document\Договор_аренды.docx", 
                    filePath: filePath,
                    printMode: 3, 
                    numContract: leaseNumber,
                    counterparty: data["counterpartyFirm"]?.ToString() ?? "",
                    counterpartName: data["cont_person_name"]?.ToString() ?? "",
                    dateFrom: Convert.ToDateTime(data["rental_date_from"]).ToShortDateString(),
                    dateUntil: Convert.ToDateTime(data["rental_date_until"]).ToShortDateString(),
                    dateConclusion: Convert.ToDateTime(data["conclusion_date"]).ToShortDateString(),
                    companyName: data["company"]?.ToString() ?? "",
                    actualadress: data["actual_address"]?.ToString() ?? "",
                    cops: data["COPC"]?.ToString() ?? "",
                    workerName: data["worker"]?.ToString() ?? "",
                    floor: data["floor"]?.ToString() ?? "",
                    office: data["office"]?.ToString() ?? "",
                    square: Convert.ToDecimal(data["square"]),
                    pricePerM2: Convert.ToDecimal(data["price_per_m2"]),
                    price: Convert.ToDecimal(data["price"]),
                    totalPrice: Convert.ToDecimal(data["total_price"]),
                    bic: data["BIC"]?.ToString() ?? "",
                    inn: data["INN"]?.ToString() ?? "",
                    correspondentAccount: data["Correspondent_account"]?.ToString() ?? "",
                    paymentAccount: data["Payment_account"]?.ToString() ?? ""
                );

                if (success)
                {
                    MessageBox.Show($"Договор №{leaseNumber} успешно создан и сохранён!\n{filePath}",
                                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
                    }
                    catch { }
                
                }
                else
                {
                    MessageBox.Show("Договор сохранён в базу, но возникла ошибка при создании Word-файла.", "Предупреждение");
                }

                ClearForm();
                leaseAgreement.LoadDraftLeases(cmbDraft);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ClearForm()
        {
            GenerateLieseNumber();
            cmbClient.SelectedIndex = -1;
            cmbRoom.SelectedIndex = -1;
            cmbFloor.SelectedIndex = -1;
            dpStart.SelectedDate = null;
            dpEnd.SelectedDate = null;
            imgFloorPlan.Source = null;

            
            cmbClient.Focus();
        }
        private bool ValidateInput()
        {
            if (cmbClient.SelectedValue == null || cmbRoom.SelectedValue == null)
            {
                MessageBox.Show("Выберите контрагента и помещение!", "Ошибка");
                return false;
            }

            if (dpStart.SelectedDate == null || dpEnd.SelectedDate == null)
            {
                MessageBox.Show("Укажите даты!", "Ошибка");
                return false;
            }

            if (dpStart.SelectedDate.Value >= dpEnd.SelectedDate.Value)
            {
                MessageBox.Show("Дата начала должна быть раньше даты окончания!", "Ошибка");
                return false;
            }

            return true;
        }

        private void RefreshAfterSave()
        {
            ClearForm();
            GenerateLieseNumber();
            leaseAgreement.LoadDraftLeases(cmbDraft);   
        }

        
    }
}
