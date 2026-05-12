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
    /// Логика взаимодействия для форма_администратора.xaml
    /// </summary>
    public partial class форма_администратора : Window
    {
        public форма_администратора()
        {
            InitializeComponent();
            if (ConnectionBD.resFio.Length == 0)
            {
                title.Text = "ошибка";
            }
            title.Text = $@"С возвращением, {ConnectionBD.resFio}";
        }

        private void выйти_Click(object sender, RoutedEventArgs e)
        {
            MainWindow autoriz = new MainWindow();
            Application.Current.MainWindow = autoriz;
            autoriz.Show();

            this.Close();
        }

        private void пользователи_Click(object sender, RoutedEventArgs e)
        {
            форма_пользователей polzovateli = new форма_пользователей();
            Application.Current.MainWindow = polzovateli;
            polzovateli.Show();

            this.Close();
        }

        private void справочники_Click(object sender, RoutedEventArgs e)
        {
            форма_справочников spravochnik = new форма_справочников();
            Application.Current.MainWindow = spravochnik;
            spravochnik.Show();

            this.Close();
        }

        private void контрагенты_Click(object sender, RoutedEventArgs e)
        {
            
            форма_контрагентов spravochnik = new форма_контрагентов();
            Application.Current.MainWindow = spravochnik;
            spravochnik.Show();

            this.Close();
        }

        private void прайс_Click(object sender, RoutedEventArgs e)
        {
            форма_прайса spravochnik = new форма_прайса();
            Application.Current.MainWindow = spravochnik;
            spravochnik.Show();

            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void пользователь_Click(object sender, RoutedEventArgs e)
        {
            UserOverlap overlap = new UserOverlap();
            Application.Current.MainWindow = overlap;
            overlap.Show();
        }

        private void документы_Click(object sender, RoutedEventArgs e)
        {
            форма_договоров__лицензий spravochnik = new форма_договоров__лицензий ();
            Application.Current.MainWindow = spravochnik;
            spravochnik.Show();

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

