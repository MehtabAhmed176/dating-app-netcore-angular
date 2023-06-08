using System.Security.Claims;
using API.DTOs;
using api.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private IUserRepository _iUserRepository;

    private IMapper _mapper;

    private IPhotoService _photoService;

    // section 8 - introduce the repository pattern
    public UsersController(IUserRepository iUserRepository, IMapper iMapper, IPhotoService photoService)
    {
        _mapper = iMapper;
        _iUserRepository = iUserRepository;
        _photoService = photoService;
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
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var user = await _iUserRepository.GetUserByUserNameAsync(User.GetUsername());

        if (user == null) return NotFound("user not found");
        _mapper.Map(memberUpdateDto, user); // make the MemberDto equals to user
        if (await _iUserRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user");
    }
    
    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _iUserRepository.GetUserByUserNameAsync(User.GetUsername());

        if (user == null) return NotFound("Username not found");

        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        if (await _iUserRepository.SaveAllAsync())
        {
            return CreatedAtAction(nameof(GetUser),
                new { username = user.UserName },
                _mapper.Map<PhotoDto>(photo));
        }

        return BadRequest("Problem while adding photo");
    }
    
    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _iUserRepository.GetUserByUserNameAsync(User.GetUsername()); //User.GetUsername() is helper function

        if (user == null) return NotFound("User Not Found");
        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound("No Photo associated with this Id");
        if (photo.IsMain) return BadRequest("This is already a main photo");
        

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if (currentMain != null)  currentMain.IsMain = false;

        photo.IsMain = true;
        
        if(await _iUserRepository.SaveAllAsync()) return NoContent();
        
        return BadRequest("Problem setting the main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _iUserRepository.GetUserByUserNameAsync(User.GetUsername());
        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
        
        if (photo == null) return NotFound("No Photo found");

        if (photo.IsMain) return BadRequest("Cannot Delete a main photo");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _iUserRepository.SaveAllAsync()) return Ok();
        
        return BadRequest("Problem deleting a photo");
    }

}