using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace financialApp.Models
{
    public class Item
    {
        [Key]
        public int ItemID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
