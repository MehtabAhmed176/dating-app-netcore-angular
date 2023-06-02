
using System.Security.Claims;
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
    
    [HttpPut]
    public async Task <ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _iUserRepository.GetUserByUserNameAsync(username);

        if (user == null)  return NotFound("user not found");
        _mapper.Map(memberUpdateDto, user); // make the MemberDto equals to user
        if (await _iUserRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user");

    }
    
}