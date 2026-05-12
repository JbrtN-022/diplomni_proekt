using rccs.MyClass;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
    /// Логика взаимодействия для форма_бухгалтера.xaml
    /// </summary>
    public partial class форма_бухгалтера : Window
    {
        public форма_бухгалтера()
        {
            InitializeComponent();
            if (ConnectionBD.resFio.Length == 0)
            {
                title.Text = "ошибка";
            }
            title.Text = $@"С возвращением, {ConnectionBD.resFio}";
        }

        private void контрагенты_Click(object sender, RoutedEventArgs e)
        {
          
            форма_контрагентов spravochnik = new форма_контрагентов();
            Application.Current.MainWindow = spravochnik;
            spravochnik.Show();

            this.Close();
        }

        private void печать_Click(object sender, RoutedEventArgs e)
        {

        }

        private void пользователь_Click(object sender, RoutedEventArgs e)
        {
            UserOverlap overlap = new UserOverlap();
            Application.Current.MainWindow = overlap;
            overlap.Show();
        }

        private void выйти_Click(object sender, RoutedEventArgs e)
        {
           
            MainWindow autoriz = new MainWindow();
            Application.Current.MainWindow = autoriz;
            autoriz.Show();

            this.Close();
        }

        private void Просмотр_договоров_аренды_Click(object sender, RoutedEventArgs e)
        {
          
            просмотр_договоров_аренды arenda = new просмотр_договоров_аренды();
            Application.Current.MainWindow = arenda;
            arenda.Show();

            this.Close();
        }

        private void Оформление_договоров_Click(object sender, RoutedEventArgs e)
        {
            
            создание_договора_аренды oform = new создание_договора_аренды();
            Application.Current.MainWindow = oform;
            oform.Show();

            this.Close();
        }

        private void подключение_Click(object sender, RoutedEventArgs e)
        {
            форма__насторойки_подключения_бд openedWindow = null;

            foreach (Window window in Application.Current.Windows)
            {
                if (window is форма__насторойки_подключения_бд)
                {
                    openedWindow =
                        (форма__насторойки_подключения_бд)window;

                    break;
                }
            }

            if (openedWindow == null)
            {
                форма__насторойки_подключения_бд formpodkl =
                    new форма__насторойки_подключения_бд(
                        ConnectionBD.currentDataSource,
                        ConnectionBD.currentUser,
                        ConnectionBD.currentPassword,
                        ConnectionBD.currentDataBase);

                bool? result = formpodkl.ShowDialog();



                if (result == true)
                {
                    if (ConnectionBD.ConnectBD(
                        formpodkl.NovayaBD,
                        formpodkl.NovyyHost,
                        formpodkl.NovyyUser,
                        formpodkl.NovyyPassword))
                    {
                        MessageBox.Show(
                            "Настройки подключения обновлены",
                            "Успешно",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
            }

            else
            {
                MessageBox.Show(
                    "Нельзя открыть больше 1 окна!",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                openedWindow.Activate();
                openedWindow.Focus();
            }

        }
    }
}
