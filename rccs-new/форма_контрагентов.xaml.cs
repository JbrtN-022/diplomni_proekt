using rccs.MyClass;
using rccs_new.MyClass;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для форма_контрагентов.xaml
    /// </summary>
    public partial class форма_контрагентов : Window
    {
        private int? selectedCounterpartyId = null;

        public форма_контрагентов()
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

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму контрагентов");

            LoadAllCounterparties();
            LoadFilters();

            // Скрытие кнопки удаления для менеджера (роль 1)
            if (ConnectionBD.roll == "1")
            {
                DelBtn.Visibility = Visibility.Collapsed;
                HistoryLogger.Log($"Для пользователя {ConnectionBD.resFio} скрыта кнопка удаления контрагентов");
            }
        }

        // Показывает справочное сообщение о форме контрагентов
        private void ShowHelp()
        {
            MessageBox.Show(
@"ФОРМА КОНТРАГЕНТОВ
Назначение формы:
Управление базой данных контрагентов (партнеров, клиентов, поставщиков).
Что можно сделать на этой форме:
• Просматривать список всех контрагентов
• Добавлять новых контрагентов
• Редактировать данные существующих контрагентов
• Удалять контрагентов (только для администратора)
• Искать контрагентов по различным критериям
• Фильтровать список по городам и типам лиц
Функциональные возможности:
1. ДОБАВЛЕНИЕ КОНТРАГЕНТА
   • Кнопка ""+""
   • Открывает форму для ввода информации о новом контрагенте
   • Обязательные поля: название, город, тип лица
2. РЕДАКТИРОВАНИЕ КОНТРАГЕНТА
   • Выберите контрагента из списка
   • Нажмите кнопку ""✎"" (карандаш)
   • Измените необходимые данные
3. УДАЛЕНИЕ КОНТРАГЕНТА
   • Доступно только для администратора
   • Выберите контрагента из списка
   • Нажмите кнопку ""🗑"" (корзина)
   • Подтвердите удаление
4. ПОИСК И ФИЛЬТРАЦИЯ
   • Поле ""Поиск"" - поиск по названию контрагента
   • Фильтр ""Город"" - отбор по городу
   • Фильтр ""Тип лица"" - отбор по типу (юр. лицо/физ. лицо)
5. КАРТОЧКИ КОНТРАГЕНТОВ
   • Отображают основную информацию
   • Клик по карточке - выбор контрагента
Примечание:
Перед удалением контрагента убедитесь, что он не используется в документах.
Удалить контрагента невозможно, если на него ссылаются договоры.",
                "Помощь - Форма контрагентов",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // Загрузка фильтров (города и типы лица)
        private void LoadFilters()
        {
            guideBD.selectGoroda();
            var cities = ConnectionBD.dtGoroda.AsEnumerable()
                .Select(row => new
                {
                    city = row.Field<string>("city"),
                    id_city = row.Field<int>("id_city")
                })
                .ToList();
            cities.Insert(0, new { city = "Все города", id_city = 0 });
            cmbCity.ItemsSource = cities;
            cmbCity.DisplayMemberPath = "city";
            cmbCity.SelectedValuePath = "id_city";
            cmbCity.SelectedIndex = 0;

            guideBD.selectVidLica();
            var types = ConnectionBD.dtVidLica.AsEnumerable()
                .Select(row => new
                {
                    type_of_face = row.Field<string>("type_of_face"),
                    id_type_of_face = row.Field<int>("id_type_of_face")
                })
                .ToList();
            types.Insert(0, new { type_of_face = "Все типы", id_type_of_face = 0 });
            cmbTypeFace.ItemsSource = types;
            cmbTypeFace.DisplayMemberPath = "type_of_face";
            cmbTypeFace.SelectedValuePath = "id_type_of_face";
            cmbTypeFace.SelectedIndex = 0;

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} загрузил фильтры контрагентов");
        }

        // Загрузка списка контрагентов с учётом поиска и фильтров
        private void LoadAllCounterparties(string search = "", int cityId = 0, int typeId = 0)
        {
            selectedCounterpartyId = null;
            UppBtn.IsEnabled = false;
            DelBtn.IsEnabled = false;

            counterparty.SelectCounterparty(itemsControlCounterparty, search, cityId, typeId);

            // Подписка на событие клика по карточке контрагента
            foreach (UserControlCounterparty card in itemsControlCounterparty.Items)
            {
                card.CardClicked -= Card_CardClicked;
                card.CardClicked += Card_CardClicked;
            }

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} обновил список контрагентов");
        }

        // Обработка выбора контрагента по клику на карточку
        private void Card_CardClicked(object sender, int id)
        {
            selectedCounterpartyId = id;
            UppBtn.IsEnabled = true;
            DelBtn.IsEnabled = true;
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} выбрал контрагента ID: {id}");
        }

        // Возврат в главное окно в зависимости от роли пользователя
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} нажал кнопку Назад");

            switch (ConnectionBD.roll)
            {
                case "1":
                    форма_менеджера formMEN = new форма_менеджера();
                    Application.Current.MainWindow = formMEN;
                    formMEN.Show();
                    this.Close();
                    break;
                case "2":
                    форма_бухгалтера formBuch = new форма_бухгалтера();
                    Application.Current.MainWindow = formBuch;
                    formBuch.Show();
                    this.Close();
                    break;
                case "3":
                    форма_администратора formadm = new форма_администратора();
                    Application.Current.MainWindow = formadm;
                    formadm.Show();
                    this.Close();
                    break;
                default:
                    MessageBox.Show("Неверные данные!");
                    HistoryLogger.Log("Ошибка: неизвестная роль пользователя");
                    break;
            }
        }

        // Открытие формы добавления нового контрагента
        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} открыл форму добавления контрагента");

            var addForm = new добавление_контрагента();

            if (addForm.ShowDialog() == true)
            {
                LoadAllCounterparties(
                    search: txtSearch.Text.Trim(),
                    cityId: cmbCity.SelectedValue != null ? Convert.ToInt32(cmbCity.SelectedValue) : 0,
                    typeId: cmbTypeFace.SelectedValue != null ? Convert.ToInt32(cmbTypeFace.SelectedValue) : 0
                );

                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} обновил список после добавления контрагента");
            }
        }

        // Удаление выбранного контрагента
        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCounterpartyId == null) return;

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} пытается удалить контрагента ID: {selectedCounterpartyId.Value}");

            var result = MessageBox.Show("Удалить этого контрагента?", "Подтверждение",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                counterparty.DeleteCounterparty(selectedCounterpartyId.Value);
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} удалил контрагента ID: {selectedCounterpartyId.Value}");
                LoadAllCounterparties();
            }
            else
            {
                HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} отменил удаление контрагента");
            }
        }

        // Открытие формы редактирования выбранного контрагента
        private void UppBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCounterpartyId == null) return;

            var addForm = new редактирование_контрагентов(selectedCounterpartyId.Value);

            if (addForm.ShowDialog() == true)
            {
                LoadAllCounterparties(
                    search: txtSearch.Text.Trim(),
                    cityId: cmbCity.SelectedValue != null ? Convert.ToInt32(cmbCity.SelectedValue) : 0,
                    typeId: cmbTypeFace.SelectedValue != null ? Convert.ToInt32(cmbTypeFace.SelectedValue) : 0
                );
            }
        }

        // Поиск при изменении текста в поле поиска
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            int cityId = cmbCity.SelectedValue is int c && c > 0 ? c : 0;
            int typeId = cmbTypeFace.SelectedValue is int t && t > 0 ? t : 0;

            LoadAllCounterparties(searchText, cityId, typeId);

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} выполнил поиск контрагентов: {searchText}");
        }

        // Применение фильтров (город / тип лица)
        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCity == null || cmbTypeFace == null) return;

            string searchText = txtSearch.Text.Trim();
            int cityId = cmbCity.SelectedValue is int c && c > 0 ? c : 0;
            int typeId = cmbTypeFace.SelectedValue is int t && t > 0 ? t : 0;

            LoadAllCounterparties(searchText, cityId, typeId);

            HistoryLogger.Log($"Пользователь {ConnectionBD.resFio} изменил фильтры контрагентов");
        }
    }
}