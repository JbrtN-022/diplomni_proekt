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
    /// Логика взаимодействия для форма_пользователей.xaml
    /// </summary>
    public partial class форма_пользователей : Window
    {
        public форма_пользователей()
        {
            InitializeComponent();
            LoadWorkers();
        }

        private void LoadWorkers()
        {
           ClassWorkers.SelectWorkers();                   
            dgSpravochnik.ItemsSource = ConnectionBD.dtWorkers.DefaultView;   
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

        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dgSpravochnik.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления!", "Предупреждение",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            
            DataRowView row = dgSpravochnik.SelectedItem as DataRowView;
            if (row == null) return;

            int id_workers = Convert.ToInt32(row["id_workers"]);

           
            var result = MessageBox.Show(
                $@"Вы действительно хотите удалить сотрудника? ФИО: {row["name"]}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    ClassWorkers.DeleteWorker(id_workers);
                    MessageBox.Show("Сотрудник успешно удалён!", "Успешно",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadWorkers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении:\n{ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UppBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dgSpravochnik.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования!", "Предупреждение",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = dgSpravochnik.SelectedItem as DataRowView;
            if (row == null) return;

            int id_workers = Convert.ToInt32(row["id_workers"]);

          
            редактирование_пользователей editForm = new редактирование_пользователей(id_workers);
            editForm.Owner = this;

            if (editForm.ShowDialog() == true)
            {
                LoadWorkers();        
            }
        }
        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            добавление_пользователей addform = new добавление_пользователей();
            addform.Owner = this;

            if (addform.ShowDialog() == true)
            {
                LoadWorkers();
            }


        }
    }
}
