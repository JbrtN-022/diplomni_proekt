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
    /// Логика взаимодействия для редактирование_пользователей.xaml
    /// </summary>
    public partial class редактирование_пользователей : Window
    {
        private int currentWorkerId;
        public редактирование_пользователей(int id_workers)
        {
            InitializeComponent();
            currentWorkerId = id_workers;
            LoadDataForEdit();
            LoadRoles();
        }
        private void LoadDataForEdit()
        {
            ConnectionBD.mycommand.CommandText = @"
            SELECT 
                workers.id_workers,
                workers.name,
                workers.work_device_data,
                workers.number,
                workers.id_company,
                users.login,
                users.password,
                users.id_roll
            FROM rccs.workers
            JOIN rccs.users ON users.id_workers = workers.id_workers
            WHERE workers.id_workers = @id_workers";

            ConnectionBD.mycommand.Parameters.Clear();
            ConnectionBD.mycommand.Parameters.AddWithValue("@id_workers", currentWorkerId);

            ConnectionBD.dtTemp.Clear();
            ConnectionBD.myDataAdapter.Fill(ConnectionBD.dtTemp);

            if (ConnectionBD.dtTemp.Rows.Count > 0)
            {
                DataRow row = ConnectionBD.dtTemp.Rows[0];

                txtFIO.Text = row["name"]?.ToString() ?? "";
                txtPhone.Text = row["number"]?.ToString() ?? "";
                txtLogin.Text = row["login"]?.ToString() ?? "";
                txtPassword.Text = row["password"]?.ToString() ?? "";

                if (row["work_device_data"] != DBNull.Value && row["work_device_data"] != null)
                {
                    dpDate.SelectedDate = Convert.ToDateTime(row["work_device_data"]);
                }

                if (row["id_roll"] != DBNull.Value && row["id_roll"] != null)
                {
                    int roleId = Convert.ToInt32(row["id_roll"]);

                  
                    cmbRole.SelectedValue = roleId;

                   
                    if (cmbRole.SelectedValue == null)
                    {
                        
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            cmbRole.SelectedValue = roleId;
                        }), System.Windows.Threading.DispatcherPriority.Loaded);
                    }
                }
                if (row["id_company"] != DBNull.Value && row["id_company"] != null)
                {
                    int companyId = Convert.ToInt32(row["id_company"]);


                    cmbComp.SelectedValue = companyId;


                    if (cmbComp.SelectedValue == null)
                    {

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            cmbComp.SelectedValue = companyId;
                        }), System.Windows.Threading.DispatcherPriority.Loaded);
                    }
                }


            }
            else
            {
                MessageBox.Show("Не удалось загрузить данные сотрудника.", "Ошибка");
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFIO.Text))
            {
                MessageBox.Show("Введите ФИО сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Введите логин сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Введите пароль сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Введите телефон сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                
                ConnectionBD.mycommand.CommandText = $@"
                    UPDATE rccs.workers 
                    SET name = '{txtFIO.Text.Replace("'", "''")}',
                        work_device_data = '{dpDate.SelectedDate?.ToString("yyyy-MM-dd")}',
                        number = '{txtPhone.Text.Replace("'", "''")}',
                        id_company = '{cmbComp.SelectedValue}'
                    WHERE id_workers = {currentWorkerId}";

                ConnectionBD.mycommand.ExecuteNonQuery();

                
                ConnectionBD.mycommand.CommandText = $@"
                    UPDATE rccs.users 
                    SET login = '{txtLogin.Text.Replace("'", "''")}',
                        password = '{txtPassword.Text.Replace("'", "''")}',
                        id_roll = {cmbRole.SelectedValue}
                    WHERE id_workers = {currentWorkerId}";

                ConnectionBD.mycommand.ExecuteNonQuery();

                MessageBox.Show("Данные сотрудника успешно обновлены!", "Успех",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true; 
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении:\n{ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
