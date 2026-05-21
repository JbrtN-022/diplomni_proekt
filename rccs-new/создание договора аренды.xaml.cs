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
    
    public partial class создание_договора_аренды : Window
    {
        public создание_договора_аренды()
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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму создания договора аренды");
            LoadClients();
            LoadFloor();
            LoadRoom(null);
            GenerateLieseNumber();
            leaseAgreement.LoadDraftLeases(cmbDraft);
        }
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА СОЗДАНИЯ ДОГОВОРА АРЕНДЫ

Назначение формы:
Создание нового договора аренды помещения.

Что можно сделать на этой форме:
• Выбрать контрагента (арендатора)
• Выбрать этаж и помещение
• Установить срок действия договора
• Просмотреть план этажа с помещениями
• Сохранить черновик договора
• Создать и распечатать готовый договор

Поля для заполнения:

1. НОМЕР ДОГОВОРА
   • Генерируется автоматически
   • 3-значный номер

2. КОНТРАГЕНТ (обязательное поле)
   • Выбор из списка существующих
   • Кнопка ""+"" для добавления нового

3. ЭТАЖ (обязательное поле)
   • Выбор из списка этажей
   • При выборе загружается план этажа

4. ПОМЕЩЕНИЕ (обязательное поле)
   • Выбор из списка помещений
   • Зависит от выбранного этажа

5. ПЛАН ЭТАЖА
   • Отображается при выборе этажа
   • Показывает расположение помещений

6. СРОК ДЕЙСТВИЯ (обязательное поле)
   • Дата начала аренды
   • Дата окончания аренды
   • Окончание должно быть позже начала

7. ЧЕРНОВИКИ
   • Выпадающий список сохраненных черновиков
   • Позволяет загрузить и продолжить работу

Кнопки управления:

• СОХРАНИТЬ - сохраняет черновик договора
• СОЗДАТЬ - создает готовый договор и формирует Word-документ
• НАЗАД - возврат в главное меню



Примечание:
Перед созданием договора убедитесь, что помещение свободно на выбранный период.
Черновики позволяют отложить оформление и завершить позже.
Договор аренды создается в формате Word и сохраняется в выбранную папку.
При создании договора из черновика он удаляется из списка черновиков.",
                "Помощь - Создание договора аренды",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        private void LoadClients()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список контрагентов");
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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список этажей");
            guideBD.selectEtaj();
            cmbFloor.ItemsSource = ConnectionBD.dtEtaj.DefaultView;
            cmbFloor.DisplayMemberPath = "floor";
            cmbFloor.SelectedValuePath = "id_floor";
        }
        private void LoadRoom(int? room, int? currentRoomId = null)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список помещений");

            leaseAgreement.SelectComboBoxRoom(room, currentRoomId);

            cmbRoom.ItemsSource = ConnectionBD.dtFloorForLeaseComboBox.DefaultView;
            cmbRoom.DisplayMemberPath = "office";
            cmbRoom.SelectedValuePath = "id_room";
        }

        private void btnAddNewClient_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму добавления контрагента");
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

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} выбрал этаж {floorNumber}");

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
            string baseDirectoryArt = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectoryArt = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDirectoryArt, @"..\..\"));
            string documentPathArt = System.IO.Path.Combine(projectDirectoryArt, "image");
            
            switch (floorId)
            {
                case 1:
                    path = System.IO.Path.Combine(documentPathArt, "1 этаж_page-0001.jpg"); 
                    break;

                case 2:
                    path = System.IO.Path.Combine(documentPathArt, "2 этаж_page-0001.jpg");

                    break;

                case 3:
                    path = System.IO.Path.Combine(documentPathArt, "3 этаж_page-0001.jpg");
                    
                    break;
                case 4:
                    path = System.IO.Path.Combine(documentPathArt, "4 этаж_page-0001.jpg");
                    break;
                case 6:
                    path = System.IO.Path.Combine(documentPathArt, "6 этаж_page-0001.jpg");
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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} вернулся в форму бухгалтера из формы создания договора аренды");
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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} выбрал черновик договора аренды ID {leaseId}");
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

            if (row["id_counterparty"] != DBNull.Value)
                cmbClient.SelectedValue = Convert.ToInt32(row["id_counterparty"]);

            if (row["id_floor"] != DBNull.Value)
            {
                int floorId = Convert.ToInt32(row["id_floor"]);

                cmbFloor.SelectedValue = floorId;

                LoadRoom(floorId, Convert.ToInt32(row["id_room"]));
                LoadFloorImage(floorId);
            }

           
            if (row["id_room"] != DBNull.Value)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    cmbRoom.SelectedValue = Convert.ToInt32(row["id_room"]);
                }),
                System.Windows.Threading.DispatcherPriority.Background);
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
      

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} сохраняет черновик договора аренды №{txtLicenseNumber.Text}");

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
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно сохранил черновик договора аренды ID {leaseId}");
                ClearForm();
                leaseAgreement.LoadDraftLeases(cmbDraft);
            }
            else
            {
                HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог сохранить черновик договора аренды");
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

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} начал создание договора аренды №{leaseNumber}");

                if (cmbDraft.SelectedValue != null)
                {
                    leaseId = Convert.ToInt32(cmbDraft.SelectedValue);
                    if (!leaseAgreement.ApproveDraft(leaseId))
                    {
                        HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог утвердить черновик договора ID {leaseId}");
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
                        HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог создать договор аренды №{leaseNumber}");
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

                if (saveDialog.ShowDialog() != true) return;

                string filePath = saveDialog.FileName;

                DataRow data = leaseAgreement.GetLeaseDataForPrint(leaseId);

                if (data == null)
                {
                    HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог получить данные для печати договора аренды ID {leaseId}");
                    MessageBox.Show("Не удалось получить данные для печати договора.", "Ошибка");
                    return;
                }

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDirectory, @"..\..\"));
                string documentPath = System.IO.Path.Combine(projectDirectory, "document");
                string templatePath = System.IO.Path.Combine(documentPath, "Договор_аренды.docx");

                bool success = document_lease_premises.CreateLeasePremises(
                     templatePath: templatePath,
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
                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно создал договор аренды №{leaseNumber}");
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
                    HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} создал договор аренды №{leaseNumber}, но Word-файл не был создан");
                    MessageBox.Show("Договор сохранён в базу, но возникла ошибка при создании Word-файла.", "Предупреждение");
                }

                ClearForm();
                leaseAgreement.LoadDraftLeases(cmbDraft);
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} получил ошибку при создании договора аренды: {ex.Message}");
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

       
    }
}