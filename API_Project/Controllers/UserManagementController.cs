using API_Project.Models;
using API_Project.Services;
using API_Project.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Project.Controllers
{
    [Authorize(Roles = ("Admin"))]
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserService userService;

        public UserManagementController(IUserService userService)
        {
            this.userService = userService;
        }
        public async Task<string> AddUser(RegisterModel model)
        {
            return await userService.RegisterAsync(model);
        }

        public async Task<string> ChangePassword(ChangePasswordModel model)
        {
            return await userService.ChangePassword(model);
        }

        [HttpDelete("delete/{id}")]
        public async Task<string> DeleteUser(string id)
        {
            return await userService.RemoveUser(id);
        }

        [HttpGet("users")]
        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            return await userService.GetAllUsers();
        }

        [HttpGet("t-user/{id}")]
        public async Task<ApplicationUser> RecoverUser(string id)
        {
            return await userService.RecoverUser(id);
        }

        [HttpGet("t-users")]
        public async Task<IEnumerable<ApplicationUser>> AllTrashedUsers()
        {
            return await userService.TrashedUserList();
        }
    }
}
