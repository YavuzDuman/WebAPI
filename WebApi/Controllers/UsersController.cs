using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;
using WebApi.Entities.Dtos;
using WebApi.DataAccess;
using WebApi.DataAccess.Abstract;
using WebApi.Business.Abstract;

namespace WebApi.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserManager _manager;

		public UsersController(IUserManager manager)
		{
			_manager = manager;
		}

		[Authorize(Roles = "admin")]
		[HttpGet("getall")]
		public IActionResult GetAllUsers()
		{
			var users = _manager.GetAllUsers();
			return Ok(users);
		}

		[HttpGet("getbyid/{id}")]
		public IActionResult GetUserById(int id)
		{
			var user = _manager.GetUserById(id);
			if (user == null)
				return NotFound();
			return Ok(user);
		}

		[Authorize(Roles = "admin,manager")]
		[HttpPost("create")]
		public IActionResult CreateUser(User user)
		{
			_manager.CreateUser(user);
			return Ok("Kullanıcı eklendi.");
		}

		[Authorize(Roles = "admin,manager")]
		[HttpPut("update/{id}")]
		public IActionResult UpdateUser(int id, User updatedUser)
		{
			var user = _manager.GetUserById(id);
			if (user == null)
				return NotFound();

			_manager.UpdateUser(id, updatedUser);
			return Ok("Kullanıcı güncellendi.");
		}

		[Authorize(Roles = "admin")]
		[HttpDelete("delete/{id}")]
		public IActionResult DeleteUser(int id)
		{
			var user = _manager.GetUserById(id);
			if (user == null)
				return NotFound();

			_manager.DeleteUser(id);
			return Ok("Kullanıcı silindi.");
		}

		[HttpGet("orderbydate")]
		public IActionResult GetAllUsersOrderByDate()
		{
			var users = _manager.GetAllUsersOrderByDate();
			return Ok(users);
		}

		[Authorize(Roles = "admin")]
		[HttpDelete("soft/{id}")]
		public IActionResult SoftDeleteUserById(int id)
		{
			var user = _manager.GetUserById(id);
			if (user == null)
				return NotFound();
			_manager.SoftDeleteUserById(id);
			return Ok("Kullanıcı pasif hale getirildi.");
		}

		


	}
}
