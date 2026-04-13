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
    /// Логика взаимодействия для форма_менеджера.xaml
    /// </summary>
    public partial class форма_менеджера : Window
    {
        public форма_менеджера()
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

        private void выйти_Click(object sender, RoutedEventArgs e)
        {
            MainWindow autoriz = new MainWindow();
            Application.Current.MainWindow = autoriz;
            autoriz.Show();

            this.Close();
        }
        private void ReplaceText(DocumentFormat.OpenXml.Wordprocessing.Body body, string placeholder, string newText)
        {
            foreach (var text in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, newText);
                }
            }
        }
        private void печать_Click(object sender, RoutedEventArgs e)
        {

        }
        

    }
}
