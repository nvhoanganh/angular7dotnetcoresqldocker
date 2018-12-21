using dotnetcore.webapi;
using dotnetcore.webapi.Data;
using dotnetcore.webapi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace dotnetcore_webapi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly DockerDbContext _context;
		private readonly UserManager<ApplicationUser> _manager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public UsersController(DockerDbContext context, UserManager<ApplicationUser> manager, RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			_manager = manager;
			_roleManager = roleManager;
		}

		// GET: api/Users
		[HttpGet]
		[ProducesResponseType(typeof(List<UserDto>), (int)HttpStatusCode.OK)]
		public IEnumerable<UserDto> GetAll()
		{
			return _context.Users.Select(x => new UserDto { Id = x.Id, Email = x.Email, FirstName = x.FirstName, LastName = x.LastName });
		}

		// GET: api/Users/5
		[HttpGet("{id}", Name ="GetUser")]
		public async Task<IActionResult> Get([FromRoute] string id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var userDto = await _context.Users.FindAsync(id);

			if (userDto == null)
			{
				return NotFound();
			}

			return Ok(new UserDto { Id = userDto.Id, Email = userDto.Email, FirstName = userDto.FirstName, LastName = userDto.LastName });
		}

		// PUT: api/Users/5
		[HttpPut("{id}")]
		public async Task<IActionResult> PutUserDto([FromRoute] string id, [FromBody] UserDto userDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != userDto.Id)
			{
				return BadRequest();
			}

			var user = await _context.Users.FindAsync(id);
			if (user == null)
			{
				return BadRequest();
			}
			user.FirstName = userDto.FirstName;
			user.LastName = userDto.LastName;
			user.Email = userDto.Email;
			await _context.SaveChangesAsync();
			return Ok();
		}

		// POST: api/Users
		[HttpPost]
		public async Task<IActionResult> PostUserDto([FromBody] UserDto userDto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (string.IsNullOrEmpty(userDto.Password))
			{
				return BadRequest(ModelState);
			}

			var newUser = new ApplicationUser
			{
				Email = userDto.Email,
				FirstName = userDto.FirstName,
				LastName = userDto.LastName,
				UserName = userDto.Email,
				SecurityStamp = Guid.NewGuid().ToString()
			};

			await _manager.CreateAsync(newUser, userDto.Password);
			_context.Users.Add(newUser);
			return Ok();
		}

		// DELETE: api/Users/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUserDto([FromRoute] string id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var userDto = await _context.Users.FindAsync(id);
			if (userDto == null)
			{
				return NotFound();
			}
			await _manager.DeleteAsync(userDto);
			return Ok();
		}
	}
}
