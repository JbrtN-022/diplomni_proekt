using rccs_new.MyClass.document;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace rccs_new
{
    public partial class UserControlLicenseAgreement : UserControl
    {
        public string LicenseID;
        public string Counterparty;
        public string CounterpartyName;

        public string ProgramName;

        public string WorkerName;

        public DateTime DateFrom;
        public DateTime DateUntil;
        public DateTime ConclusionDate;

        public string Approved;

        public UserControlLicenseAgreement(DataRow row)
        {
            InitializeComponent();

            LicenseID = row["id_license_agreement"]?.ToString();

            Counterparty = row["counterparty_name"]?.ToString();

            CounterpartyName = row["counterparty_name"]?.ToString();

            ProgramName = row["program_name"]?.ToString();

            WorkerName = row["worker_name"]?.ToString();

            DateFrom = Convert.ToDateTime(row["rental_date_from"]);

            DateUntil = Convert.ToDateTime(row["rental_date_until"]);

            ConclusionDate = row["conclusion_date"] != DBNull.Value
       ? Convert.ToDateTime(row["conclusion_date"])
       : DateTime.MinValue;


            Approved = row["approved"]?.ToString();

            txtLicenseId.Text = LicenseID ?? "-";

            txtCounterparty.Text = Counterparty ?? "-";

            txtProgram.Text = ProgramName ?? "-";

            txtDateFrom.Text = DateFrom.ToShortDateString();

            txtDateUntil.Text = DateUntil.ToShortDateString();

            txtConclusionDate.Text =
    ConclusionDate != DateTime.MinValue
    ? ConclusionDate.ToShortDateString()
    : "-";

            txtWorker.Text = "Ответственный: " + (WorkerName ?? "-");

            txtApproved.Text = Approved ?? "-";

            LoadServices(row);
          
            if (Approved == "Не утверждён")
            {
                txtApproved.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#FFCE4436"));
            }

            if (Approved == "Утверждён")
            {
                txtApproved.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#4CAF50"));
            }
        }

        private void LoadServices(DataRow row)
        {
            try
            {
                if (row.Table.Columns.Contains("services_list"))
                {
                    txtServicesList.Text =
                        row["services_list"]?.ToString()
                        ?? "Услуги отсутствуют";
                }
                else
                {
                    txtServicesList.Text =
                        "Список услуг недоступен";
                }
            }
            catch (Exception ex)
            {
                txtServicesList.Text =
                    "Ошибка загрузки услуг";

                MessageBox.Show(ex.Message);
            }
        }

        private void StackPanel_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var result = MessageBox.Show(
                    "Напечатать лицензионный договор?",
                    "Печать",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var saveDialog =
                            new Microsoft.Win32.SaveFileDialog
                            {
                                Filter = "Word (*.docx)|*.docx",
                                FileName =
                                    $"Договор_об_оформлении_лицензии_№{LicenseID}_{DateTime.Now:yyyyMMdd_HHmmss}"
                            };
                        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        string projectDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDirectory, @"..\..\"));
                        string documentPath = System.IO.Path.Combine(projectDirectory, "document");
                        string templatePath = System.IO.Path.Combine(documentPath, "Шаблон_ТС_А0_2026_с_НДС.docx");

                        if (!System.IO.File.Exists(templatePath))
                        {
                            MessageBox.Show($"Файл шаблона не найден:\n{templatePath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (saveDialog.ShowDialog() == true)
                        {
                            bool success =
                                document_license_agreement
                                .CreateLicenseAgreement(

                                    templatePath:
                                   templatePath,

                                    filePath:
                                    saveDialog.FileName,

                                    numContract:
                                    LicenseID,

                                    Customers:
                                    Counterparty,

                                    CustomersName:
                                    CounterpartyName,

                                    program:
                                    ProgramName,

                                    dateFrom:
                                    DateFrom.ToShortDateString(),

                                    dateUntil:
                                    DateUntil.ToShortDateString(),

                                    dateConclusion:
                                    ConclusionDate.ToShortDateString(),

                                    licenseId:
                                    LicenseID
                                );

                            if (success)
                            {
                                MessageBox.Show(
                                    "Договор успешно сохранён!",
                                    "Успех",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                                try
                                {
                                    System.Diagnostics.Process.Start(
                                        "explorer.exe",
                                        "/select,\"" + saveDialog.FileName + "\"");
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            ex.Message,
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}