using Microsoft.Maui.Controls;

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
            FrequencyPicker.Items.Add("Ежедневно");
            FrequencyPicker.Items.Add("Каждые 2 дня");
            FrequencyPicker.Items.Add("Раз в неделю");
            FrequencyPicker.Items.Add("Раз в месяц");
            FrequencyPicker.Items.Add("Только по будням");
            FrequencyPicker.Items.Add("Только по выходным");

            FrequencyPicker.SelectedIndex = 0;
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
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
                LastCompletedDate = DateTime.MinValue, // Никогда не выполнялась
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
    }
}