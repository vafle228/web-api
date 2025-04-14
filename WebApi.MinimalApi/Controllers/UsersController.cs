using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApi.MinimalApi.Domain;
using WebApi.MinimalApi.Models;

namespace WebApi.MinimalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserRepository userRepository, IMapper mapper) : Controller
{
    [HttpGet("{userId}", Name = nameof(GetUserById))]
    [Produces("application/json", "application/xml")]
    public ActionResult<UserDto> GetUserById([FromRoute] Guid userId)
    {
        var user = userRepository.FindById(userId);
        return user is null ? NotFound() : Ok(mapper.Map<UserDto>(user));
    }

    [HttpPost]
    [Produces("application/json", "application/xml")]
    public IActionResult CreateUser([FromBody] UserCreateDto? user)
    {
        if (user is null) 
            return BadRequest();
        
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        
        var entity = userRepository.Insert(mapper.Map<UserEntity>(user));
        return CreatedAtRoute(nameof(GetUserById), new { userId = entity.Id }, entity.Id);
    }

    [HttpPut("{userId}")]
    [Produces("application/json", "application/xml")]
    public IActionResult UpdateUser([FromRoute] Guid userId, [FromBody] UserUpdateDto? user)
    {
        if (user is null || userId == Guid.Empty)
            return BadRequest();
        
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
            
        var entity = mapper.Map(user, new UserEntity(userId)); 
        userRepository.UpdateOrInsert(entity, out var created);
        return created ? CreatedAtRoute(nameof(GetUserById), new { userId = entity.Id }, entity.Id) : NoContent();
    }
}