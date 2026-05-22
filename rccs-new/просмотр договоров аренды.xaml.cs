using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для просмотр_договоров_аренды.xaml
    /// </summary>
    public partial class просмотр_договоров_аренды : Window
    {
        public просмотр_договоров_аренды()
        {
            InitializeComponent();

            // Подключение обработчика клавиши F1 для вызова справки
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.F1)
                {
                    ShowHelp();
                    e.Handled = true;
                }
            };

            // Первоначальная загрузка списка договоров аренды
            leaseAgreement.leaseAgreementSelect(itemsControlLicenses);
            LoadWorkersCombo();

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл просмотр договоров аренды");
        }

        // Показывает справочное сообщение о форме просмотра договоров аренды
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА ПРОСМОТРА ДОГОВОРОВ АРЕНДЫ
Назначение формы:
Просмотр и фильтрация заключенных договоров аренды помещений.
Что можно сделать на этой форме:
• Просматривать список всех договоров аренды
• Искать договоры по контрагенту
• Фильтровать по ответственному сотруднику
• Фильтровать по датам
• Очищать все фильтры
Функциональные возможности:
1. ПОИСК ПО КОНТРАГЕНТУ
   • Поле ""Поиск""
   • Введите название контрагента
   • Поиск происходит автоматически при вводе
2. ФИЛЬТР ПО СОТРУДНИКУ
   • Выпадающий список ""Ответственный сотрудник""
   • Показывает договоры, оформленные конкретным сотрудником
   • Можно выбрать ""Пусто"" для отключения фильтра
3. ФИЛЬТР ПО ДАТАМ
   • Поле ""Дата от"" - начало периода
   • Поле ""Дата до"" - конец периода
   • Показывает договоры в указанном диапазоне дат
4. КНОПКА ""ОЧИСТИТЬ ФИЛЬТРЫ""
   • Сбрасывает все фильтры
   • Возвращает полный список договоров
5. КАРТОЧКИ ДОГОВОРОВ
   • Отображают основную информацию
   • Номер договора
   • Контрагент
   • Ответственный сотрудник
   • Срок действия
   • Сумма договора
Фильтрация:
• Все фильтры работают одновременно
• Поиск регистронезависимый
• Даты можно вводить вручную или выбирать из календаря
Примечание:
Для создания нового договора аренды используйте форму ""Оформление договоров"".
Договор аренды можно распечатать из карточки договора.
Фильтры применяются автоматически при изменении параметров.",
                "Помощь - Просмотр договоров аренды",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // Загрузка списка сотрудников в комбобокс фильтра
        private void LoadWorkersCombo()
        {
            guideBD.selectWorkers();
            cmbWorker.ItemsSource = ConnectionBD.dtWorkersComboBox.DefaultView;
            cmbWorker.DisplayMemberPath = "name";
            cmbWorker.SelectedValuePath = "id_workers";
        }

        // Обновление списка договоров с учётом всех фильтров
        private void RefreshLicenses()
        {
            itemsControlLicenses.Items.Clear();

            string searchText = txtSearch.Text.Trim();
            DateTime? dateFrom = dpDateFrom.SelectedDate;
            DateTime? dateTo = dpDateTo.SelectedDate;
            int? workerId = cmbWorker.SelectedValue as int?;

            leaseAgreement.leaseAgreementSelect(
                itemsControlLicenses,
                searchText,
                workerId,
                dateFrom,
                dateTo);

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} применил фильтр договоров аренды");
        }

        // Очистка всех фильтров и возврат полного списка
        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            cmbWorker.SelectedIndex = -1;
            dpDateFrom.SelectedDate = null;
            dpDateTo.SelectedDate = null;

            RefreshLicenses();

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} очистил фильтры договоров аренды");
        }

        // Возврат в главное окно бухгалтера
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            форма_бухгалтера formBuch = new форма_бухгалтера();
            Application.Current.MainWindow = formBuch;
            formBuch.Show();

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} закрыл просмотр договоров аренды");
            this.Close();
        }

        // Автообновление списка при изменении фильтра сотрудника
        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshLicenses();
        }

        // Автообновление списка при изменении текста поиска
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshLicenses();
        }

        // Автообновление списка при выборе даты "от"
        private void dpDateFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshLicenses();
        }

        // Автообновление списка при выборе даты "до"
        private void dpDateTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshLicenses();
        }
    }
}