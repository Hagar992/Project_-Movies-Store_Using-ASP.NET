using full_Project.Models.DTO;
using full_Project.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace full_Project.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private readonly IUserAuthenticationService authService;

        public UserAuthenticationController(IUserAuthenticationService authService)
        {
            this.authService = authService;
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await authService.LoginAsync(model);
                if (result.StatusCode == 1)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["msg"] = "Could not log in.";
                    return RedirectToAction(nameof(Login));
                }
            }
            catch (FormatException ex)
            {
                // Log the exception or handle it appropriately
                TempData["msg"] = $"An error occurred during login: {ex.Message}";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                TempData["msg"] = $"An unexpected error occurred: {ex.Message}";
                return RedirectToAction(nameof(Login));
            }
        }

        public async Task<IActionResult> Logout()
        {
            await authService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
