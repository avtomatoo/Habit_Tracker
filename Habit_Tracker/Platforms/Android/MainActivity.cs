using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace Habit_Tracker
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Настройка статус бара (верхней панели)
            SetStatusBarColor();
        }

        protected override void OnResume()
        {
            base.OnResume();
            // Обновляем настройки при возвращении в приложение
            SetStatusBarColor();
        }

        private void SetStatusBarColor()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                // Убеждаемся, что окно доступно
                if (Window == null) return;

                // Убираем тень под статус баром для более чистого вида
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);

                // Устанавливаем цвет статус бара
                // Замените "#2196F3" на желаемый цвет
                Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#6F42C1"));

                // Настройка цвета текста и иконок:
                // Для светлого текста на темном фоне - используйте 0
                // Для темного текста на светлом фоне - используйте SystemUiFlags.LightStatusBar

                // Пример для темного текста (для светлых фонов):
                Window.DecorView.SystemUiFlags = SystemUiFlags.LightStatusBar;

                // ИЛИ для светлого текста (для темных фонов):
                // Window.DecorView.SystemUiFlags = 0;
            }
        }
    }
}
