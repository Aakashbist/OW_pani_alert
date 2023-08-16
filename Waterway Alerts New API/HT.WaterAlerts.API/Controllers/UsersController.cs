using Microsoft.AspNetCore.Mvc;
using HT.WaterAlerts.Domain;
using HT.WaterAlerts.Service;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace HT.WaterAlerts.API.Controllers
{
    public class UsersController : CommonController
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Get([FromQuery] DataTableRequestDTO request)
        {
            DataTableResponseDTO response = _userService.GetUsers(request);
            if (response.Data.Length > 0)
            {
                return Ok(response);
            }
            return NotFound(new ErrorResponseDTO() { Code = "HT404", Error = "Data Not Found" });
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            if (!IsUserValid(id))
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = "Invalid Permission" });
            }

            UsersDTO usersData = _userService.GetUser(id);
            if (usersData != null)
            {
                return Ok(usersData);
            }
            return NotFound(new ErrorResponseDTO() { Code = "HT404", Error = "Data Not Found" });
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UsersDTO user)
        {
            var result = await _userService.CreateUser(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = result.Errors });
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UsersDTO user)
        {
            var result = await _userService.UpdateUser(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = result.Errors });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            var result = await _userService.DeleteUser(id);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = result.Errors });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] JsonPatchDocument patchUser)
        {
            if (!IsUserValid(id))
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = "Invalid Permission" });
            }
            
            var result = await _userService.UpdateUserPartial(id, patchUser, IsUserAdmin());
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = result.Errors });
        }
    }
}
