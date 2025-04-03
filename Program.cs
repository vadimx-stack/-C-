using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpApp
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            Console.WriteLine("Система управления задачами");
            Console.WriteLine("==========================");
            
            var taskManager = new TaskManager();
            var database = DatabaseFactory.CreateDatabase("tasks.json");
            var apiService = new ApiService("https://api.example.com/");
            
            try
            {
                List<TaskItem> savedTasks = database.LoadTasks();
                foreach (var task in savedTasks)
                {
                    taskManager.AddTask(task);
                }
                
                Console.WriteLine($"Загружено {savedTasks.Count} задач из локального хранилища.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
            }
            
            Console.WriteLine("\nДобавление новых задач:");
            
            taskManager.AddTask(new TaskItem { Id = 1, Title = "Разработка новой функции", DueDate = DateTime.Now.AddDays(7), IsCompleted = false });
            taskManager.AddTask(new TaskItem { Id = 2, Title = "Тестирование приложения", DueDate = DateTime.Now.AddDays(3), IsCompleted = false });
            taskManager.AddTask(new TaskItem { Id = 3, Title = "Обновление документации", DueDate = DateTime.Now.AddDays(1), IsCompleted = true });
            
            Console.WriteLine("\nСписок всех задач:");
            DisplayTasks(taskManager.GetAllTasks());
            
            Console.WriteLine("\nНевыполненные задачи:");
            DisplayTasks(taskManager.GetIncompleteTasks());
            
            Console.WriteLine("\nСрочные задачи (в ближайшие 5 дней):");
            DisplayTasks(taskManager.GetUrgentTasks(5));
            
            Console.WriteLine("\nВыполнение задачи #2");
            taskManager.CompleteTask(2);
            
            Console.WriteLine("\nОбновленный список задач:");
            DisplayTasks(taskManager.GetAllTasks());
            
            try
            {
                database.SaveTasks(taskManager.GetAllTasks());
                Console.WriteLine("\nЗадачи сохранены в локальное хранилище.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка при сохранении задач: {ex.Message}");
            }
            
            Console.WriteLine("\nПолучение задач из API...");
            try
            {
                List<TaskDto> remoteTasks = await apiService.GetTasksAsync();
                Console.WriteLine($"Получено {remoteTasks.Count} задач из API");
                
                if (remoteTasks.Count > 0)
                {
                    Console.WriteLine("\nЗадачи из API:");
                    foreach (var task in remoteTasks)
                    {
                        Console.WriteLine($"ID: {task.Id}, Название: {task.Title}, " +
                                          $"Описание: {task.Description}, " +
                                          $"Приоритет: {task.Priority}, " +
                                          $"Срок: {task.DueDate.ToShortDateString()}, " +
                                          $"Статус: {(task.IsCompleted ? "Выполнено" : "Не выполнено")}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении задач из API: {ex.Message}");
            }
            
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
        
        static void DisplayTasks(List<TaskItem> tasks)
        {
            if (tasks.Count == 0)
            {
                Console.WriteLine("Задач не найдено.");
                return;
            }
            
            foreach (var task in tasks)
            {
                Console.WriteLine($"ID: {task.Id}, Название: {task.Title}, " +
                                  $"Срок: {task.DueDate.ToShortDateString()}, " +
                                  $"Статус: {(task.IsCompleted ? "Выполнено" : "Не выполнено")}");
            }
        }
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }

    class TaskManager
    {
        private List<TaskItem> tasks = new List<TaskItem>();
        
        public void AddTask(TaskItem task)
        {
            tasks.Add(task);
        }
        
        public void CompleteTask(int taskId)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.IsCompleted = true;
            }
        }
        
        public List<TaskItem> GetAllTasks()
        {
            return tasks;
        }
        
        public List<TaskItem> GetIncompleteTasks()
        {
            return tasks.Where(t => !t.IsCompleted).ToList();
        }
        
        public List<TaskItem> GetUrgentTasks(int days)
        {
            DateTime threshold = DateTime.Now.AddDays(days);
            return tasks.Where(t => !t.IsCompleted && t.DueDate <= threshold).ToList();
        }
    }
}
