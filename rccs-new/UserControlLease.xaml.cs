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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для UserControlLease.xaml
    /// </summary>
    public partial class UserControlLease : UserControl
    {
        public string LeaseID, Counterparty, CounterpartyName, ContPersonName;
        public string CompanyName, WorkerName, adress, COPC;
        public string OfficeName, FloorName;
        public string Approved;

        public DateTime RentalFrom, RentalUntil, ConclusionDate;

        public decimal Square, PricePerM2, Price, TotalPrice;

        public string BIC, INN, CorrespondentAccount, PaymentAccount;
        public UserControlLease(DataRow row) { 
            InitializeComponent();
            LeaseID = row["id_lease_agreement"]?.ToString();
            Counterparty = row["counterpartyFirm"]?.ToString();
            CounterpartyName = row["cont_person_name"]?.ToString();
           

            CompanyName = row["company"]?.ToString();
            adress = row["actual_address"]?.ToString();
            COPC = row["COPC"]?.ToString();
            WorkerName = row["worker"]?.ToString();

            OfficeName = row["office"]?.ToString();
            FloorName = row["floor"]?.ToString();
            Approved = row["approved"]?.ToString();

            RentalFrom = row.Field<DateTime>("rental_date_from");
            RentalUntil = row.Field<DateTime>("rental_date_until");
            ConclusionDate = row.Field<DateTime>("conclusion_date");

            Square = row["square"] == DBNull.Value ? 0 : Convert.ToDecimal(row["square"]);
            PricePerM2 = row["price_per_m2"] == DBNull.Value ? 0 : Convert.ToDecimal(row["price_per_m2"]);
            Price = row["price"] == DBNull.Value ? 0 : Convert.ToDecimal(row["price"]);
            TotalPrice = row["total_price"] == DBNull.Value ? 0 : Convert.ToDecimal(row["total_price"]);

            BIC = row["BIC"]?.ToString();
            INN = row["INN"]?.ToString();
            CorrespondentAccount = row["Correspondent_account"]?.ToString();
            PaymentAccount = row["Payment_account"]?.ToString();

           
            txtLeaseId.Text = LeaseID;
            txtCounterparty.Text = CounterpartyName;
            txtOffice.Text = OfficeName;
            txtDateFrom.Text = RentalFrom.ToShortDateString();
            txtDateUntil.Text = RentalUntil.ToShortDateString();
            txtWorker.Text = WorkerName;

            txtApproved.Text = Approved ?? "-";

            if (Approved == "Не утверждён")
                txtApproved.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCE4436"));

            if (Approved == "Утверждён")
                txtApproved.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)   
            {
                var result = MessageBox.Show(
                    "Напечатать договор аренды?",
                    "Печать договора",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {

                    string numContract = txtLeaseId.Text.Trim();

                    if (string.IsNullOrWhiteSpace(numContract) || numContract == "-")
                    {
                        MessageBox.Show("Не удалось определить номер договора!",
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    try
                    {
                        var saveDialog = new Microsoft.Win32.SaveFileDialog
                        {
                            Title = "Сохранить договор аренды",
                            Filter = "Word Document (*.docx)|*.docx|Word 97-2003 (*.doc)|*.doc",
                            DefaultExt = ".docx",
                            FileName = $"Договор_аренды_№{numContract}_{DateTime.Now:yyyyMMdd_HHmmss}"
                        };

                        if (saveDialog.ShowDialog() == true)
                        {
                            string filePath = saveDialog.FileName;

                            bool success = document_lease_premises.CreateLeasePremises(
                                templatePath: @"E:\ДИПЛОМ\rccs\rccs-new\document\Договор_аренды.docx",
                                filePath: filePath,
                                printMode: 1, 
                                numContract: LeaseID,
                                counterparty:Counterparty,
                                counterpartName: CounterpartyName,
                                dateFrom: RentalFrom.ToShortDateString(),
                                dateUntil: RentalUntil.ToShortDateString(),
                                dateConclusion: ConclusionDate.ToShortDateString(),
                                companyName: CompanyName,
                                actualadress:adress,
                                cops: COPC,
                                workerName: WorkerName,
                                floor: FloorName,
                                office: OfficeName,

                                square: Square,
                                pricePerM2: PricePerM2,
                                price: Price,
                                totalPrice: TotalPrice,

                                bic: BIC,
                                inn: INN,
                                correspondentAccount: CorrespondentAccount,
                                paymentAccount: PaymentAccount
                            );

                            if (success)
                            {
                                MessageBox.Show(
                                    $"Договор успешно сохранён!\nПуть: {filePath}",
                                    "Успех",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                                System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + filePath + "\"");
                            }
                            else
                            {
                                MessageBox.Show("Не удалось создать документ.",
                                    "Ошибка",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Ошибка при сохранении файла:\n{ex.Message}",
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }
    }
    
}
