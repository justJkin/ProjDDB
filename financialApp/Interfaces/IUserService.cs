using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IUserService
{ 
    //nig
    IEnumerable<User> GetAllUsers();
    User GetUserById(int id);
    void CreateUser(User user);
    void UpdateUser(User user);
    void DeleteUser(int id);
    bool AuthenticateUser(string username, string password);
    string GetUserRole(string username);
    bool RegisterUser(string username, string email, string password);
    decimal GetTotalBalance(int userId);
    decimal GetTotalExpenses(int userId);
    decimal GetTotalSavings(int userId);
    string GetUsername(int userId);
    string GetEmail(int userId);
    int GetUserId(string username);

    IEnumerable<Transaction> GetUserTransactions(int userId);
    void AddTransaction(Transaction transaction);
}
