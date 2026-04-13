using rccs.MyClass;
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
            LoadSpravochniki();
            cmbSpravochnik.SelectedIndex = 0;
        }
        private void LoadSpravochniki()
        {

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
                Add.Visibility = Visibility.Collapsed;
                Upp.Visibility = Visibility.Collapsed;
                dgSpravochnik.ItemsSource = null;
                dgSpravochnik.Columns.Clear();

                switch (SelectedSpravochnikId)
                {
                    case 1:
                        guideBD.selectVidLica();
                        dgSpravochnik.ItemsSource = ConnectionBD.dtVidLica.DefaultView;
                        break;
                    case 2:
                        guideBD.selectGoroda();
                        dgSpravochnik.ItemsSource = ConnectionBD.dtGoroda.DefaultView;
                        break;
                    case 3:
                        guideBD.selectEtaj();
                        dgSpravochnik.ItemsSource = ConnectionBD.dtEtaj.DefaultView;
                        break;
                    case 4:
                        guideBD.selectOffice();
                        dgSpravochnik.ItemsSource = ConnectionBD.dtOffice.DefaultView;
                        break;
                    case 5:
                        guideBD.selectRoll();
                        dgSpravochnik.ItemsSource = ConnectionBD.dtRoll.DefaultView;
                        break;
                }

                dgSpravochnik.UpdateLayout();
                Add.Visibility = Visibility.Visible;
                txtAdd.Text = "";
                addtxt.Text = $"Добавление: {selected.Value}";
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            форма_администратора formadm = new форма_администратора();
            Application.Current.MainWindow = formadm;
            formadm.Show();

            this.Close();
        }

        private void dgSpravochnik_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Upp.Visibility = Visibility.Collapsed;
            selectedItemId = null;

            if (dgSpravochnik.SelectedItem is DataRowView row)
            {
                string idColumnName = GetIdColumnName();

                if (row.Row.Table.Columns.Contains(idColumnName))
                {
                    selectedItemId = Convert.ToInt32(row[idColumnName]);

                  
                    string nameColumn = GetNameColumnName();

                    if (row.Row.Table.Columns.Contains(nameColumn))
                    {
                        txtUpp.Text = row[nameColumn]?.ToString() ?? "";
                    }
                    else
                    {
                        txtUpp.Text = row[1]?.ToString() ?? ""; 
                    }

                    upptxt.Text = $"Редактирование: {cmbSpravochnik.Text}";   
                    Upp.Visibility = Visibility.Visible;
                }
            }
        }
        private string GetIdColumnName()
        {
            switch (SelectedSpravochnikId)
            {
                case 2: return "id_city";      
                case 3: return "id_floor";   
                case 4: return "id_office";    
                case 5: return "id_roll";     
                case 1: return "id_type_of_face";      
                default: return "id";
            }
        }

       
        private string GetNameColumnName()
        {
            switch (SelectedSpravochnikId)
            {
                
                case 2: return "city";
                case 3: return "floor";
                case 4: return "office";
                case 5: return "roll";
                case 1: return "type_of_face";    
                default: return "name";
            }
        }
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAdd.Text))
            {
                MessageBox.Show("Введите название для добавления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string name = txtAdd.Text.Trim(); 
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
                                "Дубликат", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                txtAdd.Text = "";
                RefreshCurrentTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            dgSpravochnik.ItemsSource = null;

            switch (SelectedSpravochnikId)
            {
                case 1: guideBD.selectVidLica(); dgSpravochnik.ItemsSource = ConnectionBD.dtVidLica.DefaultView; break;
                case 2: guideBD.selectGoroda(); dgSpravochnik.ItemsSource = ConnectionBD.dtGoroda.DefaultView; break;
                case 3: guideBD.selectEtaj(); dgSpravochnik.ItemsSource = ConnectionBD.dtEtaj.DefaultView; break;
                case 4: guideBD.selectOffice(); dgSpravochnik.ItemsSource = ConnectionBD.dtOffice.DefaultView; break;
                case 5: guideBD.selectRoll(); dgSpravochnik.ItemsSource = ConnectionBD.dtRoll.DefaultView; break;
            }
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItemId == null)
            {
                MessageBox.Show("Выберите запись для удаления!", "Предупреждение");
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
                    count = GetCount($"SELECT COUNT(*) FROM rccs.room WHERE id_city = {selectedItemId}");
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
                                $"Она используется в таблице «{tableName}» ({count} записей).",
                                "Запрет удаления",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            var result = MessageBox.Show(
                $"Вы действительно хотите удалить эту запись?\n\n" +
                $"Название: {txtUpp.Text}\n",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            
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

                MessageBox.Show("Запись успешно удалена!", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                RefreshCurrentTable();         
                Upp.Visibility = Visibility.Collapsed;  
                selectedItemId = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonUpp_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItemId == null || string.IsNullOrWhiteSpace(txtUpp.Text))
            {
                MessageBox.Show("Выберите запись и введите новое название!", "Предупреждение");
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
                    count = GetCount($"SELECT COUNT(*) FROM rccs.room WHERE id_city = {selectedItemId}");
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
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string name = txtAdd.Text.Trim();
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
                                "Дубликат", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                MessageBox.Show(selectedItemId.ToString(), txtUpp.Text.Trim());
                switch (SelectedSpravochnikId)
                {
                    case 1: guideBD.UppVidLica(selectedItemId.ToString(), txtUpp.Text.Trim()); break;
                    case 2: guideBD.UppCity(selectedItemId.ToString(), txtUpp.Text.Trim()); break;
                    case 3: guideBD.UppFloor(selectedItemId.ToString(), txtUpp.Text.Trim()); break;
                    case 4: guideBD.UppOffice(selectedItemId.ToString(), txtUpp.Text.Trim()); break;
                    case 5: guideBD.UppRoll(selectedItemId.ToString(), txtUpp.Text.Trim()); break;
                }

                MessageBox.Show("Запись успешно обновлена!", "Успех");
                RefreshCurrentTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления:\n{ex.Message}", "Ошибка");
            }
        }
    }
}
