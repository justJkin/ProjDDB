using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Reminder
{
    [Key]
    public int ReminderID { get; set; }
    public int UserID { get; set; }
    public string Frequency { get; set; }
    public DateTime NextReminderDate { get; set; }
    public string Description { get; set; }

    public User User { get; set; }
}
