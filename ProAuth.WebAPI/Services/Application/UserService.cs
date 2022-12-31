using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProAuth.Services.Contracts;
using ProAuth.WebAPI.Dtos;
using ProAuth.WebAPI.Models.Identity;
using ProAuth.WebAPI.Security;

namespace ProAuth.Services.Application;

public class UserService : IUserService
{
    public UserManager<User> _userManager { get; }
    public SignInManager<User> _signInManager { get; }
    public RoleManager<Role> _roleInManager { get; }
    public IMapper _mapper { get; }

    public UserService(UserManager<User> UserManager
    , SignInManager<User> signInManager
    , RoleManager<Role> roleInManager
    , IMapper mapper
    )
    {
        _userManager = UserManager;
        _signInManager = signInManager;
        _roleInManager = roleInManager;
        _mapper = mapper;
    }
    public async Task<RetornoDto> RegisterUser(UserDto userDto)
    {
        var retorno = new RetornoDto();

        try
        {
            var user = _mapper.Map<User>(userDto);

            var userFound = await _userManager.FindByNameAsync(user.UserName);

            if (userFound != null)
            {
                retorno.Message = "Usuário já cadastrado!";
                retorno.StatusCode = StatusCodes.Status409Conflict;
                retorno.Object = userFound;
                return retorno;
            }

            var role = await _roleInManager.FindByNameAsync(userDto.Role);

            if (role == null)
            {
                retorno.Message = "Role não encontrada";
                retorno.StatusCode = StatusCodes.Status404NotFound;
                return retorno;
            }

            user.FullName = user.FullName.Trim();
            user.UserName = user.UserName.Trim();

            var result = await _userManager.CreateAsync(user, userDto.Password);


            if (role != null) await _userManager.AddToRoleAsync(user, userDto.Role);

            if (result.Succeeded)
            {
                retorno.Success = true;
                retorno.Message = "Usuário criado com sucesso!";
                retorno.Object = await _userManager.FindByNameAsync(user.UserName);
            }
            else
            {
                retorno.Object = result.Errors;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            retorno.Success = false;
            retorno.Message = "Erro ao tentar adicionar usuário";
            retorno.Object = ex.Message;
        }

        return retorno;
    }

    public async Task<RetornoDto> LoginUser(UserLoginDto userLogin)
    {
        var retorno = new RetornoDto();

        try
        {
            var userFound = await _userManager.Users
                .FirstOrDefaultAsync(u => u.NormalizedUserName == userLogin.UserName.ToUpper());

            if (userFound == null)
            {
                retorno.Message = "Usuario não encontrado";
                retorno.StatusCode = StatusCodes.Status404NotFound;
                return retorno;
            }

            if (userFound.Status == false)
            {
                retorno.Message = "Usuario desativado";
                retorno.StatusCode = StatusCodes.Status403Forbidden;
                return retorno;
            }

            var passWordOk = await _userManager.CheckPasswordAsync(userFound, userLogin.Password);

            if (passWordOk)
            {

                var userToReturn = _mapper.Map<UserDto>(userFound);
                var roles = await _userManager.GetRolesAsync(userFound);

                userFound.DataUltimoLogin = DateTime.Now;

                await _userManager.UpdateAsync(userFound);

                // var log = new UserAction();

                // log.Description = "Fez login no PRO";
                // log.Tittle = "Login";
                // log.UserId = user.Id;

                // _repo.Add(log);

                // await _repo.SaveChangesAsync();

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userFound.Id.ToString()),
                new Claim(ClaimTypes.Name, userFound.UserName),
                new Claim("ExternalId", userFound.ExternalId.ToString())
            };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var retornoToken = new
                {
                    accessToken = TokenService.GenerateAccessToken(claims),
                    refreshToken = TokenService.GenerateRefreshToken(claims),
                    user = userToReturn,
                    roles = roles
                };

                retorno.Success = true;
                retorno.Message = "Login realizado com sucesso";
                retorno.StatusCode = StatusCodes.Status200OK;
                retorno.Object = retornoToken;
            }
            else
            {
                retorno.Success = false;
                retorno.Message = "Senha incorreta";
                retorno.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            retorno.Success = false;
            retorno.Message = "Erro ao tentar realizar login";
            retorno.Object = ex.Message;
        }


        return retorno;
    }

    public async Task<RetornoDto> RefreshToken(TokenDto tokenDto)
    {
        var retorno = new RetornoDto();

        retorno.StatusCode = StatusCodes.Status404NotFound;
        retorno.Success = false;

        if (tokenDto is null)
        {
            retorno.Message = "Token null";
            return retorno;
        }

        string accessToken = tokenDto.AccessToken;
        string refreshToken = tokenDto.RefreshToken;


        try
        {
            var principal = await TokenService.GetPrincipalFromExpiredToken(refreshToken);

            var username = principal.Identity.Name;

            var userFound = await _userManager.Users
                .FirstOrDefaultAsync(u => u.NormalizedUserName == username.ToUpper());

            if (userFound == null)
            {
                retorno.Message = "Usuario não encontrado";
                return retorno;
            }

            if (userFound.Status == false)
            {
                retorno.Message = "Usuario desativado";
                retorno.StatusCode = StatusCodes.Status403Forbidden;
                return retorno;
            }

            var newAccessToken = TokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = TokenService.GenerateRefreshToken(principal.Claims);


            var newToken = new TokenDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

            retorno.Success = true;
            retorno.StatusCode = 200;
            retorno.Object = newToken;
            retorno.Message = "Access token gerado com sucesso";

           
        }catch (SecurityTokenException ex)
        {
            Console.WriteLine(ex.StackTrace);
            retorno.StatusCode = StatusCodes.Status403Forbidden;
            retorno.Success = false;
            retorno.Message = "Erro ao tentar realizar atualizar o token. Token invalido";
            retorno.Object = ex.Message;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            retorno.StatusCode = StatusCodes.Status500InternalServerError;
            retorno.Success = false;
            retorno.Message = "Erro ao tentar realizar atualizar o token";
            retorno.Object = ex.Message;
        }


        return retorno;
    }
}
