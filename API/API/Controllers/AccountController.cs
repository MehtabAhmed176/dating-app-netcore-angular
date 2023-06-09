using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using api.Entities;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class AccountController : BaseApiController
{
    private DataContext _context;
    private HMACSHA512 _hmacsha512;
    private HMACSHA512 _hmac;
    private ITokenService _token;
    private readonly IMapper _mapper;

    public AccountController(DataContext context, ITokenService tokenService, IMapper mapper, HMACSHA512 hmac = null)
    {
        _context = context;
        _hmacsha512 = new HMACSHA512();
        _token = tokenService;
        _mapper = mapper;
        // @ TODO fix the injection
        // _hmac = new HMACSHA512(string passwordSalt);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username)) return BadRequest("Username already taken");
        // @TODO make it good for unit tests
        using var hmac = _hmacsha512;
        
        var user = _mapper.Map<AppUser>(registerDto);
        user.DateOfBirth =  System.DateOnly.Parse(registerDto.DateOfBirth.ToString());;
        user.UserName = registerDto.Username.ToLower();
        
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password.ToLower()));
        user.PasswordSalt = hmac.Key;
        
        _context.Users.Add(user);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e);
        }
        
        return new UserDto
        {
            Username = user.UserName,
            Token =  _token.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs
        };
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDTO)
    {
        // FindAsyn is used when know the PK
        var user = await _context.Users.Include(p => p.Photos)
            .SingleOrDefaultAsync(u => u.UserName == loginDTO.Username);
        if (user == null) return Unauthorized("UserName invalid");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.password));

        // Check if the hash values are equal
        bool areEqual = user.PasswordHash.SequenceEqual(computedHash);
        if (areEqual)
            return new UserDto()
            {
                Username = user.UserName,
                Token = _token.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        return Unauthorized("Invalid password");
    }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}