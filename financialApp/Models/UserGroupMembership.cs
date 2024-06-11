using System;

public class UserGroupMembership
{
    public int UserID { get; set; }
    public User User { get; set; }

    public int GroupID { get; set; }
    public UserGroup UserGroup { get; set; }
}
