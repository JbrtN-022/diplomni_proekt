using rccs.MyClass;
using rccs_new.MyClass;
using rccs_new.MyClass.document;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace rccs_new
{
    public partial class форма_договоров__лицензий : Window
    {
        private DataTable currentLeaseData;
        private DataTable currentLicenseData;

        public форма_договоров__лицензий()
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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму договоров и лицензий");
            LoadDocumentTypes();
        }
        // Показывает справочное сообщение о форме
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА ДОГОВОРОВ И ЛИЦЕНЗИЙ

Назначение формы:
Централизованное управление всеми договорами и лицензиями в системе.

Что можно сделать на этой форме:
• Просматривать список договоров аренды
• Просматривать список лицензий на ПО
• Фильтровать документы по статусу (утверждённые/все)
• Редактировать существующие документы
• Удалять документы (с проверкой)
• Распечатывать документы в формате Word

Доступные типы документов:

1. ДОГОВОР АРЕНДЫ
   • Договоры на аренду помещений
   • Информация: контрагент, помещение, сроки, стоимость
   • Возможность редактирования, удаления, печати

2. ЛИЦЕНЗИЯ НА ПО
   • Лицензионные договоры на ПО
   • Информация: контрагент, программа, услуги, сроки
   • Возможность редактирования, удаления, печати

Функциональные возможности:

1. ФИЛЬТРАЦИЯ
   • Выбор типа документа (договор/лицензия)
   • Чекбокс ""Только утверждённые""
   • Автоматическое обновление списка

2. РЕДАКТИРОВАНИЕ
   • Выберите документ из списка
   • Нажмите кнопку ""✎""
   • Измените необходимые данные

3. УДАЛЕНИЕ
   • Выберите документ из списка
   • Нажмите кнопку ""🗑""
   • Подтвердите удаление
   • Утверждённые документы могут иметь ограничения

4. ПЕЧАТЬ
   • Выберите документ из списка
   • Нажмите кнопку ""Печать""
   • Выберите место сохранения Word-файла
   • Документ создаётся по шаблону

Примечание:
Перед удалением документа убедитесь, что это не нарушит целостность данных.
Утверждённые документы могут быть защищены от удаления.
При печати документа используется стандартный шаблон организации.
Созданный Word-файл можно сохранить в любую папку на компьютере.",
                "Помощь - Договоры и лицензии",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        // Инициализация типов документов в комбобоксе
        private void LoadDocumentTypes()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил типы документов");
            cmbDocumentType.SelectedIndex = 0;
        }
        // Возврат в главное окно администратора
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} вернулся в панель администратора");

            форма_администратора adm = new форма_администратора();
            Application.Current.MainWindow = adm;
            adm.Show();

            this.Close();
        }
        // Редактирование выбранного документа
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDocumentType.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип документа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbDocumentList.SelectedItem == null)
            {
                MessageBox.Show("Выберите документ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string documentType = (cmbDocumentType.SelectedItem as ComboBoxItem).Content.ToString();
            int id = Convert.ToInt32(cmbDocumentList.SelectedValue);

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл редактирование документа ID {id}, тип: {documentType}");

            if (documentType == "Договор аренды")
            {
                Редактирование_договоров__аренды frm = new Редактирование_договоров__аренды(id);
                frm.Owner = this;
                frm.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                if (frm.ShowDialog() == true)
                {
                    LoadDocumentsByType(documentType, chkOnlyApproved.IsChecked == true);
                }
            }
            else if (documentType == "Лицензия на ПО")
            {
                Редактирование__лицензий_на_ПО frm = new Редактирование__лицензий_на_ПО(id);
                frm.Owner = this;
                frm.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                if (frm.ShowDialog() == true)
                {
                    LoadDocumentsByType(documentType, chkOnlyApproved.IsChecked == true);
                }
            }
        }
        // Удаление выбранного документа
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDocumentType.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип документа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbDocumentType.Focus();
                return;
            }

            if (cmbDocumentList.SelectedItem == null)
            {
                MessageBox.Show("Выберите документ!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbDocumentList.Focus();
                return;
            }

            if (chkOnlyApproved.IsChecked == true)
            {
                MessageBoxResult approvedResult = MessageBox.Show(
                    "Вы выбрали утверждённые документы.\n\nУдаление утверждённых документов может быть ограничено.\n\nПродолжить удаление?",
                    "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (approvedResult != MessageBoxResult.Yes)
                    return;
            }

            int documentId = Convert.ToInt32(cmbDocumentList.SelectedValue);
            string documentType = (cmbDocumentType.SelectedItem as ComboBoxItem).Content.ToString();

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} пытается удалить документ ID {documentId}, тип: {documentType}");

            if (!leaseAndLicenseAgreement.CanDeleteDocument(documentId, documentType))
            {
                HistoryLogger.Log($"Пользователю {ConnectionBD.resFio} запрещено удаление документа ID {documentId}");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Удалить документ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            bool deleted = leaseAndLicenseAgreement.DeleteDocument(documentId, documentType);

            if (deleted)
            {
                MessageBox.Show("Документ удалён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} удалил документ ID {documentId}, тип: {documentType}");
                LoadDocumentsByType(documentType, chkOnlyApproved.IsChecked == true);
            }
            else
            {
                HistoryLogger.Log($"Ошибка удаления документа ID {documentId} пользователем {ConnectionBD.resFio}");
                MessageBox.Show("Ошибка удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Печать выбранного документа в Word
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDocumentType.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип документа!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbDocumentType.Focus();
                return;
            }

            if (cmbDocumentList.SelectedValue == null)
            {
                MessageBox.Show("Выберите документ для печати!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbDocumentList.Focus();
                return;
            }

            string documentType = (cmbDocumentType.SelectedItem as ComboBoxItem)?.Content?.ToString();
            int documentId = Convert.ToInt32(cmbDocumentList.SelectedValue);

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} начал печать документа ID {documentId}, тип: {documentType}");

            try
            {
                Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Word Document (*.docx)|*.docx",
                    FileName = $"Документ_№{documentId}_{DateTime.Now:yyyyMMdd_HHmmss}",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Title = "Сохранить документ"
                };

                if (saveDialog.ShowDialog() != true) return;

                string filePath = saveDialog.FileName;
                bool success = false;

                // Получаем путь к папке document в корне проекта
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDirectory, @"..\..\"));
                string documentPath = System.IO.Path.Combine(projectDirectory, "document");

                if (documentType == "Договор аренды")
                {
                    DataRow data = leaseAgreement.GetLeaseDataForPrint(documentId);

                    if (data == null)
                    {
                        MessageBox.Show("Не удалось получить данные договора.", "Ошибка");
                        return;
                    }

                    string templatePath = System.IO.Path.Combine(documentPath, "Договор_аренды.docx");

                    if (!System.IO.File.Exists(templatePath))
                    {
                        MessageBox.Show($"Файл шаблона не найден:\n{templatePath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    success = document_lease_premises.CreateLeasePremises(
                        templatePath: templatePath,
                        filePath: filePath,
                        printMode: 3,
                        numContract: documentId.ToString(),
                        counterparty: data["counterpartyFirm"]?.ToString() ?? "",
                        counterpartName: data["cont_person_name"]?.ToString() ?? "",
                        dateFrom: data["rental_date_from"] != DBNull.Value ? Convert.ToDateTime(data["rental_date_from"]).ToShortDateString() : "",
                        dateUntil: data["rental_date_until"] != DBNull.Value ? Convert.ToDateTime(data["rental_date_until"]).ToShortDateString() : "",
                        dateConclusion: data["conclusion_date"] != DBNull.Value ? Convert.ToDateTime(data["conclusion_date"]).ToShortDateString() : "",
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
                        paymentAccount: data["Payment_account"]?.ToString() ?? "");
                }
                else if (documentType == "Лицензия на ПО")
                {
                    DataRow licenseRow = licenseAgreement.GetLicenseAgreementForPrint(documentId);

                    if (licenseRow == null)
                    {
                        MessageBox.Show("Не удалось получить данные лицензии.", "Ошибка");
                        return;
                    }

                    string templatePath = System.IO.Path.Combine(documentPath, "Шаблон_ТС_А0_2026_с_НДС.docx");

                    if (!System.IO.File.Exists(templatePath))
                    {
                        MessageBox.Show($"Файл шаблона не найден:\n{templatePath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    success = document_license_agreement.CreateLicenseAgreement(
                        templatePath: templatePath,
                        filePath: filePath,
                        numContract: documentId.ToString(),
                        Customers: licenseRow["counterparty_name"]?.ToString() ?? "",
                        CustomersName: licenseRow["cont_person_name"]?.ToString() ?? "",
                        program: licenseRow["program_name"]?.ToString() ?? "",
                        dateFrom: licenseRow["rental_date_from"] != DBNull.Value ? Convert.ToDateTime(licenseRow["rental_date_from"]).ToShortDateString() : "",
                        dateUntil: licenseRow["rental_date_until"] != DBNull.Value ? Convert.ToDateTime(licenseRow["rental_date_until"]).ToShortDateString() : "",
                        dateConclusion: licenseRow["conclusion_date"] != DBNull.Value ? Convert.ToDateTime(licenseRow["conclusion_date"]).ToShortDateString() : "",
                        licenseId: documentId.ToString());
                }

                if (success)
                {
                    MessageBox.Show("Документ успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно сохранил документ ID {documentId}, тип: {documentType}");

                    try
                    {
                        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");
                    }
                    catch { }
                }
                else
                {
                    HistoryLogger.Log($"Ошибка создания документа ID {documentId} пользователем {ConnectionBD.resFio}");
                    MessageBox.Show("Ошибка при создании документа.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"ОШИБКА! Пользователь {ConnectionBD.resFio} не смог распечатать документ ID {documentId}. Ошибка: {ex.Message}");
                MessageBox.Show("Ошибка при печати:\n" + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Обработчики изменения фильтров
        private void chkOnlyApproved_Unchecked(object sender, RoutedEventArgs e)
        {
            if (cmbDocumentType.SelectedItem is ComboBoxItem item)
            {
                string type = item.Content.ToString();
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} изменил фильтр документов. Тип: {type}, только утверждённые: {chkOnlyApproved.IsChecked}");
                LoadDocumentsByType(type, false);
            }
        }

        private void chkOnlyApproved_Checked(object sender, RoutedEventArgs e)
        {
            if (cmbDocumentType.SelectedItem is ComboBoxItem item)
            {
                string type = item.Content.ToString();
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} изменил фильтр документов. Тип: {type}, только утверждённые: {chkOnlyApproved.IsChecked}");
                LoadDocumentsByType(type, true);
            }
        }

        private void cmbDocumentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDocumentType.SelectedItem is ComboBoxItem item)
            {
                string type = item.Content.ToString();
                bool onlyApproved = chkOnlyApproved.IsChecked == true;

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} выбрал тип документа: {type}");

                cmbDocumentList.ItemsSource = null;
                cmbDocumentList.SelectedIndex = -1;

                LoadDocumentsByType(type, onlyApproved);
            }
        }
        // Основной метод загрузки документов выбранного типа
        private void LoadDocumentsByType(string documentType, bool onlyApproved)
        {
            itemsControlDocuments.ItemsSource = null;

            if (documentType == "Договор аренды")
            {
                currentLeaseData = leaseAndLicenseAgreement.LoadLeaseAgreementsToList(onlyApproved);
                cmbDocumentList.ItemsSource = null;
                cmbDocumentList.DisplayMemberPath = "DisplayName";
                cmbDocumentList.SelectedValuePath = "id_lease_agreement";
                cmbDocumentList.ItemsSource = currentLeaseData?.DefaultView;

                if (currentLeaseData != null && currentLeaseData.Rows.Count > 0)
                {
                    List<UserControlLease> leaseCards = new List<UserControlLease>();

                    foreach (DataRow row in currentLeaseData.Rows)
                    {
                        UserControlLease leaseCard = new UserControlLease(row);
                        leaseCards.Add(leaseCard);
                    }

                    itemsControlDocuments.ItemsSource = leaseCards;
                    cmbDocumentList.SelectedIndex = -1;
                }
            }
            else if (documentType == "Лицензия на ПО")
            {
                currentLicenseData = leaseAndLicenseAgreement.LoadLicenseAgreementsToList(onlyApproved);
                cmbDocumentList.ItemsSource = null;
                cmbDocumentList.DisplayMemberPath = "DisplayName";
                cmbDocumentList.SelectedValuePath = "id_license_agreement";
                cmbDocumentList.ItemsSource = currentLicenseData?.DefaultView;

                if (currentLicenseData != null && currentLicenseData.Rows.Count > 0)
                {
                    List<UserControlLicenseAgreement> licenseCards = new List<UserControlLicenseAgreement>();

                    foreach (DataRow row in currentLicenseData.Rows)
                    {
                        UserControlLicenseAgreement licenseCard =
     new UserControlLicenseAgreement(row);
                        licenseCards.Add(licenseCard);
                    }

                    itemsControlDocuments.ItemsSource = licenseCards;
                    cmbDocumentList.SelectedIndex = -1;
                }
            }
        }
        // Обработка выбора документа в комбобоксе
        private void cmbDocumentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDocumentList.SelectedValue != null && cmbDocumentList.SelectedIndex != -1)
            {
                int id = Convert.ToInt32(cmbDocumentList.SelectedValue);
                string type = (cmbDocumentType.SelectedItem as ComboBoxItem)?.Content.ToString();
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} выбрал документ ID {id}, тип: {type}");

                if (type == "Договор аренды")
                {
                    DataRow selectedRow = currentLeaseData?.Select($"id_lease_agreement = {id}").FirstOrDefault();
                    if (selectedRow != null)
                    {
                        UserControlLease leaseCard = new UserControlLease(selectedRow);
                        itemsControlDocuments.ItemsSource = new List<UserControlLease> { leaseCard };
                    }
                }
                else if (type == "Лицензия на ПО")
                {
                    DataRow selectedRow = currentLicenseData?.Select($"id_license_agreement = {id}").FirstOrDefault();
                    if (selectedRow != null)
                    {
                        UserControlLicenseAgreement licenseCard =
     new UserControlLicenseAgreement(selectedRow);
                        itemsControlDocuments.ItemsSource = new List<UserControlLicenseAgreement> { licenseCard };
                    }
                }
            }
        }
    }
}