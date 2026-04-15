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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для UserControlLease.xaml
    /// </summary>
    public partial class UserControlLease : UserControl
    {
        public UserControlLease(string leaseID, string counterpartyName, string roomNum, string dateFrom, string dateUntil, string conclusionDate, string workerName, string approved)
        {
            InitializeComponent();
            txtLeaseId.Text = leaseID;
            txtCounterparty.Text = counterpartyName;
            txtOffice.Text = roomNum;
            txtDateFrom.Text = dateFrom;
            txtDateUntil.Text = dateUntil;
            txtWorker.Text = workerName;
            if (approved == "Не утверждён")
            {
                txtApproved.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCE4436"));
            }
            if (approved == "Утверждён")
            {
                txtApproved.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
            }
            txtApproved.Text = approved ?? "-";
        }
    }
    
}
