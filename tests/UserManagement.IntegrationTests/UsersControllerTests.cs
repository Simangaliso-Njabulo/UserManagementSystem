using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using UserManagement.Core.DTOs;
using UserManagement.IntegrationTests.TestFixtures;

namespace UserManagement.IntegrationTests.Controllers
{
    public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public UsersControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnOkWithUsers()
        {
            // Act
            var response = await _client.GetAsync("/api/users");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
            users.Should().NotBeNull();
            users.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task GetUserById_WhenUserExists_ShouldReturnOkWithUser()
        {
            // Arrange - Using seeded user with ID 1
            var userId = 1;

            // Act
            var response = await _client.GetAsync($"/api/users/{userId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var user = await response.Content.ReadFromJsonAsync<UserDto>();
            user.Should().NotBeNull();
            user!.Id.Should().Be(userId);
            user.Email.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetUserById_WhenUserDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistentUserId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/users/{nonExistentUserId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateUser_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                FirstName = "Integration",
                LastName = "Test",
                Email = "integration.test@example.com",
                GroupIds = new List<int> { 1 }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", createDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdUser = await response.Content.ReadFromJsonAsync<UserDto>();
            createdUser.Should().NotBeNull();
            createdUser!.FirstName.Should().Be("Integration");
            createdUser.LastName.Should().Be("Test");
            createdUser.Email.Should().Be("integration.test@example.com");
            createdUser.Groups.Should().HaveCount(1);

            // Verify Location header
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateUser_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange - Missing required fields
            var createDto = new CreateUserDto
            {
                FirstName = "",
                LastName = "",
                Email = "invalid-email",
                GroupIds = new List<int>()
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/users", createDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateUser_WithValidData_ShouldReturnOk()
        {
            // Arrange - First create a user to update
            var createDto = new CreateUserDto
            {
                FirstName = "ToUpdate",
                LastName = "User",
                Email = "toupdate@example.com",
                GroupIds = new List<int> { 1 }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/users", createDto);
            var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();

            var updateDto = new UpdateUserDto
            {
                FirstName = "Updated",
                LastName = "User",
                Email = "updated@example.com",
                GroupIds = new List<int> { 2 }
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/users/{createdUser!.Id}", updateDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedUser = await response.Content.ReadFromJsonAsync<UserDto>();
            updatedUser.Should().NotBeNull();
            updatedUser!.FirstName.Should().Be("Updated");
            updatedUser.Email.Should().Be("updated@example.com");
            updatedUser.Groups.Should().HaveCount(1);
            updatedUser.Groups.First().Id.Should().Be(2);
        }

        [Fact]
        public async Task UpdateUser_WhenUserDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var updateDto = new UpdateUserDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                GroupIds = new List<int>()
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/users/9999", updateDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteUser_WhenUserExists_ShouldReturnNoContent()
        {
            // Arrange - First create a user to delete
            var createDto = new CreateUserDto
            {
                FirstName = "ToDelete",
                LastName = "User",
                Email = "todelete@example.com",
                GroupIds = new List<int>()
            };

            var createResponse = await _client.PostAsJsonAsync("/api/users", createDto);
            var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();

            // Act
            var response = await _client.DeleteAsync($"/api/users/{createdUser!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify user is actually deleted
            var getResponse = await _client.GetAsync($"/api/users/{createdUser.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteUser_WhenUserDoesNotExist_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/users/9999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetTotalUserCount_ShouldReturnCount()
        {
            // Act
            var response = await _client.GetAsync("/api/users/count");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var count = await response.Content.ReadFromJsonAsync<int>();
            count.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetUserCountByGroup_ShouldReturnCounts()
        {
            // Act
            var response = await _client.GetAsync("/api/users/count-by-group");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var counts = await response.Content.ReadFromJsonAsync<List<UserCountByGroupDto>>();
            counts.Should().NotBeNull();
            counts.Should().HaveCountGreaterThan(0);

            foreach (var count in counts!)
            {
                count.GroupId.Should().BeGreaterThan(0);
                count.GroupName.Should().NotBeNullOrEmpty();
                count.UserCount.Should().BeGreaterThanOrEqualTo(0);
            }
        }

        [Fact]
        public async Task CompleteWorkflow_CreateReadUpdateDelete_ShouldWork()
        {
            // 1. Create
            var createDto = new CreateUserDto
            {
                FirstName = "Workflow",
                LastName = "Test",
                Email = "workflow@example.com",
                GroupIds = new List<int> { 1, 2 }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/users", createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();
            createdUser.Should().NotBeNull();
            var userId = createdUser!.Id;

            // 2. Read
            var getResponse = await _client.GetAsync($"/api/users/{userId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var fetchedUser = await getResponse.Content.ReadFromJsonAsync<UserDto>();
            fetchedUser!.Email.Should().Be("workflow@example.com");
            fetchedUser.Groups.Should().HaveCount(2);

            // 3. Update
            var updateDto = new UpdateUserDto
            {
                FirstName = "WorkflowUpdated",
                LastName = "TestUpdated",
                Email = "workflow.updated@example.com",
                GroupIds = new List<int> { 3 }
            };

            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{userId}", updateDto);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedUser = await updateResponse.Content.ReadFromJsonAsync<UserDto>();
            updatedUser!.FirstName.Should().Be("WorkflowUpdated");
            updatedUser.Groups.Should().HaveCount(1);
            updatedUser.Groups.First().Id.Should().Be(3);

            // 4. Delete
            var deleteResponse = await _client.DeleteAsync($"/api/users/{userId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // 5. Verify deletion
            var verifyResponse = await _client.GetAsync($"/api/users/{userId}");
            verifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}