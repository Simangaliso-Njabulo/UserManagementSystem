using UserManagement.Core.DTOs;
using UserManagement.Core.Entities;
using UserManagement.Core.Interfaces;

namespace UserManagement.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                UserGroups = createUserDto.GroupIds.Select(groupId => new UserGroup
                {
                    GroupId = groupId
                }).ToList()
            };

            var createdUser = await _userRepository.CreateUserAsync(user);

            // Reload user with groups to return complete DTO
            var userWithGroups = await _userRepository.GetUserByIdAsync(createdUser.Id);
            return MapToDto(userWithGroups!);
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Email = updateUserDto.Email;

            // Update groups - remove old ones and add new ones
            user.UserGroups.Clear();
            user.UserGroups = updateUserDto.GroupIds.Select(groupId => new UserGroup
            {
                UserId = id,
                GroupId = groupId
            }).ToList();

            var updatedUser = await _userRepository.UpdateUserAsync(user);

            // Reload user with groups
            var userWithGroups = await _userRepository.GetUserByIdAsync(updatedUser.Id);
            return MapToDto(userWithGroups!);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<int> GetTotalUserCountAsync()
        {
            return await _userRepository.GetTotalUserCountAsync();
        }

        public async Task<IEnumerable<UserCountByGroupDto>> GetUserCountByGroupAsync()
        {
            var counts = await _userRepository.GetUserCountByGroupAsync();
            return counts.Select(c => new UserCountByGroupDto
            {
                GroupId = c.GroupId,
                GroupName = c.GroupName,
                UserCount = c.UserCount
            });
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Groups = user.UserGroups.Select(ug => new GroupDto
                {
                    Id = ug.Group.Id,
                    Name = ug.Group.Name,
                    Description = ug.Group.Description
                }).ToList()
            };
        }
    }
}