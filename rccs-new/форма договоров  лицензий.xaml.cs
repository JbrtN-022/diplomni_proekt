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

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для форма_договоров__лицензий.xaml
    /// </summary>
    public partial class форма_договоров__лицензий : Window
    {
        public форма_договоров__лицензий()
        {
            InitializeComponent();
            LoadDocumentTypes();
        }
        private void LoadDocumentTypes()
        {
           
            cmbDocumentType.SelectedIndex = 0; 
            cmbDocumentType_SelectionChanged(null, null);
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            форма_администратора adm = new форма_администратора();
            Application.Current.MainWindow = adm;
            adm.Show();

            this.Close();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDocumentType.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите тип документа!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (cmbDocumentList.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите документ!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            string documentType =
                (cmbDocumentType.SelectedItem as ComboBoxItem)
                .Content.ToString();

            int id =
                Convert.ToInt32(cmbDocumentList.SelectedValue);
            if (documentType == "Договор аренды")
            {
                Редактирование_договоров__аренды frm =
                    new Редактирование_договоров__аренды(id);

                frm.Owner = this;

                frm.WindowStartupLocation =
                    WindowStartupLocation.CenterOwner;

                frm.ShowDialog();
            }
            else if (documentType == "Лицензия на ПО")
            {
                Редактирование__лицензий_на_ПО frm =
                    new Редактирование__лицензий_на_ПО(id);

                frm.Owner = this;

                frm.WindowStartupLocation =
                    WindowStartupLocation.CenterOwner;

                frm.ShowDialog();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDocumentType.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите тип документа!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbDocumentType.Focus();
                return;
            }
            if (cmbDocumentList.SelectedItem == null)
            {
                MessageBox.Show(
                    "Выберите документ!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbDocumentList.Focus();
                return;
            }
            if (chkOnlyApproved.IsChecked == true)
            {
                MessageBoxResult approvedResult =
                    MessageBox.Show(
                        "Вы выбрали утверждённые документы.\n\n" +
                        "Удаление утверждённых документов может быть ограничено.\n\n" +
                        "Продолжить удаление?",
                        "Предупреждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                if (approvedResult != MessageBoxResult.Yes)
                    return;
            }
            int documentId = Convert.ToInt32(cmbDocumentList.SelectedValue);
            string documentType =
       (cmbDocumentType.SelectedItem as ComboBoxItem).Content.ToString();

            int id =
                Convert.ToInt32(cmbDocumentList.SelectedValue);
            if (!leaseAndLicenseAgreement.CanDeleteDocument(
                id,
                documentType))
            {
                return;
            }
            MessageBoxResult result =
                MessageBox.Show(
                    "Удалить документ?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            bool deleted =
                leaseAndLicenseAgreement.DeleteDocument(
                    id,
                    documentType);

            if (deleted)
            {
                MessageBox.Show(
                    "Документ удалён!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                cmbDocumentType_SelectionChanged(null, null);
            }
            else
            {
                MessageBox.Show(
                    "Ошибка удаления!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            
            if (cmbDocumentType.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип документа!", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbDocumentType.Focus();
                return;
            }

            if (cmbDocumentList.SelectedValue == null)
            {
                MessageBox.Show("Выберите документ для печати!", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbDocumentList.Focus();
                return;
            }

            string documentType = (cmbDocumentType.SelectedItem as ComboBoxItem)?.Content?.ToString();
            int documentId = Convert.ToInt32(cmbDocumentList.SelectedValue);

            try
            {
                
                Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Word Document (*.docx)|*.docx",
                    FileName = $"Документ_№{documentId}_{DateTime.Now:yyyyMMdd_HHmmss}",
                    InitialDirectory = @"E:\крошик",
                    Title = "Сохранить документ"
                };

                if (saveDialog.ShowDialog() != true) return;

                string filePath = saveDialog.FileName;
                bool success = false;

                
                if (documentType == "Договор аренды")
                {
                    DataRow data = leaseAgreement.GetLeaseDataForPrint(Convert.ToInt32(documentId));
                    if (data == null)
                    {
                        MessageBox.Show("Не удалось получить данные договора.", "Ошибка");
                        return;
                    }
                    success = document_lease_premises.CreateLeasePremises(
                     templatePath: @"E:\ДИПЛОМ\rccs\rccs-new\document\Договор_аренды.docx",
                     filePath: filePath,
                     printMode: 3,
                     numContract: documentId.ToString(),
                     counterparty: data["counterpartyFirm"]?.ToString() ?? "",   
                     counterpartName: data["cont_person_name"]?.ToString() ?? "",
                     dateFrom: data["rental_date_from"] != DBNull.Value
                         ? Convert.ToDateTime(data["rental_date_from"]).ToShortDateString() : "",
                     dateUntil: data["rental_date_until"] != DBNull.Value
                         ? Convert.ToDateTime(data["rental_date_until"]).ToShortDateString() : "",
                     dateConclusion: data["conclusion_date"] != DBNull.Value
                         ? Convert.ToDateTime(data["conclusion_date"]).ToShortDateString() : "",
                     companyName: data["company"]?.ToString() ?? "",
                     actualadress: data["actual_address"]?.ToString() ?? "",
                     cops: data["COPC"]?.ToString() ?? "",
                     workerName: data["worker"]?.ToString() ?? "",
                     floor: data["floor"]?.ToString() ?? "",
                     office: data["office"]?.ToString() ?? "",
                     square: data["square"] != DBNull.Value ? Convert.ToDecimal(data["square"]) : 0,
                     pricePerM2: data["price_per_m2"] != DBNull.Value ? Convert.ToDecimal(data["price_per_m2"]) : 0,
                     price: data["price"] != DBNull.Value ? Convert.ToDecimal(data["price"]) : 0,
                     totalPrice: data["total_price"] != DBNull.Value ? Convert.ToDecimal(data["total_price"]) : 0,
                     bic: data["BIC"]?.ToString() ?? "",
                     inn: data["INN"]?.ToString() ?? "",
                     correspondentAccount: data["Correspondent_account"]?.ToString() ?? "",
                     paymentAccount: data["Payment_account"]?.ToString() ?? ""
                 );
                }

                else if(documentType == "Лицензия на ПО")
                {
                    DataRow licenseRow = licenseAgreement.GetLicenseAgreementForPrint(documentId);
                    if (licenseRow == null)
                    {
                        MessageBox.Show("Не удалось получить данные лицензии.", "Ошибка");
                        return;
                    }
                   
                    string licenseFilePath = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(filePath),
                $"Лицензия_№{documentId}_{DateTime.Now:yyyyMMdd_HHmmss}.docx"
            );

                            success = document_license_agreement.CreateLicenseAgreement(
                        templatePath: @"E:\ДИПЛОМ\rccs\rccs-new\document\Шаблон_ТС_А0_2026_с_НДС.docx",
                        filePath: licenseFilePath,
                        numContract: documentId.ToString(),
                        Customers: licenseRow["counterparty_name"]?.ToString() ?? "",
                        CustomersName: licenseRow["cont_person_name"]?.ToString() ?? "",
                        program: licenseRow["program_name"]?.ToString() ?? "",
                        dateFrom: licenseRow["rental_date_from"] != DBNull.Value
                            ? Convert.ToDateTime(licenseRow["rental_date_from"]).ToShortDateString() : "",
                        dateUntil: licenseRow["rental_date_until"] != DBNull.Value
                            ? Convert.ToDateTime(licenseRow["rental_date_until"]).ToShortDateString() : "",
                        dateConclusion: licenseRow["conclusion_date"] != DBNull.Value
                            ? Convert.ToDateTime(licenseRow["conclusion_date"]).ToShortDateString() : "",
                        licenseId: documentId.ToString()
                    );
                }

                if (success)
                {
                    MessageBox.Show("Документ успешно сохранен!", "Успех",
                                   MessageBoxButton.OK, MessageBoxImage.Information);

                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
                    }
                    catch { }
                }
                else
                {
                    MessageBox.Show("Ошибка при создании документа.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при печати:\n" + ex.Message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void chkOnlyApproved_Unchecked(object sender, RoutedEventArgs e)
        {
            chkOnlyApproved_Checked(sender, e);
        }
        private void chkOnlyApproved_Checked(object sender, RoutedEventArgs e)
        {
            if (cmbDocumentType.SelectedItem is ComboBoxItem item)
            {
                string type = item.Content.ToString();
                LoadDocumentsByType(type, chkOnlyApproved.IsChecked == true);
            }
        }

        private void cmbDocumentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDocumentType.SelectedItem is ComboBoxItem item)
            {
                string type = item.Content.ToString();
                bool onlyApproved = chkOnlyApproved.IsChecked == true;

                LoadDocumentsByType(type, onlyApproved);
            }
        }
        private void LoadDocumentsByType(string documentType, bool onlyApproved)
        {
            cmbDocumentList.ItemsSource = null;

            string approvedFilter = onlyApproved ? "Утверждён" : null;

            if (documentType == "Договор аренды")
            {
                leaseAndLicenseAgreement.LoadLeaseAgreements(
                    cmbDocumentList as System.Windows.Controls.ComboBox,
                    approvedFilter);
            }
            else if (documentType == "Лицензия на ПО")
            {
                leaseAndLicenseAgreement.LoadLicenseAgreements(
                    cmbDocumentList as System.Windows.Controls.ComboBox,
                    approvedFilter);
            }
        }
        private void cmbDocumentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDocumentList.SelectedValue == null) return;

            int id = Convert.ToInt32(cmbDocumentList.SelectedValue);
            string type = (cmbDocumentType.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (type == "Договор аренды")
            {

                //leaseAndLicenseAgreement.LoadLeaseCards(itemsControlDocuments, id);
            }
            else if (type == "Лицензия на ПО")
            {

                //leaseAndLicenseAgreement.LoadLicenseCards(itemsControlDocuments, id);
            }
        }
    }
}
