using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
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

        // ID уведомления для управления
        public int NotificationId => Id + 1000; // Уникальный ID для каждого habit

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

            // Перепланируем уведомление после выполнения
            Task.Run(async () => await ScheduleNotification());
        }

        // Метод для сброса выполнения (на следующий день)
        public void ResetCompletion()
        {
            if (LastCompletedDate.Date < DateTime.Today)
            {
                IsCompletedToday = false;
            }
        }

        // МЕТОДЫ ДЛЯ УВЕДОМЛЕНИЙ

        /// <summary>
        /// Планирует уведомление для привычки
        /// </summary>
        public async Task ScheduleNotification()
        {
            try
            {
                // Отменяем старое уведомление
                CancelNotification();

                // Если привычка уже выполнена сегодня - не показываем уведомление
                if (IsCompletedToday)
                    return;

                var notificationTime = CalculateNextNotificationTime();

                if (notificationTime.HasValue && notificationTime.Value > DateTime.Now)
                {
                    var request = new NotificationRequest
                    {
                        NotificationId = NotificationId,
                        Title = "Напоминание о привычке",
                        Description = $"Не забудьте выполнить: {Name}",
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = notificationTime.Value
                        },
                        Android = new AndroidOptions
                        {
                            ChannelId = "habit_reminders",
                            AutoCancel = false
                        }
                    };

                    await LocalNotificationCenter.Current.Show(request);
                    System.Diagnostics.Debug.WriteLine($"Уведомление запланировано для {Name} на {notificationTime.Value}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка планирования уведомления: {ex.Message}");
            }
        }

        /// <summary>
        /// Отменяет уведомление для привычки
        /// </summary>
        public void CancelNotification()
        {
            try
            {
                LocalNotificationCenter.Current.Cancel(NotificationId);
                System.Diagnostics.Debug.WriteLine($"Уведомление отменено для привычки: {Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка отмены уведомления: {ex.Message}");
            }
        }

        /// <summary>
        /// Вычисляет следующее время для уведомления
        /// </summary>
        private DateTime? CalculateNextNotificationTime()
        {
            try
            {
                var now = DateTime.Now;
                var todayNotificationTime = DateTime.Today.Add(ReminderTime);

                // Если время сегодня уже прошло, планируем на следующий подходящий день
                if (now > todayNotificationTime)
                {
                    return GetNextValidDate(DateTime.Today.AddDays(1));
                }

                // Если время сегодня еще не наступило, проверяем подходит ли сегодняшний день
                if (IsDateValidForFrequency(DateTime.Today))
                {
                    return todayNotificationTime;
                }

                // Ищем следующий подходящий день
                return GetNextValidDate(DateTime.Today.AddDays(1));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка вычисления времени уведомления: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Находит следующую подходящую дату для уведомления
        /// </summary>
        private DateTime? GetNextValidDate(DateTime startDate)
        {
            try
            {
                for (int i = 0; i < 365; i++) // Ограничим поиск годом
                {
                    var checkDate = startDate.AddDays(i);
                    if (IsDateValidForFrequency(checkDate))
                    {
                        return checkDate.Add(ReminderTime);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка поиска следующей даты: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Проверяет, подходит ли дата для частоты привычки
        /// </summary>
        private bool IsDateValidForFrequency(DateTime date)
        {
            try
            {
                return Frequency switch
                {
                    "Ежедневно" => true,
                    "Каждые 2 дня" => LastCompletedDate == DateTime.MinValue ||
                                     (date - LastCompletedDate.Date).TotalDays >= 2,
                    "Раз в неделю" => LastCompletedDate == DateTime.MinValue ||
                                     (date - LastCompletedDate.Date).TotalDays >= 7,
                    "Раз в месяц" => LastCompletedDate == DateTime.MinValue ||
                                    (date - LastCompletedDate.Date).TotalDays >= 30,
                    "Только по будням" => date.DayOfWeek != DayOfWeek.Saturday &&
                                        date.DayOfWeek != DayOfWeek.Sunday,
                    "Только по выходным" => date.DayOfWeek == DayOfWeek.Saturday ||
                                          date.DayOfWeek == DayOfWeek.Sunday,
                    _ => true
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка проверки даты: {ex.Message}");
                return true;
            }
        }

        /// <summary>
        /// Перепланирует все уведомления для привычки (при изменении времени или частоты)
        /// </summary>
        public async Task RescheduleNotification()
        {
            try
            {
                CancelNotification();
                await ScheduleNotification();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка перепланирования уведомления: {ex.Message}");
            }
        }

        /// <summary>
        /// Проверяет, активно ли уведомление для привычки
        /// </summary>
        public async Task<bool> IsNotificationScheduled()
        {
            try
            {
                var pendingNotifications = await LocalNotificationCenter.Current.GetPendingNotificationList();
                return pendingNotifications.Any(n => n.NotificationId == NotificationId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка проверки уведомления: {ex.Message}");
                return false;
            }
        }
    }
}