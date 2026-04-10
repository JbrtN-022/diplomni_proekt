using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для UserControlCounterparty.xaml
    /// </summary>
    public partial class UserControlCounterparty : UserControl
    {
        public UserControlCounterparty(string name, string city, string typeFace,
                                       string actualAddress, string legalAddress,
                                       string email, string phone, string inn, string bic,
                                       string contPerson)
        {
            InitializeComponent();
            txtName.Text = $@"{name} • {typeFace}";
            txtAdress.Text = $"{city ?? "—"} • {actualAddress ?? "—"}";                  
            txtContPerson.Text = contPerson ?? "—";
            txtPhone.Text = phone ?? "—";
            txtEmail.Text = email ?? "—";
            txtINN.Text = inn ?? "—";
            txtBIC.Text = bic ?? "—";
        }
    }
}
