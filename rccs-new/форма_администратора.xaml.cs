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
    }
}

