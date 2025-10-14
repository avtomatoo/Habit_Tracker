using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls;

namespace Habit_Tracker
{
    public partial class StatisticsPage : ContentPage
    {
        private readonly HabitDatabase _database;
        public ObservableCollection<HabitStatistics> HabitStatistics { get; set; }

        public StatisticsPage()
        {
            InitializeComponent();
            _database = new HabitDatabase();
            HabitStatistics = new ObservableCollection<HabitStatistics>();

            // Убедимся, что CarouselView существует перед установкой ItemsSource
            if (StatisticsCarouselView != null)
            {
                StatisticsCarouselView.ItemsSource = HabitStatistics;
            }

            LoadStatistics();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadStatistics();
        }

        private async void LoadStatistics()
        {
            try
            {
                var habits = await _database.GetHabitsAsync();

                HabitStatistics.Clear();
                foreach (var habit in habits)
                {
                    var stats = new HabitStatistics { Habit = habit };
                    HabitStatistics.Add(stats);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить статистику: {ex.Message}", "OK");
            }
        }

        // Навигация по нижней панели
        private async void OnHomeClicked(object sender, EventArgs e)
        {
            // Уже на странице статистики
            await DisplayAlert("Информация", "Вы уже на странице статистики", "OK");
        }

        private async void OnHabitsClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Информация", "Раздел настроек в разработке", "OK");
        }
    }
}