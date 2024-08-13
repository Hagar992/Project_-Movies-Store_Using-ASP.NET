using Microsoft.AspNetCore.Identity;
using full_Project.Repositories.Abstract;
using System.Security.Claims;
using full_Project.Models.Domain;
using full_Project.Models.DTO;

namespace full_Project.Repositories.Implementation
{
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserAuthenticationService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public async Task<Status> RegisterAsync(RegistrationModel model)
        {
            var status = new Status();
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "User already exists";
                return status;
            }

            var user = new ApplicationUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Name = model.Name,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return status;
            }

            if (!await roleManager.RoleExistsAsync(model.Role))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(model.Role));
                if (!roleResult.Succeeded)
                {
                    status.StatusCode = 0;
                    status.Message = "Role creation failed: " + string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return status;
                }
            }

            if (await roleManager.RoleExistsAsync(model.Role))
            {
                var addToRoleResult = await userManager.AddToRoleAsync(user, model.Role);
                if (!addToRoleResult.Succeeded)
                {
                    status.StatusCode = 0;
                    status.Message = "Adding to role failed: " + string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                    return status;
                }
            }

            status.StatusCode = 1;
            status.Message = "You have registered successfully";
            return status;
        }

        public async Task<Status> LoginAsync(LoginModel model)
        {
            var status = new Status();
            var user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return status;
            }

            var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, true, lockoutOnFailure: true);
            if (signInResult.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                };

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                status.StatusCode = 1;
                status.Message = "Logged in successfully";
            }
            else if (signInResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User is locked out";
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Invalid login attempt";
            }

            return status;
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        // Uncomment and update the ChangePasswordAsync method if needed
        //public async Task<Status> ChangePasswordAsync(ChangePasswordModel model, string username)
        //{
        //    var status = new Status();
        //    var user = await userManager.FindByNameAsync(username);
        //    if (user == null)
        //    {
        //        status.Message = "User does not exist";
        //        status.StatusCode = 0;
        //        return status;
        //    }
        //    var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        //    if (result.Succeeded)
        //    {
        //        status.Message = "Password has been updated successfully";
        //        status.StatusCode = 1;
        //    }
        //    else
        //    {
        //        status.Message = "An error occurred: " + string.Join(", ", result.Errors.Select(e => e.Description));
        //        status.StatusCode = 0;
        //    }
        //    return status;
        //}
    }
}
