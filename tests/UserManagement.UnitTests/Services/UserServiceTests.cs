using FluentAssertions;
using Moq;
using UserManagement.Api.Services;
using UserManagement.Core.DTOs;
using UserManagement.Core.Entities;
using UserManagement.Core.Interfaces;

namespace UserManagement.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepository;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockRepository = new Mock<IUserRepository>();
            _service = new UserService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    CreatedAt = DateTime.UtcNow,
                    UserGroups = new List<UserGroup>
                    {
                        new UserGroup
                        {
                            UserId = 1,
                            GroupId = 1,
                            Group = new Group { Id = 1, Name = "Admin", Description = "Admin group" }
                        }
                    }
                }
            };

            _mockRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _service.GetAllUsersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            var userDto = result.First();
            userDto.FirstName.Should().Be("John");
            userDto.LastName.Should().Be("Doe");
            userDto.Groups.Should().HaveCount(1);
            userDto.Groups.First().Name.Should().Be("Admin");
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                CreatedAt = DateTime.UtcNow,
                UserGroups = new List<UserGroup>()
            };

            _mockRepository.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.FirstName.Should().Be("Jane");
            result.Email.Should().Be("jane@example.com");
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetUserByIdAsync(999)).ReturnsAsync((User?)null);

            // Act
            var result = await _service.GetUserByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateAndReturnUser()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                GroupIds = new List<int> { 1, 2 }
            };

            var createdUser = new User
            {
                Id = 5,
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                CreatedAt = DateTime.UtcNow
            };

            var userWithGroups = new User
            {
                Id = 5,
                FirstName = "New",
                LastName = "User",
                Email = "new@example.com",
                CreatedAt = DateTime.UtcNow,
                UserGroups = new List<UserGroup>
                {
                    new UserGroup { UserId = 5, GroupId = 1, Group = new Group { Id = 1, Name = "Admin", Description = "" } },
                    new UserGroup { UserId = 5, GroupId = 2, Group = new Group { Id = 2, Name = "Level 1", Description = "" } }
                }
            };

            _mockRepository.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(createdUser);
            _mockRepository.Setup(r => r.GetUserByIdAsync(5)).ReturnsAsync(userWithGroups);

            // Act
            var result = await _service.CreateUserAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.FirstName.Should().Be("New");
            result.Email.Should().Be("new@example.com");
            result.Groups.Should().HaveCount(2);
            _mockRepository.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenUserDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var updateDto = new UpdateUserDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                GroupIds = new List<int>()
            };

            _mockRepository.Setup(r => r.GetUserByIdAsync(999)).ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _service.UpdateUserAsync(999, updateDto);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("User with ID 999 not found.");
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldCallRepository()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteUserAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteUserAsync(1);

            // Assert
            result.Should().BeTrue();
            _mockRepository.Verify(r => r.DeleteUserAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetTotalUserCountAsync_ShouldReturnCount()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetTotalUserCountAsync()).ReturnsAsync(5);

            // Act
            var result = await _service.GetTotalUserCountAsync();

            // Assert
            result.Should().Be(5);
        }

        [Fact]
        public async Task GetUserCountByGroupAsync_ShouldReturnCounts()
        {
            // Arrange
            var counts = new List<(int GroupId, string GroupName, int UserCount)>
            {
                (1, "Admin", 2),
                (2, "Level 1", 3),
                (3, "Level 2", 1)
            };

            _mockRepository.Setup(r => r.GetUserCountByGroupAsync()).ReturnsAsync(counts);

            // Act
            var result = await _service.GetUserCountByGroupAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            var resultList = result.ToList();
            resultList[0].GroupName.Should().Be("Admin");
            resultList[0].UserCount.Should().Be(2);
            resultList[1].GroupName.Should().Be("Level 1");
            resultList[1].UserCount.Should().Be(3);
        }
    }
}