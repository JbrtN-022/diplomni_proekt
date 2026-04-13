using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Логика взаимодействия для форма_прайса.xaml
    /// </summary>
    public partial class форма_прайса : Window
    {
        private PriceHistory.PriceType currentType;
        public форма_прайса()
        {
            InitializeComponent();


            cmbPrice.Items.Add("Прайс за м²");
            cmbPrice.Items.Add("Прайс программ");
            cmbPrice.Items.Add("Прайс услуг");

            cmbPrice.SelectedIndex = 0;
        }
        private void LoadRoomsForLink()
        {
            guideBD.selectOffice(); 

            cmbLink.ItemsSource = ConnectionBD.dtOfficeComboBox.DefaultView;
            cmbLink.DisplayMemberPath = "office";      
            cmbLink.SelectedValuePath = "id_room";    
            cmbLink.SelectedIndex = 0;
        }

        private void LoadProgramsForLink()
        {
            
            guideBD.selectPrograms();

            cmbLink.ItemsSource = ConnectionBD.dtProgramsComboBox.DefaultView; 
            cmbLink.DisplayMemberPath = "name";
            cmbLink.SelectedValuePath = "id_program";
            cmbLink.SelectedIndex = 0;
        }

        private void LoadServicesForLink()
        {
            guideBD.selectServices();

            cmbLink.ItemsSource = ConnectionBD.dtServicesComboBox.DefaultView;
            cmbLink.DisplayMemberPath = "name";
            cmbLink.SelectedValuePath = "id_services";
            cmbLink.SelectedIndex = 0;
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            форма_администратора formadm = new форма_администратора();
            Application.Current.MainWindow = formadm;
            formadm.Show();
            this.Close();
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dgPriceTable.SelectedItem == null)
            {
                MessageBox.Show("Выбери запись!");
                return;
            }

            DataRowView row = dgPriceTable.SelectedItem as DataRowView;
            int id = Convert.ToInt32(row[0]);

            if (PriceHistory.DeletePrice(currentType, id))
            {
                PriceHistory.LoadPriceData(dgPriceTable, currentType);
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Введите цену!");
                return;
            }

            if (cmbLink.SelectedValue == null)
            {
                MessageBox.Show("Выберите объект!");
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Ошибка цены!");
                return;
            }

            int id = Convert.ToInt32(cmbLink.SelectedValue);

            if (PriceHistory.AddPrice(currentType, price, id))
            {
                MessageBox.Show("Добавлено!");
                txtPrice.Clear();

                PriceHistory.LoadPriceData(dgPriceTable, currentType);
            }
        }

        private void cmbPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentType = (PriceHistory.PriceType)(cmbPrice.SelectedIndex + 1);

            switch (currentType)
            {
                case PriceHistory.PriceType.Meter:
                    guideBD.selectOffice();
                    cmbLink.ItemsSource = ConnectionBD.dtOffice.DefaultView;
                    cmbLink.DisplayMemberPath = "office";
                    cmbLink.SelectedValuePath = "id_office";
                    break;

                case PriceHistory.PriceType.Program:
                    guideBD.selectPrograms();
                    cmbLink.ItemsSource = ConnectionBD.dtProgramsComboBox.DefaultView;
                    cmbLink.DisplayMemberPath = "name";
                    cmbLink.SelectedValuePath = "id_program";                    
                    break;

                case PriceHistory.PriceType.Service:
                    guideBD.selectServices();
                    cmbLink.ItemsSource = ConnectionBD.dtServicesComboBox.DefaultView;
                    cmbLink.DisplayMemberPath = "name";
                    cmbLink.SelectedValuePath = "id_services";                
                    break;
            }

            cmbLink.SelectedIndex = 0;

            PriceHistory.LoadPriceData(dgPriceTable, currentType);
        }
        private void dgSpravochnik_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
