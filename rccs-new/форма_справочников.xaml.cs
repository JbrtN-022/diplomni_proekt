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
    /// Логика взаимодействия для форма_справочников.xaml
    /// </summary>
    public partial class форма_справочников : Window
    {
        public static int SelectedSpravochnikId { get; private set; } = 1;
        private int? selectedItemId = null;

        public форма_справочников()
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
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл окно управления справочниками");

            LoadSpravochniki();

            cmbSpravochnik.SelectedIndex = 0;
        }
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА УПРАВЛЕНИЯ СПРАВОЧНИКАМИ

Назначение формы:
Управление справочной информацией, используемой в системе.

Что можно сделать на этой форме:
• Просматривать содержимое справочников
• Добавлять новые записи в справочники
• Редактировать существующие записи
• Удалять записи (с проверкой использования)

Доступные справочники:

1. ВИД ЛИЦА
   • Типы контрагентов (Юридическое лицо, Физическое лицо)
   • Используется при оформлении контрагентов

2. ГОРОДА
   • Список городов
   • Используется для указания адресов контрагентов

3. ЭТАЖИ
   • Номера этажей в помещениях
   • Используется при описании арендуемых помещений

4. ПОМЕЩЕНИЯ
   • Список арендуемых помещений/офисов
   • Основной справочник для договоров аренды

5. РОЛИ НА РАБОТЕ
   • Должности и роли сотрудников
   • Определяет права доступа в системе

Функциональные возможности:

1. ДОБАВЛЕНИЕ
   • Выберите справочник
   • Введите название в поле ""Добавление""
   • Нажмите кнопку ""+""

2. РЕДАКТИРОВАНИЕ
   • Выберите запись в таблице
   • Измените название в поле ""Редактирование""
   • Нажмите кнопку ""✎""

3. УДАЛЕНИЕ
   • Выберите запись в таблице
   • Нажмите кнопку ""🗑""
   • Подтвердите удаление

Ограничения:
• Нельзя удалить запись, если она используется в других таблицах
• Запрещено создавать дубликаты записей
• При редактировании проверяется уникальность названия



Примечание:
Справочники являются основой для многих форм системы.
Изменение справочников влияет на все связанные документы.
Будьте внимательны при удалении записей - это может нарушить целостность данных.",
                "Помощь - Управление справочниками",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        private void LoadSpravochniki()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил список справочников");

            var spravochniki = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(1, "Вид лица"),
                new KeyValuePair<int, string>(2, "Города"),
                new KeyValuePair<int, string>(3, "Этажи"),
                new KeyValuePair<int, string>(4, "Помещения"),
                new KeyValuePair<int, string>(5, "Роли на работе")
            };

            cmbSpravochnik.ItemsSource = spravochniki;
            cmbSpravochnik.DisplayMemberPath = "Value";
            cmbSpravochnik.SelectedValuePath = "Key";

            cmbSpravochnik.SelectedIndex = 0;
        }

        private void cmbSpravochnik_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbSpravochnik.SelectedItem is KeyValuePair<int, string> selected)
            {
                SelectedSpravochnikId = selected.Key;
                selectedItemId = null;

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} выбрал справочник: {selected.Value} (ID: {selected.Key})");

                Add.Visibility = Visibility.Collapsed;
                Upp.Visibility = Visibility.Collapsed;

                dgSpravochnik.ItemsSource = null;
                dgSpravochnik.Columns.Clear();

                switch (SelectedSpravochnikId)
                {
                    case 1:
                        guideBD.selectVidLicaForTable();
                        dgSpravochnik.ItemsSource = ConnectionBD.dtVidLica.DefaultView;
                        break;

                    case 2:
                        guideBD.selectGorodaForTable();
                        dgSpravochnik.ItemsSource = ConnectionBD.dtGoroda.DefaultView;
                        break;

                    case 3:
                        guideBD.selectEtajForTable();
                        dgSpravochnik.ItemsSource = ConnectionBD.dtEtaj.DefaultView;
                        break;

                    case 4:
                        guideBD.selectOfficeForTable();
                        dgSpravochnik.ItemsSource = ConnectionBD.dtOffice.DefaultView;
                        break;

                    case 5:
                        guideBD.selectRollForTable();
                        dgSpravochnik.ItemsSource = ConnectionBD.dtRoll.DefaultView;
                        break;
                }

                dgSpravochnik.UpdateLayout();

                Add.Visibility = Visibility.Visible;

                txtAdd.Text = "";

                addtxt.Text = $"Добавление: {selected.Value}";
            }
        }

        private string GetNameFromRow(DataRowView row)
        {
            if (row == null) return "";

            switch (SelectedSpravochnikId)
            {
                case 1: return row["тип лица"]?.ToString() ?? "";
                case 2: return row["города"]?.ToString() ?? "";
                case 3: return row["этаж"]?.ToString() ?? "";
                case 4: return row["помещение"]?.ToString() ?? "";
                case 5: return row["роль работника"]?.ToString() ?? "";
                default: return row[0]?.ToString() ?? "";
            }
        }

        private int? GetIdFromDublicateQuery(string idColumnName)
        {
            try
            {
                object result = ConnectionBD.mycommand.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"Ошибка получения ID записи: {ex.Message}");
            }

            return null;
        }

        private void dgSpravochnik_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Upp.Visibility = Visibility.Collapsed;

            selectedItemId = null;

            if (dgSpravochnik.SelectedItem is DataRowView row)
            {
                string name = GetNameFromRow(row);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} выбрал запись '{name}'");

                    bool exists = false;
                    int? foundId = null;

                    switch (SelectedSpravochnikId)
                    {
                        case 1:
                            exists = guideBD.DublicateVidLica(name);
                            if (exists) foundId = GetIdFromDublicateQuery("id_type_of_face");
                            break;

                        case 2:
                            exists = guideBD.DublicateCity(name);
                            if (exists) foundId = GetIdFromDublicateQuery("id_city");
                            break;

                        case 3:
                            exists = guideBD.DublicateFloor(name);
                            if (exists) foundId = GetIdFromDublicateQuery("id_floor");
                            break;

                        case 4:
                            exists = guideBD.DublicateOffice(name);
                            if (exists) foundId = GetIdFromDublicateQuery("id_office");
                            break;

                        case 5:
                            exists = guideBD.DublicateRoll(name);
                            if (exists) foundId = GetIdFromDublicateQuery("id_roll");
                            break;
                    }

                    if (exists && foundId.HasValue)
                    {
                        selectedItemId = foundId;

                        txtUpp.Text = name;

                        upptxt.Text = $"Редактирование: {cmbSpravochnik.Text}";

                        Upp.Visibility = Visibility.Visible;

                        HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} выбрал запись ID {selectedItemId} для редактирования");
                    }
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} вернулся в панель администратора из окна справочников");

            форма_администратора formadm = new форма_администратора();

            Application.Current.MainWindow = formadm;

            formadm.Show();

            this.Close();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAdd.Text))
            {
                MessageBox.Show("Введите название для добавления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} попытался добавить пустую запись");

                return;
            }

            string name = txtAdd.Text.Trim();

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} пытается добавить запись '{name}'");

            bool exists = false;

            switch (SelectedSpravochnikId)
            {
                case 1: exists = guideBD.DublicateVidLica(name); break;
                case 2: exists = guideBD.DublicateCity(name); break;
                case 3: exists = guideBD.DublicateFloor(name); break;
                case 4: exists = guideBD.DublicateOffice(name); break;
                case 5: exists = guideBD.DublicateRoll(name); break;
            }

            if (exists)
            {
                MessageBox.Show($"Запись с названием «{name}» уже существует!",
                                "Дубликат",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} попытался добавить дубликат '{name}'");

                return;
            }

            try
            {
                switch (SelectedSpravochnikId)
                {
                    case 1: guideBD.AddVidLica(txtAdd.Text.Trim()); break;
                    case 2: guideBD.AddCity(txtAdd.Text.Trim()); break;
                    case 3: guideBD.AddFloor(txtAdd.Text.Trim()); break;
                    case 4: guideBD.AddOffice(txtAdd.Text.Trim()); break;
                    case 5: guideBD.AddRoll(txtAdd.Text.Trim()); break;
                }

                MessageBox.Show("Запись успешно добавлена!", "Успех");

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} успешно добавил запись '{name}'");

                txtAdd.Text = "";

                RefreshCurrentTable();
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"Ошибка добавления записи '{name}': {ex.Message}");

                MessageBox.Show($"Ошибка добавления:\n{ex.Message}",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private int GetCount(string query)
        {
            ConnectionBD.mycommand.CommandText = query;

            object result = ConnectionBD.mycommand.ExecuteScalar();

            return Convert.ToInt32(result ?? 0);
        }

        private void RefreshCurrentTable()
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} обновил таблицу справочника");

            dgSpravochnik.ItemsSource = null;

            switch (SelectedSpravochnikId)
            {
                case 1:
                    guideBD.selectVidLicaForTable();
                    dgSpravochnik.ItemsSource = ConnectionBD.dtVidLica.DefaultView;
                    break;

                case 2:
                    guideBD.selectGorodaForTable();
                    dgSpravochnik.ItemsSource = ConnectionBD.dtGoroda.DefaultView;
                    break;

                case 3:
                    guideBD.selectEtajForTable();
                    dgSpravochnik.ItemsSource = ConnectionBD.dtEtaj.DefaultView;
                    break;

                case 4:
                    guideBD.selectOfficeForTable();
                    dgSpravochnik.ItemsSource = ConnectionBD.dtOffice.DefaultView;
                    break;

                case 5:
                    guideBD.selectRollForTable();
                    dgSpravochnik.ItemsSource = ConnectionBD.dtRoll.DefaultView;
                    break;
            }
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItemId == null)
            {
                MessageBox.Show("Выберите запись для удаления!", "Предупреждение");

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} попытался удалить запись без выбора");

                return;
            }

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} пытается удалить запись ID {selectedItemId}");

            int count = 0;
            string tableName = "";

            switch (SelectedSpravochnikId)
            {
                case 1:
                    count = GetCount($"SELECT count(*) FROM rccs.counterparty where id_type_of_face= {selectedItemId};");
                    tableName = "вид лица";
                    break;

                case 2:
                    count = GetCount($"SELECT COUNT(*) FROM rccs.counterparty WHERE id_city = {selectedItemId}");
                    tableName = "города";
                    break;

                case 3:
                    count = GetCount($"SELECT COUNT(*) FROM rccs.room WHERE id_floor = {selectedItemId}");
                    tableName = "помещения";
                    break;

                case 4:
                    count = GetCount($"SELECT COUNT(*) FROM rccs.room WHERE id_office = {selectedItemId}");
                    tableName = "комнаты";
                    break;

                case 5:
                    count = GetCount($"SELECT COUNT(*) FROM rccs.users WHERE id_roll = {selectedItemId}");
                    tableName = "пользователи";
                    break;
            }

            if (count > 0)
            {
                MessageBox.Show($"Нельзя удалить эту запись!\n\n" +
                                $"Она используется в таблице «{tableName}» ({count} записей).",
                                "Запрет удаления",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                HistoryLogger.Log($"Удаление записи ID {selectedItemId} запрещено. Используется в таблице {tableName}");

                return;
            }

            var result = MessageBox.Show(
                $"Вы действительно хотите удалить эту запись?\n\n" +
                $"Название: {txtUpp.Text}\n",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} отменил удаление записи ID {selectedItemId}");

                return;
            }

            try
            {
                switch (SelectedSpravochnikId)
                {
                    case 1: guideBD.DelVidLica(selectedItemId.ToString()); break;
                    case 2: guideBD.DelCity(selectedItemId.ToString()); break;
                    case 3: guideBD.DelFloor(selectedItemId.ToString()); break;
                    case 4: guideBD.DelOffice(selectedItemId.ToString()); break;
                    case 5: guideBD.DelRoll(selectedItemId.ToString()); break;
                }

                MessageBox.Show("Запись успешно удалена!",
                                "Успех",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} удалил запись ID {selectedItemId}");

                RefreshCurrentTable();

                Upp.Visibility = Visibility.Collapsed;

                selectedItemId = null;
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"Ошибка удаления записи ID {selectedItemId}: {ex.Message}");

                MessageBox.Show($"Ошибка при удалении:\n{ex.Message}",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ButtonUpp_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItemId == null || string.IsNullOrWhiteSpace(txtUpp.Text))
            {
                MessageBox.Show("Выберите запись и введите новое название!", "Предупреждение");

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} попытался обновить запись без выбора");

                return;
            }

            int count = 0;
            string tableName = "";

            switch (SelectedSpravochnikId)
            {
                case 1:
                    count = GetCount($"SELECT count(*) FROM rccs.counterparty where id_type_of_face= {selectedItemId};");
                    tableName = "вид лица";
                    break;

                case 2:
                    count = GetCount($"SELECT COUNT(*) FROM rccs.counterparty WHERE id_city = {selectedItemId}");
                    tableName = "помещения";
                    break;

                case 3:
                    count = GetCount($"SELECT COUNT(*) FROM rccs.room WHERE id_floor = {selectedItemId}");
                    tableName = "помещения";
                    break;

                case 4:
                    count = GetCount($"SELECT COUNT(*) FROM rccs.room WHERE id_office = {selectedItemId}");
                    tableName = "комнаты";
                    break;

                case 5:
                    count = GetCount($"SELECT COUNT(*) FROM rccs.users WHERE id_roll = {selectedItemId}");
                    tableName = "пользователи";
                    break;
            }

            if (count > 0)
            {
                MessageBox.Show($"Нельзя удалить эту запись!\n\n" +
                                $"Она используется в таблице «{tableName}»",
                                "Запрет удаления",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                HistoryLogger.Log($"Редактирование записи ID {selectedItemId} запрещено. Используется в таблице {tableName}");

                return;
            }

            string name = txtUpp.Text.Trim();

            bool exists = false;

            switch (SelectedSpravochnikId)
            {
                case 1: exists = guideBD.DublicateVidLica(name); break;
                case 2: exists = guideBD.DublicateCity(name); break;
                case 3: exists = guideBD.DublicateFloor(name); break;
                case 4: exists = guideBD.DublicateOffice(name); break;
                case 5: exists = guideBD.DublicateRoll(name); break;
            }

            if (exists)
            {
                MessageBox.Show($"Запись с названием «{name}» уже существует!",
                                "Дубликат",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} попытался обновить запись на дубликат '{name}'");

                return;
            }

            try
            {
                switch (SelectedSpravochnikId)
                {
                    case 1: guideBD.UppVidLica(selectedItemId.ToString(), txtUpp.Text.Trim()); break;
                    case 2: guideBD.UppCity(selectedItemId.ToString(), txtUpp.Text.Trim()); break;
                    case 3: guideBD.UppFloor(selectedItemId.ToString(), txtUpp.Text.Trim()); break;
                    case 4: guideBD.UppOffice(selectedItemId.ToString(), txtUpp.Text.Trim()); break;
                    case 5: guideBD.UppRoll(selectedItemId.ToString(), txtUpp.Text.Trim()); break;
                }

                MessageBox.Show("Запись успешно обновлена!", "Успех");

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} обновил запись ID {selectedItemId} на '{name}'");

                RefreshCurrentTable();
            }
            catch (Exception ex)
            {
                HistoryLogger.Log($"Ошибка обновления записи ID {selectedItemId}: {ex.Message}");

                MessageBox.Show($"Ошибка обновления:\n{ex.Message}", "Ошибка");
            }
        }
    }
}