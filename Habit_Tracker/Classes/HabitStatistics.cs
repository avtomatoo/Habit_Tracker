using System.ComponentModel;

namespace Habit_Tracker
{
    public class HabitStatistics : INotifyPropertyChanged
    {
        public Habit Habit { get; set; }

        public string Name => Habit?.Name ?? "Неизвестная привычка";
        public int CompletionCount => Habit?.CompletionCount ?? 0;

        // Дни существования привычки
        public string DaysExists
        {
            get
            {
                if (Habit == null) return "0 дней";

                var days = (DateTime.Now - Habit.CreationDate).Days;
                return $"{days} дней";
            }
        }

        // Процент успеха
        public string SuccessRate
        {
            get
            {
                if (Habit == null) return "0%";

                var daysExists = (DateTime.Now - Habit.CreationDate).Days;
                if (daysExists == 0) return "0%";

                var rate = (double)Habit.CompletionCount / daysExists * 100;
                return $"{rate:0}%";
            }
        }

        // Информация о частоте
        public string FrequencyInfo => $"Частота: {Habit?.Frequency ?? "Неизвестно"}";

        // Дата создания
        public string CreationDateInfo => $"Создана: {Habit?.CreationDate:dd.MM.yyyy}";

        // Данные для графика (простая реализация)
        public int MondayHeight => new Random().Next(20, 80);
        public int TuesdayHeight => new Random().Next(20, 80);
        public int WednesdayHeight => new Random().Next(20, 80);
        public int ThursdayHeight => new Random().Next(20, 80);
        public int FridayHeight => new Random().Next(20, 80);
        public int SaturdayHeight => new Random().Next(20, 80);
        public int SundayHeight => new Random().Next(20, 80);

        public int MondayCount => new Random().Next(1, 5);
        public int TuesdayCount => new Random().Next(1, 5);
        public int WednesdayCount => new Random().Next(1, 5);
        public int ThursdayCount => new Random().Next(1, 5);
        public int FridayCount => new Random().Next(1, 5);
        public int SaturdayCount => new Random().Next(1, 5);
        public int SundayCount => new Random().Next(1, 5);

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}