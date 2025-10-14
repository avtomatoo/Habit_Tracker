using Microsoft.Maui.Controls;
using System;

namespace Habit_Tracker
{
    public partial class CreatePage : ContentPage
    {
        private readonly HabitDatabase _database;

        public CreatePage()
        {
            InitializeComponent();
            InitializeFrequencyPicker();
            _database = new HabitDatabase();

            // Устанавливаем время по умолчанию на 09:00
            ReminderTimePicker.Time = new TimeSpan(9, 0, 0);
        }

        private void InitializeFrequencyPicker()
        {
            // Очищаем существующие элементы
            FrequencyPicker.Items.Clear();

            // Добавляем варианты частоты напоминаний
            FrequencyPicker.Items.Add("Ежедневно");
            FrequencyPicker.Items.Add("Каждые 2 дня");
            FrequencyPicker.Items.Add("Раз в неделю");
            FrequencyPicker.Items.Add("Раз в месяц");
            FrequencyPicker.Items.Add("Только по будням");
            FrequencyPicker.Items.Add("Только по выходным");

            // Устанавливаем значение по умолчанию
            FrequencyPicker.SelectedIndex = 0;
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            // Возвращаемся на главную страницу
            await Navigation.PopAsync();
        }

        private async void OnCreateClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HabitNameEntry.Text))
            {
                await DisplayAlert("Ошибка", "Пожалуйста, введите название привычки", "OK");
                return;
            }

            if (FrequencyPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Ошибка", "Пожалуйста, выберите частоту напоминаний", "OK");
                return;
            }

            var newHabit = new Habit
            {
                Name = HabitNameEntry.Text.Trim(),
                Frequency = FrequencyPicker.SelectedItem.ToString(),
                ReminderTime = ReminderTimePicker.Time,
                CreationDate = DateTime.Now,
                LastCompletedDate = DateTime.MinValue,
                CompletionCount = 0,
                IsCompletedToday = false
            };

            // Сохраняем в базу данных
            var result = await _database.SaveHabitAsync(newHabit);

            if (result > 0)
            {
                await DisplayAlert("Успех", $"Привычка \"{newHabit.Name}\" создана!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось сохранить привычку", "OK");
            }
        }

        // Обработчик изменения времени
        private void OnTimePickerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
            {
                // Можно добавить дополнительную логику при изменении времени
                System.Diagnostics.Debug.WriteLine($"Время напоминания изменено на: {ReminderTimePicker.Time}");
            }
        }

        // Обработчик изменения выбора частоты
        private void OnFrequencyPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (FrequencyPicker.SelectedIndex != -1)
            {
                var selectedFrequency = FrequencyPicker.SelectedItem.ToString();
                System.Diagnostics.Debug.WriteLine($"Выбрана частота: {selectedFrequency}");

                // Можно добавить дополнительную логику в зависимости от выбранной частоты
                UpdateFrequencyInfo(selectedFrequency);
            }
        }

        // Метод для обновления информации о частоте (можно расширить)
        private void UpdateFrequencyInfo(string frequency)
        {
            // Здесь можно добавить логику для отображения подсказок
            // о выбранной частоте, если нужно
            switch (frequency)
            {
                case "Только по будням":
                    // Можно показать подсказку
                    break;
                case "Только по выходным":
                    // Можно показать подсказку
                    break;
                case "Каждые 2 дня":
                    // Можно показать подсказку
                    break;
            }
        }

        // Обработчик нажатия на поле ввода названия
        private void OnHabitNameEntryFocused(object sender, FocusEventArgs e)
        {
            // Можно добавить анимацию или подсветку поля
            if (sender is Entry entry)
            {
                entry.BackgroundColor = Color.FromArgb("#F5F5F5");
            }
        }

        private void OnHabitNameEntryUnfocused(object sender, FocusEventArgs e)
        {
            // Возвращаем стандартный цвет
            if (sender is Entry entry)
            {
                entry.BackgroundColor = Color.FromArgb("#FFFFFF");
            }
        }

        // Метод для очистки формы (можно использовать при необходимости)
        private void ClearForm()
        {
            HabitNameEntry.Text = string.Empty;
            FrequencyPicker.SelectedIndex = 0;
            ReminderTimePicker.Time = new TimeSpan(9, 0, 0);
        }

        // Обработчик загрузки страницы
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Можно добавить анимацию появления или другие действия
            // при каждом появлении страницы
            System.Diagnostics.Debug.WriteLine("Страница создания привычки открыта");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Очистка ресурсов или другие действия при закрытии страницы
            System.Diagnostics.Debug.WriteLine("Страница создания привычки закрыта");
        }
    }
}