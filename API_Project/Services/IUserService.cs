using API_Project.Models;
using API_Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace API_Project.Services
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterModel model);
        Task<AuthenticationModel> LoginAsync(LoginModel model);
        Task<string> AddRoleAsync(AddRoleModel model);
        Task<AuthenticationModel> RefreshTokenAsync(string token);
        Task<ApplicationUser> GetById(string id);

        Task<IEnumerable<ApplicationUser>> GetAllUsers();
        Task<string> ChangePassword(ChangePasswordModel model);
        Task<string> ChangeEmail(ChangeEmailModel model);

        Task<string> RemoveUser(string id);

        Task<ApplicationUser> RecoverUser(string id);
        Task<IEnumerable<ApplicationUser>> TrashedUserList();


        bool RevokeToken(string token);
    }
}
