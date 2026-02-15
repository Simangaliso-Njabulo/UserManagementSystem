using System.Text.Json;
using UserManagement.Core.DTOs;

namespace UserManagement.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<IEnumerable<UserDto>?> GetAllUsersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<UserDto>>("api/users", _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users from API");
                return null;
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserDto>($"api/users/{id}", _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {UserId} from API", id);
                return null;
            }
        }

        public async Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/users", createUserDto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user via API");
                return null;
            }
        }

        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/users/{id}", updateUserDto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId} via API", id);
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/users/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId} via API", id);
                return false;
            }
        }

        public async Task<IEnumerable<GroupDto>?> GetAllGroupsAsync()
        {
            // For the UI, we'll hardcode the groups since we don't have a groups endpoint
            // In a real app, you'd create a GroupsController in the API
            return new List<GroupDto>
            {
                new GroupDto { Id = 1, Name = "Admin", Description = "Administrator group" },
                new GroupDto { Id = 2, Name = "Level 1", Description = "Basic user access" },
                new GroupDto { Id = 3, Name = "Level 2", Description = "Intermediate user access" }
            };
        }
    }
}