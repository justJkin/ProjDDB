using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PeriodicReport
{
    [Key]
    public int ReportID { get; set; }
    public int UserID { get; set; }
    public string ReportType { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string ReportData { get; set; }

    public User User { get; set; }
}
