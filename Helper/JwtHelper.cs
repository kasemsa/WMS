using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using WarehouseManagementSystem.DataBase;
using WarehouseManagementSystem.Infrastructure.JwtService;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Helper
{
    public static class JwtHelper
    {
        public static bool ValidateJwtToken(string token)
        {
            try
            {
                var stream = token.Replace("Bearer ", string.Empty);
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = jsonToken as JwtSecurityToken;
                var userId = int.Parse(tokenS!.Claims.First(claim => claim.Type == "Id").Value);

                using (WarehouseDbContext dbContext = new WarehouseDbContext())
                {
                    User? user = dbContext.Users.FirstOrDefault(u => u.Id == userId);

                    if (user == null)
                    {
                        return false;
                    }
                    else
                    {
                        UserToken userToken = dbContext.UserToken.Where(u => u.UserId == userId).FirstOrDefault();
                        if (userToken != null)
                        {
                            if (userToken.Token != token)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                            return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool CheckUserPermissions(string token, HttpContext httpContext)
        {
            try
            {
                var stream = token.Replace("Bearer ", string.Empty);
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = jsonToken as JwtSecurityToken;
                var userId = int.Parse(tokenS!.Claims.First(claim => claim.Type == "Id").Value);

                using (WarehouseDbContext dbContext = new WarehouseDbContext())
                {
                    User user = dbContext.Users.First(u => u.Id == userId);

                    var RoleID = dbContext.UserRoles.Where(u => u.UserId == user.Id).Select(u => u.RoleId).First();

                    var RolePermissions = dbContext.RolePermissions.Where(r => r.RoleId == RoleID).Select(r => r.Permission).Select(p => p.PermissionName).ToList();

                    var UserPermissions = dbContext.UserPermissions.Where(u => u.UserId == user.Id).Select(u => u.Permission).Select(p => p.PermissionName).ToList();

                    var permissions = new List<string>();

                    permissions.AddRange(RolePermissions);
                    permissions.AddRange(UserPermissions);


                    if (permissions.Contains("All"))
                    {
                        return true;
                    }

                    if (httpContext.Request.Path.Value is not null)
                    {
                        
                        var x = permissions.Where(p => permissions.Contains(httpContext.Request.Path.Value.Split("/")[3])).Count();
                       
                        if (x == 0)
                        {
                            return false;
                        }
                        else return true;
                    }
                    else return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
