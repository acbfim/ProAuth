using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProAuth.WebAPI.Dtos;

namespace ProAuth.Services.Contracts;

public interface IUserService
{
    Task<RetornoDto> RegisterUser (UserDto User);
    Task<RetornoDto> LoginUser (UserLoginDto User);
    
}
