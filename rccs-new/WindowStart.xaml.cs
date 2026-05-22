using System;
using System.Windows;
using System.Windows.Threading;

namespace rccs_new
{
    /// <summary>
    /// Логика взаимодействия для WindowStart.xaml
    /// Стартовое окно (splash screen) приложения
    /// </summary>
    public partial class WindowStart : Window
    {
        public WindowStart()
        {
            InitializeComponent();
        }

        // Обработка события загрузки окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Создаём таймер на 5 секунд
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            timer.Tick += (s, args) =>
            {
                timer.Stop();

                // Скрываем стартовое окно
                this.Hide();

                // Открываем форму авторизации
                MainWindow authForm = new MainWindow();
                authForm.Show();

                // Закрываем текущее окно
                this.Close();
            };

            timer.Start();
        }

        // Пустой обработчик (оставлен на случай, если потребуется в будущем)
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}