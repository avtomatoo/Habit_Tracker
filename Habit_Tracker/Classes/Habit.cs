using SQLite;

namespace Habit_Tracker
{
    public class Habit
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Frequency { get; set; }
        public TimeSpan ReminderTime { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsCompleted { get; set; }

        // Для отображения времени в удобном формате
        public string DisplayTime => ReminderTime.ToString(@"hh\:mm");
    }
}