using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для UserControlCounterparty.xaml
    /// Пользовательский контрол для отображения карточки контрагента
    /// </summary>
    public partial class UserControlCounterparty : UserControl
    {
        /// <summary>
        /// ID контрагента
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Событие, возникающее при клике по карточке контрагента
        /// </summary>
        public event EventHandler<int> CardClicked;

        /// <summary>
        /// Конструктор карточки контрагента
        /// </summary>
        public UserControlCounterparty(string id, string name, string city, string typeFace,
                                       string actualAddress, string legalAddress,
                                       string email, string phone, string inn, string bic,
                                       string contPerson)
        {
            InitializeComponent();

            Id = int.Parse(id);

            // Заполнение данных в элементах управления
            txtName.Text = $@"{name} • {typeFace}";
            txtAdress.Text = $"{city ?? "—"} • {actualAddress ?? "—"}";
            txtContPerson.Text = contPerson ?? "—";
            txtPhone.Text = phone ?? "—";
            txtEmail.Text = email ?? "—";
            txtINN.Text = inn ?? "—";
            txtBIC.Text = bic ?? "—";
        }

        /// <summary>
        /// Обработка клика по всей карточке контрагента
        /// </summary>
        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CardClicked?.Invoke(this, Id);
        }
    }
}