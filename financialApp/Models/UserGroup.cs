using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UserGroup
{
    [Key]
    public int GroupID { get; set; }
    public string GroupName { get; set; }

    public ICollection<UserGroupMembership> Users { get; set; }

}
