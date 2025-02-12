using System;

namespace API.Models.Users;

public interface IUser
{
        int UserId { get; set; }
        string Username { get; set; }
        string Role { get; set; }
}
