using financialApp.Models;
using System.Collections.Generic;

namespace financialApp.Interfaces
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAllUsers();
        User? GetUserById(int id);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);
        void Save();
        void Register(User user);
        User? GetUserByUsernameAndPassword(string username, string password);

        IEnumerable<Transaction> GetAllTransactions();
        IEnumerable<SavingGoal> GetAllSavingGoals();
        IEnumerable<Transaction> GetUserTransactions(int userId);
    }
}
