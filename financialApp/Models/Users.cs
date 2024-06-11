using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserID { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Role { get; set; } = "User";

    public ICollection<UserGroupMembership> UserGroups { get; set; } = new List<UserGroupMembership>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<SavingGoal> SavingGoals { get; set; } = new List<SavingGoal>();
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    public ICollection<PeriodicReport> PeriodicReports { get; set; } = new List<PeriodicReport>();
}
