using UserManagement.Core.Entities;

namespace UserManagement.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<int> GetTotalUserCountAsync();
        Task<IEnumerable<(int GroupId, string GroupName, int UserCount)>> GetUserCountByGroupAsync();
        Task<bool> UserExistsAsync(int id);
    }
}