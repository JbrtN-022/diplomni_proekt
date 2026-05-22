using rccs_new.MyClass;
using rccs_new.MyClass.document;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для UserControlLease.xaml
    /// Пользовательский контрол для отображения карточки договора аренды
    /// </summary>
    public partial class UserControlLease : UserControl
    {
        // Основные данные договора
        public string LeaseID, Counterparty, CounterpartyName, ContPersonName;
        public string CompanyName, WorkerName, adress, COPC;
        public string OfficeName, FloorName;
        public string Approved;

        // Даты
        public DateTime RentalFrom, RentalUntil, ConclusionDate;

        // Финансовые показатели
        public decimal Square, PricePerM2, Price, TotalPrice;

        // Банковские реквизиты
        public string BIC, INN, CorrespondentAccount, PaymentAccount;

        public UserControlLease(DataRow row)
        {
            InitializeComponent();

            // Заполнение данных из строки DataRow
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

            // Заполнение визуальных элементов
            txtLeaseId.Text = LeaseID;
            txtCounterparty.Text = CounterpartyName;
            txtOffice.Text = OfficeName;
            txtDateFrom.Text = RentalFrom.ToShortDateString();
            txtDateUntil.Text = RentalUntil.ToShortDateString();
            txtWorker.Text = WorkerName;
            txtApproved.Text = Approved ?? "-";

            // Цветовая индикация статуса утверждения
            if (Approved == "Не утверждён")
                txtApproved.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCE4436"));

            if (Approved == "Утверждён")
                txtApproved.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
        }

        /// <summary>
        /// Обработка двойного клика по карточке договора — печать документа
        /// </summary>
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
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        string projectDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDirectory, @"..\..\"));
                        string documentPath = System.IO.Path.Combine(projectDirectory, "document");
                        string templatePath = System.IO.Path.Combine(documentPath, "Договор_аренды.docx");
                        if (saveDialog.ShowDialog() == true)
                        {
                            string filePath = saveDialog.FileName;

                            bool success = document_lease_premises.CreateLeasePremises(
                                templatePath: templatePath,
                                filePath: filePath,
                                printMode: 1,
                                numContract: LeaseID,
                                counterparty: Counterparty,
                                counterpartName: CounterpartyName,
                                dateFrom: RentalFrom.ToShortDateString(),
                                dateUntil: RentalUntil.ToShortDateString(),
                                dateConclusion: ConclusionDate.ToShortDateString(),
                                companyName: CompanyName,
                                actualadress: adress,
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
                                MessageBox.Show($"Договор успешно сохранён!\nПуть: {filePath}",
                                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                                System.Diagnostics.Process.Start("explorer.exe", "/select,\"" + filePath + "\"");
                            }
                            else
                            {
                                MessageBox.Show("Не удалось создать документ.",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении файла:\n{ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}