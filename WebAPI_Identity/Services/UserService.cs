using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI_Identity.Data;
using WebAPI_Identity.Models;

namespace WebAPI_Identity.Services
{
    public interface IUserService
    {
        MyUser Authenticate(string username, string password);
        IEnumerable<MyUser> GetAll();
        MyUser GetById(string userId);
        IdentityResult Create(MyUser user, string password);
        void Update(MyUser user, string currentPass = null, string password = null);
        void Delete(string id);
    }

    public class UserService : IUserService
    {
        private readonly SignInManager<MyUser> _signInManager;
        private readonly UserManager<MyUser> _userManager;

        public UserService(ApplicationDbContext context, UserManager<MyUser> userManager, SignInManager<MyUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public MyUser Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _userManager.FindByNameAsync(username).Result;

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            var signInResult = _signInManager.CheckPasswordSignInAsync(user, password, false).Result;
            if(!signInResult.Succeeded)
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<MyUser> GetAll()
        {
            return _userManager.Users;
        }

        public MyUser GetById(string userId)
        {
            return _userManager.FindByIdAsync(userId).Result;
        }

        public IdentityResult Create(MyUser user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception("Password is required");

            if (_userManager.Users.Any(x => x.UserName == user.UserName))
                throw new Exception("Username \"" + user.UserName + "\" is already taken");

            var result = _userManager.CreateAsync(user, password);
            return result.Result;
            
        }


        public void Update(MyUser userParam, string currentPass = null, string password = null)
        {

            var user = _userManager.FindByIdAsync(userParam.UserName).Result;

            if (user == null)
                throw new Exception("User not found");

            if (userParam.UserName != user.UserName)
            {
                // username has changed so check if the new username is already taken
                if (_userManager.Users.Any(x => x.UserName == userParam.UserName))
                    throw new Exception("Username " + userParam.UserName + " is already taken");
            }

            // update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.UserName = userParam.UserName;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(currentPass))
            {
                _userManager.ChangePasswordAsync(user, currentPass, password);
            }

            _userManager.UpdateAsync(user);
        }

        public void Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                _userManager.DeleteAsync(user);
            }

        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
