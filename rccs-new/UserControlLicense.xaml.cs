using rccs.MyClass;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace rccs_new
{
    public partial class UserControlLicenseAgreement : UserControl
    {
        public UserControlLicenseAgreement( string licenseId, string counterpartyName,  string programName, string dateFrom, string dateUntil, string conclusionDate, string workerName, string servicesList , string approved)
        {
            InitializeComponent();
            txtLicenseId.Text = licenseId ?? "-";
            txtCounterparty.Text = counterpartyName ?? "-";
            txtProgram.Text = programName ?? "-";
            txtDateFrom.Text = dateFrom ?? "-";
            txtDateUntil.Text = dateUntil ?? "-";
            txtConclusionDate.Text = conclusionDate ?? "-";
            txtWorker.Text = "Ответственный: " + (workerName ?? "-");
            txtServicesList.Text = servicesList ?? "Услуги не указаны";
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