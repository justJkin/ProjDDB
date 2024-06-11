using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using financialApp.Models;
using financialApp.Repositories;

namespace financialApp.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        public AuthService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User Login(string username, string password)
        {
            var user = _userRepository.GetUserByUsernameAndPassword(username, password);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            return user;
        }
    }
}
