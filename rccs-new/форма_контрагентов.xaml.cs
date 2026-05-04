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
    /// Логика взаимодействия для форма_контрагентов.xaml
    /// </summary>
    public partial class форма_контрагентов : Window
    {
        
        private int? selectedCounterpartyId = null;
        public форма_контрагентов()
        {
            InitializeComponent();
            LoadAllCounterparties();
           
            LoadFilters();
            if (ConnectionBD.roll == "1")
            {
                DelBtn.Visibility = Visibility.Collapsed;

            }
        }
        private void LoadFilters()
        {
            // Города
            guideBD.selectGoroda();
            var cities = ConnectionBD.dtGoroda.AsEnumerable()
                .Select(row => new
                {
                    city = row.Field<string>("city"),
                    id_city = row.Field<int>("id_city")
                })
                .ToList();

            cities.Insert(0, new { city = "Все города", id_city = 0 });
            cmbCity.ItemsSource = cities;
            cmbCity.DisplayMemberPath = "city";
            cmbCity.SelectedValuePath = "id_city";
            cmbCity.SelectedIndex = 0;

            // Типы лица
            guideBD.selectVidLica();
            var types = ConnectionBD.dtVidLica.AsEnumerable()
                .Select(row => new
                {
                    type_of_face = row.Field<string>("type_of_face"),
                    id_type_of_face = row.Field<int>("id_type_of_face")
                })
                .ToList();

            types.Insert(0, new { type_of_face = "Все типы", id_type_of_face = 0 });
            cmbTypeFace.ItemsSource = types;
            cmbTypeFace.DisplayMemberPath = "type_of_face";
            cmbTypeFace.SelectedValuePath = "id_type_of_face";
            cmbTypeFace.SelectedIndex = 0;
        }
        private void LoadAllCounterparties(string search = "", int cityId = 0, int typeId = 0)
        {
            selectedCounterpartyId = null;
            UppBtn.IsEnabled = false;
            DelBtn.IsEnabled = false;

            counterparty.SelectCounterparty(itemsControlCounterparty, search, cityId, typeId);

           
            foreach (UserControlCounterparty card in itemsControlCounterparty.Items)
            {
                card.CardClicked -= Card_CardClicked;   
                card.CardClicked += Card_CardClicked;   
            }
        }
        private void Card_CardClicked(object sender, int id)
        {
            selectedCounterpartyId = id;


            UppBtn.IsEnabled = true;
            DelBtn.IsEnabled = true;

          
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            switch (ConnectionBD.roll)
            {
                case null:
                    MessageBox.Show("Неверные данные!");
                    break;
                case "1":
                    форма_менеджера formMEN = new форма_менеджера();
                    Application.Current.MainWindow = formMEN;
                    formMEN.Show();
                    this.Close();
                    break;
                case "2":
                    форма_бухгалтера formBuch = new форма_бухгалтера();
                    Application.Current.MainWindow = formBuch;
                    formBuch.Show();
                    this.Close();
                    break;
                case "3":
                    форма_администратора formadm = new форма_администратора();
                    Application.Current.MainWindow = formadm;
                    formadm.Show();
                    this.Close();
                    break;
                default:
                    MessageBox.Show("Неверные данные!");
                    break;

            }
           
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            var addForm = new добавление_контрагента();

           
            if (addForm.ShowDialog() == true)
            {
                
                LoadAllCounterparties(
                    search: txtSearch.Text.Trim(),
                    cityId: cmbCity.SelectedValue != null ? Convert.ToInt32(cmbCity.SelectedValue) : 0,
                    typeId: cmbTypeFace.SelectedValue != null ? Convert.ToInt32(cmbTypeFace.SelectedValue) : 0
                );
            }
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCounterpartyId == null) return;
            MessageBox.Show(selectedCounterpartyId.Value.ToString());

            var result = MessageBox.Show("Удалить этого контрагента?", "Подтверждение",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                counterparty.DeleteCounterparty(selectedCounterpartyId.Value);
                LoadAllCounterparties();   
            }
        }

        private void UppBtn_Click(object sender, RoutedEventArgs e)
        {
            var addForm = new редактирование_контрагентов(selectedCounterpartyId.Value);


            if (addForm.ShowDialog() == true)
            {

                LoadAllCounterparties(
                    search: txtSearch.Text.Trim(),
                    cityId: cmbCity.SelectedValue != null ? Convert.ToInt32(cmbCity.SelectedValue) : 0,
                    typeId: cmbTypeFace.SelectedValue != null ? Convert.ToInt32(cmbTypeFace.SelectedValue) : 0
                );
            }
        }
        
        private void dgSpravochnik_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            int cityId = cmbCity.SelectedValue is int c && c > 0 ? c : 0;
            int typeId = cmbTypeFace.SelectedValue is int t && t > 0 ? t : 0;

            LoadAllCounterparties(searchText, cityId, typeId);
        }
      
        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCity == null || cmbTypeFace == null) return;

            string searchText = txtSearch.Text.Trim();
            int cityId = cmbCity.SelectedValue is int c && c > 0 ? c : 0;
            int typeId = cmbTypeFace.SelectedValue is int t && t > 0 ? t : 0;

            LoadAllCounterparties(searchText, cityId, typeId);
        }
    }
}
