using System;

namespace API.Models.Users;

public class User : IUser
{
    public int UserId {get; set;}
    public string Username {get; set;}
    public string PasswordHash {get; set;}
    public string Role {get; set;}
}
