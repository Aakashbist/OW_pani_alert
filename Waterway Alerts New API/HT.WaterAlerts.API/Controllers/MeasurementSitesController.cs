using HT.WaterAlerts.Domain;
using HT.WaterAlerts.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HT.WaterAlerts.API.Controllers
{
    public class MeasurementSitesController : CommonController
    {
        private readonly IMeasurementSiteService _siteService;
        public MeasurementSitesController(IMeasurementSiteService siteService)
        {
            _siteService = siteService;
        }

        [HttpGet("SitesWithAlertLevels")]
        public IActionResult Get()
        {
            try
            {
                var response = _siteService.GetSites();
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
        [HttpGet("ManualSites")]
        public IActionResult GetSites([FromQuery] DataTableRequestDTO request)
        {
            try
            {
                var response = _siteService.GetSites(request);
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
        [HttpPost("CreateSite")]
        public IActionResult Post(MeasurementSitesDTO sitesDTO)
        {
            try
            {
                _siteService.CreateSite(sitesDTO);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("UpdateSite")]
        public async Task<IActionResult> Put([FromBody] MeasurementSitesDTO siteDto)
        {
            try
            {
                _siteService.UpdateSite(siteDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = ex.Message });
            }

        }
    }
}
