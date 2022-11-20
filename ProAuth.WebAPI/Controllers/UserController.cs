using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProAuth.Services.Contracts;
using ProAuth.WebAPI.Dtos;


namespace ProAuth.API.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{

    public IUserService _userService { get; }

    public UserController(IUserService UserService)
    {
        _userService = UserService;
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> Register(UserDto user)
    {
        var retorno = await _userService.RegisterUser(user);

        if(retorno.Success){
            return Ok(retorno);
        }else
        {
            return this.StatusCode(retorno.StatusCode,retorno);
        }
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(UserLoginDto user)
    {
        var retorno = await _userService.LoginUser(user);

        if(retorno.Success){
            return Ok(retorno);
        }else
        {
            return this.StatusCode(retorno.StatusCode,retorno);
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Teste()
    {
        return Ok("Estou funcionando");
    }
}
