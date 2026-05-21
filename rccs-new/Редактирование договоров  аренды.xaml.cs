using rccs.MyClass;
using rccs_new.MyClass;
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
    /// Логика взаимодействия для Редактирование_договоров__аренды.xaml
    /// </summary>
    public partial class Редактирование_договоров__аренды : Window
    {
        private int _idLeaseAgreement;

        public Редактирование_договоров__аренды(int idLeaseAgreement)
        {
            _idLeaseAgreement = idLeaseAgreement;

            InitializeComponent();
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.F1)
                {
                    ShowHelp();
                    e.Handled = true;
                }
            };
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл редактирование договора аренды ID {_idLeaseAgreement}");

            LoadClients();
            LoadFloor();

            LoadData();
        }
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА РЕДАКТИРОВАНИЯ ДОГОВОРА АРЕНДЫ

Назначение формы:
Редактирование существующего договора аренды помещения.

Что можно сделать на этой форме:
• Изменить контрагента (арендатора)
• Изменить этаж и помещение
• Изменить срок действия договора
• Утвердить или снять утверждение договора
• Сохранить изменения

Поля для заполнения:

1. НОМЕР ДОГОВОРА
   • Отображается автоматически
   • Не подлежит редактированию

2. КОНТРАГЕНТ (обязательное поле)
   • Выбор из списка существующих
   • Определяет арендатора помещения

3. ЭТАЖ (обязательное поле)
   • Выбор из списка этажей
   • Влияет на доступные помещения

4. ПОМЕЩЕНИЕ (обязательное поле)
   • Выбор из списка помещений
   • Зависит от выбранного этажа
   • Определяет арендуемое помещение

5. СРОК ДЕЙСТВИЯ (обязательное поле)
   • Дата начала аренды
   • Дата окончания аренды
   • Окончание не может быть раньше начала

6. СТАТУС ДОГОВОРА
   • Чекбокс ""Утверждён""
   • Отмечается при одобрении договора

Кнопки управления:

• СОХРАНИТЬ - сохраняет все изменения
• НАЗАД - закрывает форму без сохранения

Валидация:
• Проверяется заполнение всех обязательных полей
• Проверяется выбор помещения
• Проверяется корректность дат



Примечание:
При изменении этажа список помещений автоматически обновляется.
Помещение должно быть свободно на выбранный период.
После сохранения изменения вступают в силу немедленно.
Для создания нового договора используйте форму ""Оформление договоров"".",
                "Помощь - Редактирование договора аренды",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        private void LoadData()
        {
            DataRow row =
                leaseAgreement.LoadDraftById(
                    _idLeaseAgreement);

            if (row == null)
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} не смог загрузить договор аренды ID {_idLeaseAgreement}");

                MessageBox.Show(
                    "Договор не найден!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Close();
                return;
            }

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил данные договора аренды ID {_idLeaseAgreement}");

            txtLicenseNumber.Text =
                row["id_lease_agreement"].ToString();

            cmbClient.SelectedValue =
                row["id_counterparty"].ToString();

            cmbFloor.Text =
                row["floor_name"].ToString();

            cmbRoom.Text =
                row["office_name"].ToString();

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
                row["approved"].ToString() == "Утверждён";
        }

        private void LoadClients()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список контрагентов");

            guideBD.selectCounterpartyComboBox();

            cmbClient.ItemsSource = ConnectionBD.dtCounterpartyComboBox.DefaultView;
            cmbClient.DisplayMemberPath = "name";
            cmbClient.SelectedValuePath = "id_counterparty";
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClient.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите контрагента!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbClient.Focus();

                return;
            }

            if (cmbFloor.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите этаж!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbFloor.Focus();

                return;
            }

            if (cmbRoom.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите помещение!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbRoom.Focus();

                return;
            }

            if (dpStart.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите дату начала!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                dpStart.Focus();

                return;
            }

            if (dpEnd.SelectedDate == null)
            {
                MessageBox.Show(
                    "Выберите дату окончания!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                dpEnd.Focus();

                return;
            }

            string approved =
                chkApproved.IsChecked == true
                ? "Утверждён"
                : "Не утверждён";

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} пытается обновить договор аренды ID {_idLeaseAgreement}");

            bool result =
                leaseAgreement.UpdateLeaseAgreement(
                    _idLeaseAgreement,
                    Convert.ToInt32(cmbClient.SelectedValue),
                    Convert.ToInt32(cmbRoom.SelectedValue),
                    dpStart.SelectedDate.Value,
                    dpEnd.SelectedDate.Value,
                    approved);

            if (result)
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно обновил договор аренды ID {_idLeaseAgreement}");

                MessageBox.Show(
                    "Изменения сохранены!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Close();
            }
            else
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} не смог обновить договор аренды ID {_idLeaseAgreement}");

                MessageBox.Show(
                    "Ошибка сохранения!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} закрыл редактирование договора аренды ID {_idLeaseAgreement}");

            this.Close();
        }

        private void cmbFloor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFloor.SelectedItem is DataRowView row)
            {
                int floorNumber = Convert.ToInt32(row["floor"]);

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} выбрал этаж {floorNumber} при редактировании договора аренды");

                MessageBox.Show(floorNumber.ToString());

                LoadRoom(floorNumber);
            }
            else
            {
                LoadRoom(null);
            }
        }
    }
}