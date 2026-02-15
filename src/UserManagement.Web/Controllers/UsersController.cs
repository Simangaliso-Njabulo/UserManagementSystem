using Microsoft.AspNetCore.Mvc;
using UserManagement.Core.DTOs;
using UserManagement.Web.Services;

namespace UserManagement.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ApiService apiService, ILogger<UsersController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _apiService.GetAllUsersAsync();
            if (users == null)
            {
                TempData["Error"] = "Unable to load users. Please ensure the API is running.";
                return View(new List<UserDto>());
            }
            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _apiService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = $"User with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Groups = await _apiService.GetAllGroupsAsync();
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Groups = await _apiService.GetAllGroupsAsync();
                return View(createUserDto);
            }

            var user = await _apiService.CreateUserAsync(createUserDto);
            if (user == null)
            {
                TempData["Error"] = "Error creating user. Please try again.";
                ViewBag.Groups = await _apiService.GetAllGroupsAsync();
                return View(createUserDto);
            }

            TempData["Success"] = $"User '{user.FirstName} {user.LastName}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _apiService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = $"User with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }

            var updateDto = new UpdateUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                GroupIds = user.Groups.Select(g => g.Id).ToList()
            };

            ViewBag.Groups = await _apiService.GetAllGroupsAsync();
            ViewBag.UserId = id;
            return View(updateDto);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Groups = await _apiService.GetAllGroupsAsync();
                ViewBag.UserId = id;
                return View(updateUserDto);
            }

            var user = await _apiService.UpdateUserAsync(id, updateUserDto);
            if (user == null)
            {
                TempData["Error"] = "Error updating user. Please try again.";
                ViewBag.Groups = await _apiService.GetAllGroupsAsync();
                ViewBag.UserId = id;
                return View(updateUserDto);
            }

            TempData["Success"] = $"User '{user.FirstName} {user.LastName}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _apiService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = $"User with ID {id} not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _apiService.DeleteUserAsync(id);
            if (!result)
            {
                TempData["Error"] = "Error deleting user. Please try again.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}