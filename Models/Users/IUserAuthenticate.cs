using System;

namespace API.Models.Users;

public interface IUserAuthenticate
{
    string Username {get; set;}
    string Password {get; set;}
}
