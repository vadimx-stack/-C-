using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CSharpApp
{
    public class Database
    {
        private string _filePath;
        
        public Database(string filePath)
        {
            _filePath = filePath;
        }
        
        public void SaveTasks(List<TaskItem> tasks)
        {
            string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        
        public List<TaskItem> LoadTasks()
        {
            if (!File.Exists(_filePath))
            {
                return new List<TaskItem>();
            }
            
            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<TaskItem>>(json);
        }
    }
    
    public class DatabaseFactory
    {
        public static Database CreateDatabase(string fileName)
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CSharpApp");
                
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            
            string filePath = Path.Combine(appDataPath, fileName);
            return new Database(filePath);
        }
    }
} 