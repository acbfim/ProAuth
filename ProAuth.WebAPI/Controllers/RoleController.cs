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

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{

    public IRoleService _roleService { get; }

    public RoleController(IRoleService RoleService)
    {
        _roleService = RoleService;
    }

    //[Authorize("Bearer", Roles = "Administrador,Suporte")]
    [AllowAnonymous]
    [HttpPost("CreateRole")]
    public async Task<IActionResult> CreateRole(CreateRoleDto role)
    {
        var retorno = await _roleService.CreateRole(role);

        if(retorno.Success){
            return Ok(retorno);
        }else
        {
            return this.StatusCode(retorno.StatusCode,retorno);
        }
    }
}
