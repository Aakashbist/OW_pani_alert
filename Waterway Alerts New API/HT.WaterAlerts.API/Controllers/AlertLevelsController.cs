using HT.WaterAlerts.Domain;
using HT.WaterAlerts.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HT.WaterAlerts.API.Controllers
{
    public class AlertLevelsController : CommonController
    {
        private readonly IAlertLevelService _alertLevelService;
        public AlertLevelsController(IAlertLevelService alertLevelService)
        {
            _alertLevelService = alertLevelService;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Get([FromQuery] DataTableRequestDTO request)
        {
            try
            {
                var response = _alertLevelService.GetAlertLevels(request);
                if (response.Data.Length > 0)
                {
                    return Ok(response);
                }
                return NotFound(new ErrorResponseDTO() { Code = "HT404", Error = "Data Not Found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("{id}")]
        public IActionResult Patch(Guid id, [FromBody] JsonPatchDocument patchLevels)
        {
            try
            {
                _alertLevelService.UpdateAlertLevelsPartial(id, patchLevels);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = ex.Message });
            }
        }
    }
}
