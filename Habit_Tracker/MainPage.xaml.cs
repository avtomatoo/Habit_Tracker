using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace Habit_Tracker
{
    public partial class MainPage : ContentPage
    {
        private readonly HabitDatabase _database;
        public ObservableCollection<Habit> Habits { get; set; }

        public MainPage()
        {
            InitializeComponent();
            _database = new HabitDatabase();
            Habits = new ObservableCollection<Habit>();
            HabitsCollectionView.ItemsSource = Habits;

            LoadHabits();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadHabits();
        }

        private async void LoadHabits()
        {
            try
            {
                var habits = await _database.GetHabitsAsync();

                Habits.Clear();
                foreach (var habit in habits)
                {
                    // Сбрасываем статус выполнения для нового дня
                    habit.ResetCompletion();
                    Habits.Add(habit);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки привычек: {ex.Message}");
            }
        }

        private async void OnAddHabitClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreatePage());
        }

        private async void OnHabitDeleteClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var habit = (Habit)button.BindingContext;

            bool answer = await DisplayAlert("Удаление",
                $"Удалить привычку \"{habit.Name}\"?",
                "Да", "Нет");

            if (answer)
            {
                await _database.DeleteHabitAsync(habit);
                Habits.Remove(habit);
                await DisplayAlert("Успех", "Привычка удалена!", "OK");
            }
        }

        private async void OnHabitCompleteClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var habit = (Habit)button.BindingContext;

            if (!habit.CanCompleteToday())
            {
                string message = habit.Frequency switch
                {
                    "Только по будням" => "Эту привычку можно выполнять только по будним дням",
                    "Только по выходным" => "Эту привычку можно выполнять только по выходным",
                    "Каждые 2 дня" => "Эту привычку можно выполнять раз в 2 дня",
                    "Раз в неделю" => "Эту привычку можно выполнять раз в неделю",
                    "Раз в месяц" => "Эту привычку можно выполнять раз в месяц",
                    _ => "Эта привычка уже выполнена сегодня"
                };

                await DisplayAlert("Информация", message, "OK");
                return;
            }

            bool answer = await DisplayAlert("Подтверждение",
                $"Отметить привычку \"{habit.Name}\" как выполненную?",
                "Да", "Нет");

            if (answer)
            {
                habit.MarkCompleted();
                await _database.UpdateHabitAsync(habit);

                // Обновляем отображение
                var index = Habits.IndexOf(habit);
                if (index != -1)
                {
                    Habits[index] = habit; // Это вызовет обновление через INotifyPropertyChanged
                }

                await DisplayAlert("Успех", "Привычка отмечена как выполненная!", "OK");
            }
        }
    }
}