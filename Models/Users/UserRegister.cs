using System;

namespace API.Models.Users;

public class UserRegister : IUserRegister
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsAdmin { get; set; }
}
