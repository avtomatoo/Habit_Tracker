using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using Plugin.LocalNotification;
using Microsoft.Maui.ApplicationModel;

namespace Habit_Tracker
{
    public partial class MainPage : ContentPage
    {
        private readonly HabitDatabase _database;
        public ObservableCollection<Habit> Habits { get; set; }
        private bool _permissionChecked = false;

        public MainPage()
        {
            InitializeComponent();
            _database = new HabitDatabase();
            Habits = new ObservableCollection<Habit>();
            HabitsCollectionView.ItemsSource = Habits;

            LoadHabits();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Проверяем разрешения только при первом открытии
            if (!_permissionChecked)
            {
                _permissionChecked = true;
                await CheckNotificationPermission();
            }

            await LoadHabits();
            await RescheduleAllNotifications();
        }

        private async Task CheckNotificationPermission()
        {
            try
            {
                // Проверяем статус разрешений
                var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

                // Если уведомления запрещены, показываем сообщение
                if (status != PermissionStatus.Granted)
                {
                    await ShowNotificationInstruction();
                }
                // Если разрешены - ничего не делаем, уведомления будут работать автоматически
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка проверки разрешений: {ex.Message}");
            }
        }

        private async Task ShowNotificationInstruction()
        {
            string instructions = GetPlatformSpecificInstructions();

            await DisplayAlert(
                "Уведомления отключены",
                $"Включите уведомления в настройках, чтобы получать напоминания о привычках.\n\n{instructions}",
                "Понятно"
            );
        }

        private string GetPlatformSpecificInstructions()
        {
#if ANDROID
            return "Как включить:\n" +
                   "1. Настройки → Приложения\n" +
                   "2. Найдите 'Habit Tracker'\n" +
                   "3. Нажмите 'Уведомления'\n" +
                   "4. Включите уведомления";
#elif IOS
            return "Как включить:\n" +
                   "1. Настройки → Habit Tracker\n" +
                   "2. Нажмите 'Уведомления'\n" +
                   "3. Включите 'Разрешить уведомления'";
#else
            return "Перейдите в настройки устройства, найдите приложение 'Habit Tracker' и включите уведомления.";
#endif
        }

        private async Task RescheduleAllNotifications()
        {
            try
            {
                var habits = await _database.GetHabitsAsync();
                foreach (var habit in habits)
                {
                    await habit.ScheduleNotification();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при планировании уведомлений: {ex.Message}");
            }
        }

        private async Task LoadHabits()
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
                await DisplayAlert("Ошибка", "Не удалось загрузить привычки", "OK");
            }
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Информация", "Раздел настроек в разработке", "OK");
        }

        private async void OnHomeClicked(object sender, EventArgs e)
        {
            // Уже на главной странице 
            await DisplayAlert("Информация", "Вы уже на главной странице", "OK");
        }

        private async void OnStatisticsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new StatisticsPage());
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
                // Отменяем уведомление перед удалением
                habit.CancelNotification();

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

                // Перепланируем уведомление после выполнения
                await habit.ScheduleNotification();

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