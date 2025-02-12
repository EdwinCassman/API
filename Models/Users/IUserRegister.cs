using System;

namespace API.Models.Users;

public interface IUserRegister
{
    string Username { get; set; }
    string Password { get; set; }
    bool IsAdmin { get; set; }
}
