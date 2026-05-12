using MySql.Data.MySqlClient;
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
    /// Логика взаимодействия для форма__насторойки_подключения_бд.xaml
    /// </summary>
    public partial class форма__насторойки_подключения_бд : Window
    {
        private string staryHost;
        private string staryUser;
        private string staryPassword;
        private string staryBD;

        public string NovyyHost {  get; private set; }
        public string NovyyUser { get; private set; }
        public string NovyyPassword { get; private set; }
        public string NovayaBD { get; private set; }
        public bool Podkluchenie {  get; private set; }

        private string vremennayaStroka;
        public форма__насторойки_подключения_бд(string teckHost,
            string teckUser,
            string teckPassword,
            string teckBD)
        {
            InitializeComponent();
            staryHost = teckHost;
            staryUser = teckUser;
            staryPassword = teckPassword;   
            staryBD = teckBD;


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NovyyHost =staryHost;
            NovyyUser = staryUser;
            NovyyPassword = staryPassword;
            NovayaBD = staryBD;
            Podkluchenie = false;

          
            this.Close();
        }

        private void podkluchKBD_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDatabase.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать базу данных!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string vybranayaBD = cmbDatabase.SelectedItem.ToString();

            try
            {
                string polnayaStrika = $"{vremennayaStroka}database={vybranayaBD}";

                using (MySqlConnection soed = new MySqlConnection(polnayaStrika))
                {
                    soed.Open();
                    soed.Close();
                }

                NovyyHost = txtHost.Text.Trim();
                NovyyUser= txtUser.Text.Trim();
                NovyyPassword= txtPassword.Text;
                NovayaBD = vybranayaBD;
                Podkluchenie = true;

                MessageBox.Show($"Подключение к базе данных '{vybranayaBD}' прошло успешно!", "Успешное выполнение", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();

            }
            catch
            {

            }
        }

        private void podkluchKServeru_Click(object sender, RoutedEventArgs e)
        {
            string host =txtHost.Text.Trim();
            string user =txtUser.Text.Trim();
            string pass = txtPassword.Text;
            
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user))
            {
                MessageBox.Show("Перед подключением к серверу, необходимо ввести данные!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string strokaPodkl = $"server={host};user={user};password={pass};charset=utf8;";
                using (MySqlConnection soed = new MySqlConnection(strokaPodkl))
                {
                    soed.Open();

                    DataTable tablicaBD = soed.GetSchema("Databases");
                    cmbDatabase.Items.Clear();
                    foreach(DataRow dr in tablicaBD.Rows)
                    {
                        string imyaBD = dr["database_name"].ToString();
                        cmbDatabase.Items.Add(imyaBD);
                    }
                    
                    soed.Close();
                }
                vremennayaStroka = strokaPodkl;
                cmbDatabase.IsEnabled = true;
                podkluchKBD.IsEnabled = true;

                if (cmbDatabase.Items.Contains(staryBD))
                {
                    cmbDatabase.SelectedItem = staryBD;
                }
                MessageBox.Show("Подключение к серверу возможно! Выберите базу данных.", "Успешное подключение", MessageBoxButton.OK, MessageBoxImage.Information);
            } catch
            {
                MessageBox.Show("Произошла ошибка подключения к серверу. Вероятно данные введены некорректно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                cmbDatabase.IsEnabled =false;
                podkluchKBD.IsEnabled=false;
                cmbDatabase.Items.Clear();
            }

            

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NovyyHost = staryHost;
            NovyyUser = staryUser;
            NovyyPassword = staryPassword;
            NovayaBD = staryBD;
            Podkluchenie = false;


            this.Close();
        }
    }
}
