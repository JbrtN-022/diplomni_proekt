using rccs.MyClass;
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
    /// Логика взаимодействия для форма_справочников.xaml
    /// </summary>
    public partial class форма_справочников : Window
    {
        public static int SelectedSpravochnikId { get; private set; } = 1;
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
            if(cmbSpravochnik.SelectedItem is KeyValuePair<int, string> selected)
    {
                SelectedSpravochnikId = selected.Key;

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
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            форма_администратора formadm = new форма_администратора();
            Application.Current.MainWindow = formadm;
            formadm.Show();

            this.Close();
        }
    }
}
