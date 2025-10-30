using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace Habit_Tracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
            MainPage = new NavigationPage(new MainPage());

            // Инициализация обработчиков уведомлений
            InitializeNotificationHandlers();
        }

        private void InitializeNotificationHandlers()
        {
            // Обработка нажатия на уведомление
            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;

            // Обработка получения уведомления
            LocalNotificationCenter.Current.NotificationReceived += OnNotificationReceived;
        }

        private void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            // Обработка нажатия на уведомление
            if (e.Request is NotificationRequest request)
            {
                // Можно открыть конкретную привычку или главную страницу
                // request.NotificationId содержит ID привычки (Id + 1000)
                var habitId = request.NotificationId - 1000;

                // Здесь можно добавить логику для перехода к конкретной привычке
                // Например, открыть страницу с деталями привычки
                System.Diagnostics.Debug.WriteLine($"Нажато уведомление для привычки ID: {habitId}");

                // Показываем сообщение при нажатии на уведомление
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await MainPage?.DisplayAlert("Напоминание", request.Description, "OK");
                });
            }
        }

        private void OnNotificationReceived(NotificationEventArgs e)
        {
            // Обработка получения уведомления (когда приложение активно)
            if (e.Request is NotificationRequest request)
            {
                System.Diagnostics.Debug.WriteLine($"Получено уведомление: {request.Title} - {request.Description}");
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            System.Diagnostics.Debug.WriteLine("Приложение запущено");
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            System.Diagnostics.Debug.WriteLine("Приложение перешло в спящий режим");
        }

        protected override void OnResume()
        {
            base.OnResume();
            System.Diagnostics.Debug.WriteLine("Приложение возобновило работу");
        }
    }
}