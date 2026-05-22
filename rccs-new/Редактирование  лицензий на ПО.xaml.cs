using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.F1)
                {
                    ShowHelp();
                    e.Handled = true;
                }
            };
            _idLicenseAgreement = idLicenseAgreement;

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл редактирование лицензии ID {_idLicenseAgreement}");

            LoadClients();
            LoadProgram();
            LoadServices();

            LoadLicenseData();
        }
        // Показывает справочное сообщение о форме
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА РЕДАКТИРОВАНИЯ ЛИЦЕНЗИИ ПО

Назначение формы:
Редактирование существующей лицензии на программное обеспечение.

Что можно сделать на этой форме:
• Изменить контрагента (лицензиата)
• Изменить программу лицензирования
• Изменить срок действия лицензии
• Изменить состав услуг
• Утвердить или снять утверждение лицензии
• Сохранить изменения

Поля для заполнения:

1. НОМЕР ЛИЦЕНЗИИ
   • Отображается автоматически
   • Не подлежит редактированию

2. КОНТРАГЕНТ (обязательное поле)
   • Выбор из списка существующих
   • Определяет владельца лицензии

3. ПРОГРАММА (обязательное поле)
   • Выбор из списка ПО
   • Определяет объект лицензирования

4. СРОК ДЕЙСТВИЯ (обязательное поле)
   • Дата начала лицензии
   • Дата окончания лицензии
   • Окончание не может быть раньше начала

5. УСЛУГИ (обязательное поле)
   • Таблица доступных услуг
   • Выберите услуги (галочка)
   • Укажите количество для каждой услуги
   • Минимум одна услуга должна быть выбрана

6. СТАТУС ЛИЦЕНЗИИ
   • Чекбокс ""Утверждена""
   • Отмечается при одобрении лицензии

Кнопки управления:

• СОХРАНИТЬ - сохраняет все изменения
• НАЗАД - закрывает форму без сохранения

Валидация:
• Проверяется заполнение всех обязательных полей
• Проверяется наличие выбранных услуг
• Проверяется корректность количества услуг
• Дата окончания не может быть раньше даты начала



Примечание:
При редактировании утвержденной лицензии будьте внимательны.
Изменение услуг полностью заменяет предыдущий список.
После сохранения изменения вступают в силу немедленно.",
                "Помощь - Редактирование лицензии",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        // Загрузка всех доступных услуг
        private void LoadServices()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список услуг");

            licenseAgreement.LoadAllServices(
                dgAllServices,
                dgAllServices);
        }
        // Загрузка списка контрагентов
        private void LoadClients()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список контрагентов");

            guideBD.selectCounterpartyComboBox();

            cmbClient.ItemsSource =
                ConnectionBD.dtCounterpartyComboBox.DefaultView;

            cmbClient.DisplayMemberPath = "name";

            cmbClient.SelectedValuePath =
                "id_counterparty";
        }
        // Загрузка списка программ
        private void LoadProgram()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список программ");

            guideBD.selectPrograms();

            cmbProgram.ItemsSource =
                ConnectionBD.dtProgramsComboBox.DefaultView;

            cmbProgram.DisplayMemberPath = "name";

            cmbProgram.SelectedValuePath =
                "id_program";
        }
        // Загрузка данных редактируемой лицензии
        private void LoadLicenseData()
        {
            DataRow row =
                licenseAgreement.LoadDraftById(
                    _idLicenseAgreement);

            if (row == null)
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} не смог загрузить лицензию ID {_idLicenseAgreement}");

                MessageBox.Show(
                    "Лицензия не найдена!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Close();

                return;
            }

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил данные лицензии ID {_idLicenseAgreement}");

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
        // Загрузка ранее выбранных услуг из лицензии
        private void LoadSelectedServicesForLicense(
            int licenseId)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил услуги для лицензии ID {licenseId}");

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
        // Получение списка выбранных услуг
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
        // Сохранение изменений лицензии
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

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} пытается обновить лицензию ID {_idLicenseAgreement}");

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

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно обновил лицензию ID {_idLicenseAgreement}");

                MessageBox.Show(
                    "Изменения сохранены!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Close();
            }
            else
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} не смог обновить лицензию ID {_idLicenseAgreement}");

                MessageBox.Show(
                    "Ошибка сохранения!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        // Закрытие формы без сохранения
        private void Back_Click(
            object sender,
            RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} закрыл окно редактирования лицензии ID {_idLicenseAgreement}");

            Close();
        }
    }
}