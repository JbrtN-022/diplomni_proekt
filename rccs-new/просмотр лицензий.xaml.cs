using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для просмотр_лицензий.xaml
    /// </summary>
    public partial class просмотр_лицензий : Window
    {
        public просмотр_лицензий()
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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл просмотр лицензий");

            LoadWorkersCombo();
            LoadProgramsCombo();

            licenseAgreement.licenseAgreementSelect(itemsControlLicenses);

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список лицензий");
        }
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА ПРОСМОТРА ЛИЦЕНЗИЙ

Назначение формы:
Просмотр и фильтрация оформленных лицензий на программное обеспечение.

Что можно сделать на этой форме:
• Просматривать список всех лицензий
• Искать лицензии по контрагенту
• Фильтровать по ответственному сотруднику
• Фильтровать по программе лицензирования
• Фильтровать по датам
• Очищать все фильтры

Функциональные возможности:

1. ПОИСК ПО КОНТРАГЕНТУ
   • Поле ""Поиск""
   • Введите название контрагента
   • Поиск происходит автоматически при вводе
   • Регистронезависимый поиск

2. ФИЛЬТР ПО СОТРУДНИКУ
   • Выпадающий список ""Ответственный сотрудник""
   • Показывает лицензии, оформленные конкретным сотрудником
   • Можно выбрать ""Пусто"" для отключения фильтра

3. ФИЛЬТР ПО ПРОГРАММЕ
   • Выпадающий список ""Программа""
   • Показывает лицензии на конкретное ПО
   • Можно выбрать ""Пусто"" для отключения фильтра

4. ФИЛЬТР ПО ДАТАМ
   • Поле ""Дата от"" - начало периода
   • Поле ""Дата до"" - конец периода
   • Показывает лицензии в указанном диапазоне дат
   • Можно выбрать даты из календаря или ввести вручную

5. КНОПКА ""ОЧИСТИТЬ ФИЛЬТРЫ""
   • Сбрасывает все фильтры
   • Возвращает полный список лицензий

6. КАРТОЧКИ ЛИЦЕНЗИЙ
   • Отображают основную информацию
   • Номер лицензии
   • Контрагент
   • Программа
   • Ответственный сотрудник
   • Срок действия
   • Дополнительные услуги

Фильтрация:
• Все фильтры работают одновременно
• Поиск происходит автоматически при изменении любого параметра
• Даты можно вводить в формате ДД.ММ.ГГГГ



Примечание:
Для создания новой лицензии используйте форму ""Оформление лицензии"".
Лицензию можно распечатать из карточки лицензии.
При наведении на карточку отображается дополнительная информация.
Фильтры применяются автоматически для удобства работы.",
                "Помощь - Просмотр лицензий",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        private void LoadWorkersCombo()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список работников");

            guideBD.selectWorkers();

            cmbWorker.ItemsSource = ConnectionBD.dtWorkersComboBox.DefaultView;
            cmbWorker.DisplayMemberPath = "name";
            cmbWorker.SelectedValuePath = "id_workers";
        }

        private void LoadProgramsCombo()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список программ");

            guideBD.selectPrograms();

            cmbProgram.ItemsSource = ConnectionBD.dtProgramsComboBox.DefaultView;
            cmbProgram.DisplayMemberPath = "name";
            cmbProgram.SelectedValuePath = "id_program";
        }

        private void RefreshLicenses()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} обновил фильтрацию лицензий");

            itemsControlLicenses.Items.Clear();

            string searchText = txtSearch.Text.Trim();

            DateTime? dateFrom = dpDateFrom.SelectedDate;
            DateTime? dateTo = dpDateTo.SelectedDate;

            int? workerId = cmbWorker.SelectedValue as int?;
            int? programId = cmbProgram.SelectedValue as int?;

            licenseAgreement.licenseAgreementSelect(
                itemsControlLicenses,
                searchText,
                workerId,
                programId,
                dateFrom,
                dateTo);
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} очистил фильтры лицензий");

            txtSearch.Clear();
            cmbWorker.SelectedIndex = -1;
            cmbProgram.SelectedIndex = -1;
            dpDateFrom.SelectedDate = null;
            dpDateTo.SelectedDate = null;

            RefreshLicenses();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshLicenses();
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshLicenses();
        }

        private void dpDateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshLicenses();
        }

        private void dpDateTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshLicenses();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} вернулся в форму менеджера из просмотра лицензий");

            форма_менеджера formMEN = new форма_менеджера();

            Application.Current.MainWindow = formMEN;

            formMEN.Show();

            this.Close();
        }
    }
}