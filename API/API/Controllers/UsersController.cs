
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsersController: BaseApiController
{
    private IUserRepository _iUserRepository;

    private IMapper _mapper;

    // section 8 - introduce the repository pattern
    public UsersController(IUserRepository iUserRepository, IMapper iMapper)
    {
        _mapper = iMapper;
        _iUserRepository = iUserRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await _iUserRepository.GetMembersAsync();
        return Ok(users);
    }

    [HttpGet("{username}", Name = "GetUser")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
      return await _iUserRepository.GetMemberAsync(username);
    }
    
    [HttpDelete("{id}")]
    public ActionResult<AppUser> DeleteUser(int id)
    {
        return new AppUser();
    }
    
}