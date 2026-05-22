using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Windows;
using System.Windows.Input;

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

            // Подключение обработчика клавиши F1 для вызова справки
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.F1)
                {
                    ShowHelp();
                    e.Handled = true;
                }
            };

            // Установка приветствия в заголовке окна
            if (ConnectionBD.resFio.Length == 0)
            {
                title.Text = "ошибка";
            }
            title.Text = $@"С возвращением, {ConnectionBD.resFio}";

            // Логирование входа пользователя
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} авторизовался как бухгалтер");
        }

        // Показывает справочное сообщение о форме бухгалтера
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА БУХГАЛТЕРА
Назначение формы:
Главная панель управления для сотрудника бухгалтерии с доступом к финансовым документам.
Что можно сделать на этой форме:
• Управлять контрагентами
• Просматривать и создавать договоры аренды
• Просматривать данные текущего пользователя
• Выход из системы
Доступные разделы:
1. КОНТРАГЕНТЫ
   • Просмотр списка контрагентов
   • Добавление новых контрагентов
   • Редактирование данных контрагентов
   • Удаление контрагентов
2. ПРОСМОТР ДОГОВОРОВ АРЕНДЫ
   • Просмотр всех заключенных договоров аренды
   • Поиск и фильтрация договоров
   • Печать договоров
3. ОФОРМЛЕНИЕ ДОГОВОРОВ
   • Создание новых договоров аренды
   • Заполнение информации по договору
   • Расчет арендной платы
   • Сохранение и печать договоров
4. ДАННЫЕ ПОЛЬЗОВАТЕЛЯ
   • Просмотр информации о текущем пользователе
5. ВЫХОД
   • Завершение текущей сессии и возврат к форме авторизации
",
                "Помощь - Форма бухгалтера",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // Открытие формы контрагентов
        private void контрагенты_Click(object sender, RoutedEventArgs e)
        {
            форма_контрагентов spravochnik = new форма_контрагентов();
            Application.Current.MainWindow = spravochnik;
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму контрагентов");
            spravochnik.Show();
            this.Close();
        }

        // Создание документа статистики по договорам
        private void печать_Click(object sender, RoutedEventArgs e)
        {
            MyClass.document.document_statistic_agreement.CreateStatisticAgreementDocument();
        }

        // Открытие формы данных пользователя
        private void пользователь_Click(object sender, RoutedEventArgs e)
        {
            UserOverlap overlap = new UserOverlap();
            Application.Current.MainWindow = overlap;
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму списка пользователей");
            overlap.Show();
            this.Close();
        }

        // Выход из системы и возврат на форму авторизации
        private void выйти_Click(object sender, RoutedEventArgs e)
        {
            MainWindow autoriz = new MainWindow();
            Application.Current.MainWindow = autoriz;
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} вышел из профиля");
            autoriz.Show();
            this.Close();

            ConnectionBD.roll = null;
            ConnectionBD.resFio = null;
        }

        // Открытие формы просмотра договоров аренды
        private void Просмотр_договоров_аренды_Click(object sender, RoutedEventArgs e)
        {
            просмотр_договоров_аренды arenda = new просмотр_договоров_аренды();
            Application.Current.MainWindow = arenda;
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму просмотр договоров аренды");
            arenda.Show();
            this.Close();
        }

        // Открытие формы создания нового договора аренды
        private void Оформление_договоров_Click(object sender, RoutedEventArgs e)
        {
            создание_договора_аренды oform = new создание_договора_аренды();
            Application.Current.MainWindow = oform;
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму создание договоров аренды");
            oform.Show();
            this.Close();
        }
    }
}