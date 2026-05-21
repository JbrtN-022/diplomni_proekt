using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Логика взаимодействия для форма_прайса.xaml
    /// </summary>
    public partial class форма_прайса : Window
    {
        private PriceHistory.PriceType currentType;
        public форма_прайса()
        {
            InitializeComponent();
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.F1)
                {
                    ShowHelp();
                    e.Handled = true;
                }
            };

            cmbPrice.Items.Add("Прайс за м²");
            cmbPrice.Items.Add("Прайс программ");
            cmbPrice.Items.Add("Прайс услуг");

            cmbPrice.SelectedIndex = 0;
        }
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА УПРАВЛЕНИЯ ПРАЙС-ЛИСТАМИ

Назначение формы:
Управление ценами на аренду помещений, программное обеспечение и услуги.

Что можно сделать на этой форме:
• Просматривать историю изменения цен
• Добавлять новые цены
• Удалять устаревшие цены
• Переключаться между разными типами прайсов

Типы прайс-листов:

1. ПРАЙС ЗА М²
   • Цены на аренду 1 квадратного метра помещения
   • Привязка к конкретным офисам/помещениям
   • Позволяет отслеживать изменение стоимости аренды

2. ПРАЙС ПРОГРАММ
   • Цены на лицензирование программного обеспечения
   • Привязка к конкретным программам
   • История изменения стоимости ПО

3. ПРАЙС УСЛУГ
   • Цены на дополнительные услуги
   • Привязка к конкретным услугам
   • Отслеживание изменения стоимости услуг

Функциональные возможности:

1. ДОБАВЛЕНИЕ ЦЕНЫ
   • Выберите тип прайса в выпадающем списке
   • Выберите объект (помещение/программу/услугу)
   • Введите новую цену
   • Нажмите кнопку ""+""

2. УДАЛЕНИЕ ЦЕНЫ
   • Выберите запись в таблице
   • Нажмите кнопку ""🗑"" (корзина)
   • Цена будет удалена из истории

3. ПРОСМОТР ИСТОРИИ
   • Таблица показывает все изменения цен
   • Столбцы: Объект, Прайс, Дата
   • История сохраняется для анализа динамики цен

4. ПЕРЕКЛЮЧЕНИЕ МЕЖДУ ТИПАМИ
   • Выпадающий список для выбора типа прайса
   • При переключении автоматически загружаются соответствующие данные



Примечание:
Все изменения цен сохраняются с датой изменения.
Система хранит полную историю изменения цен.
Новые цены автоматически применяются при оформлении документов.
Цены можно удалять, но рекомендуется оставлять историю для отчетности.",
                "Помощь - Управление прайс-листами",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            форма_администратора formadm = new форма_администратора();
            Application.Current.MainWindow = formadm;
            formadm.Show();
            this.Close();
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dgPriceTable.SelectedItem == null)
            {
                MessageBox.Show("Выберите запись!");
                return;
            }

            DataRowView row =
                dgPriceTable.SelectedItem as DataRowView;

            string objectName =
                row["Объект"].ToString();

            decimal price =
                Convert.ToDecimal(row["Прайс"]);

            string date =
                row["Дата"].ToString();

            bool result =
                PriceHistory.DeletePrice(
                    currentType,
                    objectName,
                    price,
                    date);

            if (result)
            {
                MessageBox.Show("Удалено!");

                PriceHistory.LoadPriceData(
                    dgPriceTable,
                    currentType);
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Введите цену!");
                return;
            }

            if (cmbLink.SelectedValue == null)
            {
                MessageBox.Show("Выберите объект!");
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Ошибка цены!");
                return;
            }

            int id = Convert.ToInt32(cmbLink.SelectedValue);

            if (PriceHistory.AddPrice(currentType, price, id))
            {
                MessageBox.Show("Добавлено!");
                txtPrice.Clear();

                PriceHistory.LoadPriceData(dgPriceTable, currentType);
            }
        }

        private void cmbPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentType = (PriceHistory.PriceType)(cmbPrice.SelectedIndex + 1);

            switch (currentType)
            {
                case PriceHistory.PriceType.Meter:
                    guideBD.selectOffice();
                    cmbLink.ItemsSource = ConnectionBD.dtOffice.DefaultView;
                    cmbLink.DisplayMemberPath = "office";
                    cmbLink.SelectedValuePath = "id_office";
                    break;

                case PriceHistory.PriceType.Program:
                    guideBD.selectPrograms();
                    cmbLink.ItemsSource = ConnectionBD.dtProgramsComboBox.DefaultView;
                    cmbLink.DisplayMemberPath = "name";
                    cmbLink.SelectedValuePath = "id_program";                    
                    break;

                case PriceHistory.PriceType.Service:
                    guideBD.selectServices();
                    cmbLink.ItemsSource = ConnectionBD.dtServicesComboBox.DefaultView;
                    cmbLink.DisplayMemberPath = "name";
                    cmbLink.SelectedValuePath = "id_services";                
                    break;
            }

            cmbLink.SelectedIndex = 0;

            PriceHistory.LoadPriceData(dgPriceTable, currentType);
        }
        private void dgSpravochnik_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
