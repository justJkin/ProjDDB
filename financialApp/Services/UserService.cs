using System;
using System.Collections.Generic;
using System.Linq;
using financialApp.Repositories;
using financialApp.Models;
using financialApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using financialApp.Data;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly DataContext _context;

    public UserService(IUserRepository userRepository, DataContext context)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _userRepository.GetAllUsers();
    }

    public User GetUserById(int id)
    {
        return _userRepository.GetUserById(id);
    }

    public void CreateUser(User user)
    {
        _userRepository.AddUser(user);
        _userRepository.Save();
    }

    public void UpdateUser(User user)
    {
        _userRepository.UpdateUser(user);
        _userRepository.Save();
    }

    public void DeleteUser(int id)
    {
        _userRepository.DeleteUser(id);
        _userRepository.Save();
    }

    public bool AuthenticateUser(string username, string password)
    {
        var user = _userRepository.GetUserByUsernameAndPassword(username, password);
        return user != null;
    }

    public string GetUserRole(string username)
    {
        var user = _userRepository.GetAllUsers().FirstOrDefault(u => u.Username == username);
        return user?.Role ?? "User";
    }

    public bool RegisterUser(string username, string email, string password)
    {
        if (_userRepository.GetAllUsers().Any(u => u.Username == username))
        {
            return false;
        }

        var user = new User
        {
            Username = username,
            Email = email,
            Password = password,
            Role = "User"  // Ustaw domyślną rolę jako "User"
        };

        _userRepository.AddUser(user);
        _userRepository.Save();

        return true;
    }

    public decimal GetTotalBalance(int userId)
    {
        // Zakładając, że TotalBalance to suma wszystkich transakcji użytkownika
        return _userRepository.GetAllTransactions().Where(t => t.UserID == userId).Sum(t => t.Amount);
    }

    public decimal GetTotalExpenses(int userId)
    {
        // Zakładając, że TotalExpenses to suma wszystkich wydatków użytkownika
        return _userRepository.GetAllTransactions().Where(t => t.UserID == userId && t.Amount < 0).Sum(t => t.Amount);
    }

    public decimal GetTotalSavings(int userId)
    {
        // Zakładając, że TotalSavings to suma wszystkich oszczędności użytkownika
        return _userRepository.GetAllSavingGoals().Where(sg => sg.UserID == userId).Sum(sg => sg.Amount);
    }

    public string GetUsername(int userId)
    {
        var user = _userRepository.GetUserById(userId);
        return user?.Username;
    }

    public string GetEmail(int userId)
    {
        var user = _userRepository.GetUserById(userId);
        return user?.Email;
    }

    public int GetUserId(string username)
    {
        var user = _userRepository.GetAllUsers().FirstOrDefault(u => u.Username == username);
        return user?.UserID ?? 0; // Zwraca 0 jeśli nie znajdzie użytkownika
    }

    public IEnumerable<Transaction> GetUserTransactions(int userId)
    {
        return _userRepository.GetUserTransactions(userId);
    }

    public void AddTransaction(Transaction transaction)
    {
        // Sprawdzenie, czy UserID istnieje
        var userExists = _context.Users.Any(u => u.UserID == transaction.UserID);
        if (!userExists)
        {
            throw new Exception($"User with ID {transaction.UserID} does not exist.");
        }

        _context.Transactions.Add(transaction);
        _context.SaveChanges();
    }
}
