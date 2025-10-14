using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Habit_Tracker
{
    public class HabitDatabase
    {
        private SQLiteAsyncConnection _database;

        public HabitDatabase()
        {
            InitializeDatabase();
        }

        private async 
        Task
InitializeDatabase()
        {
            if (_database != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "habits.db3");
            _database = new SQLiteAsyncConnection(databasePath);
            await _database.CreateTableAsync<Habit>();
        }

        // Получить все привычки
        public async Task<List<Habit>> GetHabitsAsync()
        {
            await InitializeDatabase();
            return await _database.Table<Habit>().OrderByDescending(x => x.CreationDate).ToListAsync();
        }

        // Сохранить привычку
        public async Task<int> SaveHabitAsync(Habit habit)
        {
            await InitializeDatabase();
            if (habit.Id != 0)
            {
                return await _database.UpdateAsync(habit);
            }
            else
            {
                return await _database.InsertAsync(habit);
            }
        }

        // Удалить привычку
        public async Task<int> DeleteHabitAsync(Habit habit)
        {
            await InitializeDatabase();
            return await _database.DeleteAsync(habit);
        }

        // Получить привычку по ID
        public async Task<Habit> GetHabitAsync(int id)
        {
            await InitializeDatabase();
            return await _database.Table<Habit>()
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
        }
        public async Task<int> UpdateHabitAsync(Habit habit)
        {
            await InitializeDatabase();
            return await _database.UpdateAsync(habit);
        }
    }
}