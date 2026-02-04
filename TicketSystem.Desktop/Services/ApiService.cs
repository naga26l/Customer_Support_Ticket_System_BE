using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using TicketSystem.Desktop.Models;

namespace TicketSystem.Desktop.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5200/api";

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            var loginRequest = new { Username = username, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{BaseUrl}/auth/login", content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<User>(json, options);
            }
            return null;
        }

        public async Task<System.Collections.Generic.List<Ticket>> GetTicketsAsync(int? userId)
        {
            var url = userId.HasValue ? $"{BaseUrl}/tickets?userId={userId}" : $"{BaseUrl}/tickets";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Ticket>>(json, options);
            }
            return new List<Ticket>();
        }

        public async Task<Ticket> CreateTicketAsync(object ticketRequest)
        {
            var content = new StringContent(JsonSerializer.Serialize(ticketRequest), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/tickets", content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<Ticket>(json, options);
            }
            return null;
        }

        public async Task<bool> AssignTicketAsync(int ticketId, int adminId)
        {
            var content = new StringContent(JsonSerializer.Serialize(new { TicketId = ticketId, AdminId = adminId }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/tickets/assign", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStatusAsync(int ticketId, string status, int userId)
        {
            var content = new StringContent(JsonSerializer.Serialize(new { TicketId = ticketId, Status = status, UserId = userId }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{BaseUrl}/tickets/status", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<TicketHistoryItem>> GetTicketHistoryAsync(int ticketId)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/tickets/{ticketId}/history");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<TicketHistoryItem>>(json, options);
            }
            return new List<TicketHistoryItem>();
        }

        public async Task<bool> AddCommentAsync(int ticketId, string comment, int userId)
        {
             var content = new StringContent(JsonSerializer.Serialize(new { TicketId = ticketId, Comment = comment, UserId = userId }), Encoding.UTF8, "application/json");
             var response = await _httpClient.PostAsync($"{BaseUrl}/tickets/comment", content);
             return response.IsSuccessStatusCode;
        }

        public async Task<List<User>> GetAdminsAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/users/admins");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<User>>(json, options);
            }
            return new List<User>();
        }
    }
}
