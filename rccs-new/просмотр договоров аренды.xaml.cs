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
    /// Логика взаимодействия для просмотр_договоров_аренды.xaml
    /// </summary>
    public partial class просмотр_договоров_аренды : Window
    {
        public просмотр_договоров_аренды()
        {
            InitializeComponent();
            leaseAgreement.leaseAgreementSelect(itemsControlLicenses);
            LoadWorkersCombo();
        }
        private void LoadWorkersCombo()
        {
            guideBD.selectWorkers();
            cmbWorker.ItemsSource = ConnectionBD.dtWorkersComboBox.DefaultView;
            cmbWorker.DisplayMemberPath = "name";
            cmbWorker.SelectedValuePath = "id_workers";
        }
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
        }
        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            cmbWorker.SelectedIndex = -1;
            dpDateFrom.SelectedDate = null;
            dpDateTo.SelectedDate = null;
            RefreshLicenses();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            форма_бухгалтера formBuch = new форма_бухгалтера();
            Application.Current.MainWindow = formBuch;
            formBuch.Show();

            this.Close();
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshLicenses();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
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
    }
}
