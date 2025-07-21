using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;
using WebApi.Entities.Dtos;
using WebApi.Services;
using WebApi.Services.Abstract;

namespace WebApi.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _service;

		public UsersController(IUserService service)
		{
			_service = service;
		}

		[Authorize(Roles = "admin")]
		[HttpGet("getall")]
		public IActionResult GetAllUsers()
		{
			var users = _service.GetAllUsers();
			return Ok(users);
		}

		[HttpGet("getbyid/{id}")]
		public IActionResult GetUserById(int id)
		{
			var user = _service.GetUserById(id);
			if (user == null)
				return NotFound();
			return Ok(user);
		}

		[Authorize(Roles = "admin")]
		[HttpPost("create")]
		public IActionResult CreateUser(User user)
		{
			_service.CreateUser(user);
			return Ok("Kullanıcı eklendi.");
		}

		[Authorize(Roles = "admin")]
		[HttpPut("update/{id}")]
		public IActionResult UpdateUser(int id, User updatedUser)
		{
			var user = _service.GetUserById(id);
			if (user == null)
				return NotFound();

			_service.UpdateUser(id, updatedUser);
			return Ok("Kullanıcı güncellendi.");
		}

		[Authorize(Roles = "admin")]
		[HttpDelete("delete/{id}")]
		public IActionResult DeleteUser(int id)
		{
			var user = _service.GetUserById(id);
			if (user == null)
				return NotFound();

			_service.DeleteUser(id);
			return Ok("Kullanıcı silindi.");
		}

		[HttpGet("orderbydate")]
		public IActionResult GetAllUsersOrderByDate()
		{
			var users = _service.GetAllUsersOrderByDate();
			return Ok(users);
		}

		[Authorize(Roles = "admin")]
		[HttpDelete("soft/{id}")]
		public IActionResult SoftDeleteUserById(int id)
		{
			var user = _service.GetUserById(id);
			if (user == null)
				return NotFound();
			_service.SoftDeleteUserById(id);
			return Ok("Kullanıcı pasif hale getirildi.");
		}

		


	}
}
