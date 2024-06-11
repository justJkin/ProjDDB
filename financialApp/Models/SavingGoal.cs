using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SavingGoal
{
    [Key]
    public int GoalID { get; set; }
    public int UserID { get; set; }
    public decimal Amount { get; set; }
    public string GoalType { get; set; }
    public string Description { get; set; }

    public User User { get; set; }
}
