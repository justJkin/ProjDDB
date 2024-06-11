using System;
using System.Collections.Generic;
using System.Linq;
using financialApp.Data;
using financialApp.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace financialApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
      

        public UserRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
          
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User? GetUserById(int id)
        {
            return _context.Users.Find(id);
        }

        public void AddUser(User user)
        {
            // Brak haszowania hasła przed dodaniem do bazy
            _context.Users.Add(user);
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Register(User user)
        {
            var adminExists = _context.Users.Any(u => u.Role == "Admin");

            if (!adminExists)
            {
                user.Role = "Admin";
            }
            else
            {
                user.Role = "User";
            }

            // Brak haszowania hasła przed dodaniem do bazy
 

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Dodaj użytkownika bezpośrednio za pomocą surowego SQL
                    var sql = "INSERT INTO Users (Username, Password, Email, Role) VALUES (@Username, @Password, @Email, @Role);";
                    _context.Database.ExecuteSqlRaw(sql,
                        new SqlParameter("@Username", user.Username),
                        new SqlParameter("@Password", user.Password),
                        new SqlParameter("@Email", user.Email),
                        new SqlParameter("@Role", user.Role)
                    );
                   
                    // Ręczne dodanie użytkownika do domyślnej grupy (zakładając, że domyślna grupa ma GroupID = 1)

                    var userId = _context.Users.SingleOrDefault(u => u.Username == user.Username)?.UserID;
                    
                    if (userId != null)
                    {
                        var groupSql = "INSERT INTO UserGroupMemberships (UserID, GroupID) VALUES (@UserID, 1);";
                        _context.Database.ExecuteSqlRaw(groupSql, new SqlParameter("@UserID", userId));
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public User? GetUserByUsernameAndPassword(string username, string password)
        {
            // Brak haszowania haseł do porównania
            return _context.Users.SingleOrDefault(u => u.Username == username && u.Password == password);
        }
        public IEnumerable<Transaction> GetAllTransactions()
        {
            return _context.Transactions.ToList();
        }

        public IEnumerable<SavingGoal> GetAllSavingGoals()
        {
            return _context.SavingGoals.ToList();
        }

        public IEnumerable<Transaction> GetUserTransactions(int userId)
        {
            return _context.Transactions.Where(t => t.UserID == userId).ToList();
        }
    }
}
