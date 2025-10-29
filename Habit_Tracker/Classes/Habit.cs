using SQLite;
using System.ComponentModel;

namespace Habit_Tracker
{
    public class Habit : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Frequency { get; set; }
        public TimeSpan ReminderTime { get; set; }
        public DateTime CreationDate { get; set; }

        // Новые поля для отслеживания выполнения
        public DateTime LastCompletedDate { get; set; }
        public int CompletionCount { get; set; }

        // Вычисляемые свойства для отображения
        public string DisplayTime => ReminderTime.ToString(@"hh\:mm");

        private bool _isCompletedToday;
        public bool IsCompletedToday
        {
            get => _isCompletedToday;
            set
            {
                _isCompletedToday = value;
                OnPropertyChanged(nameof(IsCompletedToday));
                OnPropertyChanged(nameof(StatusColor));
                OnPropertyChanged(nameof(CompletionText));
            }
        }

        // Цвет кнопки выполнения
        public string StatusColor => IsCompletedToday ? "#9E9E9E" : "#6F42C1";

        // Текст кнопки выполнения
        public string CompletionText => IsCompletedToday ? "Отмечено" : "Отметить";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Метод для проверки, можно ли выполнить привычку сегодня
        public bool CanCompleteToday()
        {
            var today = DateTime.Today;

            // Если уже выполнено сегодня
            if (IsCompletedToday)
                return false;

            return Frequency switch
            {
                "Ежедневно" => true,
                "Каждые 2 дня" => LastCompletedDate == DateTime.MinValue || (today - LastCompletedDate.Date).TotalDays >= 2,
                "Раз в неделю" => LastCompletedDate == DateTime.MinValue || (today - LastCompletedDate.Date).TotalDays >= 7,
                "Раз в месяц" => LastCompletedDate == DateTime.MinValue || (today - LastCompletedDate.Date).TotalDays >= 30,
                "Только по будням" => today.DayOfWeek != DayOfWeek.Saturday && today.DayOfWeek != DayOfWeek.Sunday,
                "Только по выходным" => today.DayOfWeek == DayOfWeek.Saturday || today.DayOfWeek == DayOfWeek.Sunday,
                _ => true
            };
        }

        // Метод для выполнения привычки
        public void MarkCompleted()
        {
            LastCompletedDate = DateTime.Now;
            CompletionCount++;
            IsCompletedToday = true;
        }

        // Метод для сброса выполнения (на следующий день)
        public void ResetCompletion()
        {
            if (LastCompletedDate.Date < DateTime.Today)
            {
                IsCompletedToday = false;
            }
        }
    }
}