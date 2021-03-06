using API_Project.Constants;
using API_Project.Data;
using API_Project.Models;
using API_Project.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API_Project.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext context;
        private readonly JWT jWT;

        public UserService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JWT> jWT,
            ApplicationDbContext context
            )
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
            this.jWT = jWT.Value;
        }

        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return $"No account registered with this email";
            }
            if (await userManager.CheckPasswordAsync(user, model.Password))
            {
                var roleExists = Enum.GetNames(typeof(Authorization.Roles)).Any(x => x.ToLower() == model.Role.ToLower());

                if (roleExists)
                {
                    var validRole = Enum.GetValues(typeof(Authorization.Roles))
                        .Cast<Authorization.Roles>().Where(x => x.ToString().ToLower() == model.Role.ToLower()).FirstOrDefault();
                    await userManager.AddToRoleAsync(user, validRole.ToString());
                    return $"Added {model.Role} to user {model.Email}";
                }

                return $"Role {model.Role} not found";
            }
            return $"Incorrect credential for user {user.Email}";
        }

        public async Task<string> ChangePassword(ChangePasswordModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return $"User not foun by email {model.Email}";
            var resetPassResult = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!resetPassResult.Succeeded)
            {
                return "Password change success";
            }
            return "password change failed!!";
        }

        public async Task<string> ChangeEmail(ChangeEmailModel model)
        {
            var user = await context.Users.FindAsync(model.Id);

            if (user == null)
            {
                return "User not found";
            }

            user.UserName = model.Email;
            user.Email = model.Email;
            await userManager.UpdateAsync(user);
            await context.SaveChangesAsync();

            return "Email updated successfully";
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            return await context.Users.Where(user=>user.IsDeleted !=true).ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> TrashedUserList()
        {
            return await context.Users.Where(user => user.IsDeleted == true).ToListAsync();
        }



        public async Task<ApplicationUser> GetById(string id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<string> RemoveUser(string id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return "User not found";
            }
            //context.Users.Remove(user);
            user.IsDeleted = true;
            context.Update(user);
            await context.SaveChangesAsync();
            return "User deleted";
        }

        public async Task<ApplicationUser> RecoverUser(string id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            user.IsDeleted = false;
            context.Update(user);
            await context.SaveChangesAsync();

            return await GetById(user.Id);
        }

        public async Task<AuthenticationModel> LoginAsync(LoginModel model)
        {
            var authenticationModel = new AuthenticationModel();
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"No account found with {model.Email}";
                return authenticationModel;
            }
            if (await userManager.CheckPasswordAsync(user, model.Password))
            {
                authenticationModel.IsAuthenticated = true;
                authenticationModel.Message = "Welcome to our application!!";
                JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
                authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authenticationModel.Email = user.Email;
                authenticationModel.UserName = user.UserName;
                var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);
                authenticationModel.Roles = rolesList.ToList();

                if (user.RefreshTokens.Any(a => a.IsActive))
                {
                    var activeRefreshToken = user.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();
                    authenticationModel.RefreshToken = activeRefreshToken.Token;
                    authenticationModel.RefreshTokenExpiration = activeRefreshToken.Expires;
                }
                else
                {
                    var refreshToken = CreateRefreshToken();
                    authenticationModel.RefreshToken = refreshToken.Token;
                    authenticationModel.RefreshTokenExpiration = refreshToken.Expires;
                    user.RefreshTokens.Add(refreshToken);
                    context.Update(user);
                    context.SaveChanges();
                }

                return authenticationModel;
            }

            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"Incorrect credentials for user {user.Email}";
            return authenticationModel;
        }

        public async Task<AuthenticationModel> RefreshTokenAsync(string token)
        {
            var authenticationModel = new AuthenticationModel();
            var user = context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"Token didn't mathc any users!!";
                return authenticationModel;
            }
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"Token not active";
                return authenticationModel;
            }

            //revoked/cancel current refresh token
            refreshToken.Revoked = DateTime.UtcNow;

            // generate new refreshtoken and save to db
            var newRefreshToken = CreateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            context.Update(user);
            await context.SaveChangesAsync();

            //create new authentication model
            authenticationModel.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
            authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authenticationModel.Email = user.Email;
            authenticationModel.UserName = user.UserName;
            var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);
            authenticationModel.Roles = rolesList.ToList();
            authenticationModel.RefreshToken = newRefreshToken.Token;
            authenticationModel.RefreshTokenExpiration = newRefreshToken.Expires;

            return authenticationModel;
        }

        public async Task<string> RegisterAsync(RegisterModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var userWithSameEmail = await userManager.FindByEmailAsync(model.Email);
            if (userWithSameEmail == null)
            {
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    
                    if (model.Email == Authorization.email)
                    {
                        bool x = await roleManager.RoleExistsAsync("Admin");
                        if (!x)
                        {
                            var role = new IdentityRole();
                            role.Name = "Admin";
                            await roleManager.CreateAsync(role);
                        }
                        await userManager.AddToRoleAsync(user, Authorization.adminRoles.ToString());
                    }
                    else
                    {
                        bool x = await roleManager.RoleExistsAsync("User");
                        if (!x)
                        {
                            var role = new IdentityRole();
                            role.Name = "User";
                            await roleManager.CreateAsync(role);
                        }
                        await userManager.AddToRoleAsync(user, Authorization.role.ToString());
                    }
                    
                }
                return $"User registration with username {user.UserName}";
            }

            return $"Email {user.Email} is already registered!!";

        }

        public bool RevokeToken(string token)
        {
            var user = context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
            {
                return false;
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive)
            {
                return false;
            }

            refreshToken.Revoked = DateTime.UtcNow;
            context.Update(user);
            context.SaveChangesAsync();

            return true;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid",user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWT.Key));
            var sigingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken
            (
                issuer: jWT.Issuer,
                audience: jWT.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jWT.DurationInMinutes),
                signingCredentials: sigingCredentials
            );

            return jwtSecurityToken;

        }

        private RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var generator = new RNGCryptoServiceProvider())
            {
                generator.GetBytes(randomNumber);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expires = DateTime.UtcNow.AddDays(10),
                    CreatedDate = DateTime.UtcNow
                };
            }
        }




    }
}
