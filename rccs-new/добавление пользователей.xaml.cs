using rccs.MyClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static rccs.MyClass.ClassWorkers;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для добавление_пользователей.xaml
    /// </summary>
    public partial class добавление_пользователей : Window
    {
        public добавление_пользователей()
        {
            InitializeComponent();
            LoadRoles();
            dpDate.SelectedDateFormat = DatePickerFormat.Short;
        }
        private void txtFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            

            // только буквы + пробел
            if (!Regex.IsMatch(txtFIO.Text, @"^[а-яА-ЯёЁa-zA-Z\s]*$"))
            {
                txtFIO.BorderBrush = Brushes.Red;
            }
            else
            {
                txtFIO.BorderBrush = Brushes.Transparent;
            }
        }

        private void txtPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
           

            // +7 и максимум 12 символов
            if (!Regex.IsMatch(txtPhone.Text, @"^\+7\d{0,10}$"))
            {
                txtPhone.BorderBrush = Brushes.Red;
            }
            else
            {
                txtPhone.BorderBrush = Brushes.Transparent;
            }
        }

       

        private void dpDate_Changed(object sender, SelectionChangedEventArgs e)
        {
          

            if (dpDate.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Дата не может быть больше сегодняшней");
                dpDate.SelectedDate = null;
            }
        }
        private void LoadRoles()
        {
            guideBD.selectRoll();
            cmbRole.ItemsSource = ConnectionBD.dtRoll.DefaultView;
            cmbRole.DisplayMemberPath = "roll";
            cmbRole.SelectedValuePath = "id_roll";
            guideBD.selectCompany();
            cmbComp.ItemsSource = ConnectionBD.dtCompanyCombobox.DefaultView;
            cmbComp.DisplayMemberPath = "company";
            cmbComp.SelectedValuePath = "id_company";
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFIO.Text) ||
        string.IsNullOrEmpty(txtPhone.Text) ||
        dpDate.SelectedDate == null ||
        cmbRole.SelectedValue == null ||
        cmbComp.SelectedValue == null ||
        string.IsNullOrEmpty(txtLogin.Text) ||
        string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Заполни все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int workerId = ClassWorkers.AddWorker(
                    txtFIO.Text.Trim(),
                    dpDate.SelectedDate.Value,
                    txtPhone.Text.Trim(),
                    Convert.ToInt32(cmbComp.SelectedValue)
                );

                ClassWorkers.AddUser(
                    txtLogin.Text.Trim(),
                    txtPassword.Text.Trim(),
                    workerId,
                    Convert.ToInt32(cmbRole.SelectedValue)
                );

                MessageBox.Show("Работник успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
