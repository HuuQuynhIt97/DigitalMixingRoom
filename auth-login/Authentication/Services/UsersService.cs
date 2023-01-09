using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authentication.Data;
using Authentication.Dtos;
using Authentication.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Authentication.Services
{
    public interface IUserService
    {
        bool IsAnExistingUser(string userName);
        bool IsValidUserCredentials(string userName, string password);
        string GetUserRole(string userName);
        Task<User> GetUser(string userName, string password);
        Task<List<User>> GetAll();
        Task<PagedList<UserDto>> GetAllPaging(int page, int pageSize, string keyword);
        Task<PagedList<UserDto>> GetAllPaging(int systemCode, int page, int pageSize, string keyword);
    }

    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly DataContext _context;
        // inject your database here for user validation

        public UserService(ILogger<UserService> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmaic = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmaic.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }

                return true;
            }
        }
        public bool IsValidUserCredentials(string userName, string password)
        {
            _logger.LogInformation($"Validating user [{userName}]");
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var user = _context.Users.Where(x => x.IsShow == true).FirstOrDefault(x => x.EmployeeID.ToLower() == userName.ToLower());
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return false;

            return true;
        }

        public bool IsAnExistingUser(string userName)
        {
            return _context.Users.Where(x => x.IsShow == true).Any(x => x.EmployeeID.ToLower() == userName.ToLower());
        }

        public string GetUserRole(string userName)
        {
            if (!IsAnExistingUser(userName))
            {
                return string.Empty;
            }
            var user = _context.Users.Where(x => x.IsShow == true).Include(x=> x.Role).FirstOrDefault(x => x.EmployeeID.ToLower() == userName.ToLower());
            if (user.Role.Name.ToLower() == "admin" || user.Role.ID == 1)
            {
                return UserRoles.Admin;
            }

            return UserRoles.BasicUser;
        }

        public async Task<User> GetUser(string userName, string password)
        {
            var user = await _context.Users.Where(x=>x.IsShow == true)
                .Include(x=>x.UserSystems)
                .Include(x=>x.Role).FirstOrDefaultAsync(x => x.EmployeeID.ToLower() == userName.ToLower());
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public async Task<PagedList<UserDto>> GetAllPaging(int page, int pageSize, string keyword)
        {
            var source = _context.Users.Include(x => x.UserSystems).Where(x => x.IsShow == true && x.Username != "admin").OrderByDescending(x => x.ID).Select(x => new UserDto { isLeader = x.isLeader, ID = x.ID, Username = x.Username, Email = x.Email, RoleName = x.Role.Name, RoleID = x.RoleID, EmployeeID = x.EmployeeID }).AsQueryable();
            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Username.Contains(keyword) || x.Email.Contains(keyword));
            }
            return await PagedList<UserDto>.CreateAsync(source, page, pageSize);
        }

        public async Task<PagedList<UserDto>> GetAllPaging(int systemCode, int page, int pageSize, string keyword)
        {
            var source = _context.Users.Include(x => x.UserSystems).Where(x => x.IsShow == true && x.Username != "admin" && x.UserSystems.Where(x => x.Status == true).Select(x => x.SystemID).Contains(systemCode)).OrderByDescending(x => x.ID).Select(x => new UserDto { isLeader = x.isLeader, ID = x.ID, Username = x.Username, Email = x.Email, RoleName = x.Role.Name, RoleID = x.RoleID, EmployeeID = x.EmployeeID }).AsQueryable();
            if (!keyword.IsNullOrEmpty())
            {
                source = source.Where(x => x.Username.Contains(keyword) || x.Email.Contains(keyword));
            }
            return await PagedList<UserDto>.CreateAsync(source, page, pageSize);
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.Where(x => x.IsShow == true && x.Username != "admin").ToListAsync();
        }
    }

    public static class UserRoles
    {
        public const string Admin = nameof(Admin);
        public const string BasicUser = nameof(BasicUser);
    }
}
