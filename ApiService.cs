using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSharpApp
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiService(string baseUrl)
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<List<TaskDto>> GetTasksAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("api/tasks");
                response.EnsureSuccessStatusCode();
                
                string content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<TaskDto>>(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении задач: {ex.Message}");
                return new List<TaskDto>();
            }
        }

        public async Task<bool> CreateTaskAsync(TaskDto task)
        {
            try
            {
                string json = JsonSerializer.Serialize(task);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = await _httpClient.PostAsync("api/tasks", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании задачи: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateTaskAsync(TaskDto task)
        {
            try
            {
                string json = JsonSerializer.Serialize(task);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = await _httpClient.PutAsync($"api/tasks/{task.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении задачи: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/tasks/{taskId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении задачи: {ex.Message}");
                return false;
            }
        }
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public int Priority { get; set; }
    }
} 