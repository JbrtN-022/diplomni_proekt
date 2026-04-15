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
            LoadWorkersCombo();
            LoadProgramsCombo();
            licenseAgreement.licenseAgreementSelect(itemsControlLicenses);
        }
        private void LoadWorkersCombo()
        {
            guideBD.selectWorkers();
            cmbWorker.ItemsSource = ConnectionBD.dtWorkersComboBox.DefaultView;
            cmbWorker.DisplayMemberPath = "name";
            cmbWorker.SelectedValuePath = "id_workers";
        }

        private void LoadProgramsCombo()
        {
            guideBD.selectPrograms();
            cmbProgram.ItemsSource = ConnectionBD.dtProgramsComboBox.DefaultView;
            cmbProgram.DisplayMemberPath = "name";
            cmbProgram.SelectedValuePath = "id_program";
        }
        private void RefreshLicenses()
        {
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
            форма_менеджера formMEN = new форма_менеджера();
            Application.Current.MainWindow = formMEN;
            formMEN.Show();
            this.Close();
        }
    }
}
