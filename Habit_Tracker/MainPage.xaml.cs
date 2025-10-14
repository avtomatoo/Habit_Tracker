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

            // ВАЖНО: Установите ItemsSource
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
                    Habits.Add(habit);
                }

                // Для отладки - проверьте количество элементов
                System.Diagnostics.Debug.WriteLine($"Загружено привычек: {Habits.Count}");
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
    }
}