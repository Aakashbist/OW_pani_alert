using HT.WaterAlerts.Domain;
using HT.WaterAlerts.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HT.WaterAlerts.API.Controllers
{
    public class TemplatesController : CommonController
    {
        private readonly ITemplateService _templateService;
        public TemplatesController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Post(TemplateDTO template)
        {
            try
            {
                Guid createdId = _templateService.CreateTemplate(template);
                return Ok(createdId);

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        public IActionResult Put(TemplateDTO template)
        {
            try
            {
                _templateService.UpdateTemplate(template);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = ex.Message });
            }
        }
    }
}
