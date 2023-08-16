using HT.WaterAlerts.Domain;
using HT.WaterAlerts.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HT.WaterAlerts.API.Controllers
{
    public class ContactHistoryController : CommonController
    {
        private readonly IContactHistoryService _historyService;
        public ContactHistoryController(IContactHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet("{userId}")]
        public IActionResult Get(Guid userId)
        {
            try
            {
                if (!IsUserValid(userId))
                {
                    return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = "Invalid Permission" });
                }

                var response = _historyService.GetContactHistories(userId);
                if (response.Count() > 0)
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
        [HttpGet]
        public IActionResult Get([FromQuery] DataTableRequestDTO request)
        {
            try
            {
                var response = _historyService.GetContactHistoriesForManualSites(request);
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
        [HttpPost]
        public IActionResult Post([FromBody] ContactHistoryPostDTO contactHistory)
        {
            try
            {
                _historyService.SaveContactHistories(contactHistory);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = ex.Message });
            }
        }
    }
}
