using HT.WaterAlerts.Domain;
using HT.WaterAlerts.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace HT.WaterAlerts.API.Controllers
{
    public class SubscriptionsController : CommonController
    {
        private readonly ISubscriptionService _subscriptionService;
        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] SubscriptionDTO subscription)
        {
            try
            {
                _subscriptionService.SaveSubscription(subscription);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(new ErrorResponseDTO() { Code = "HT400", Error = ex.Message });
            }
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

                var response = _subscriptionService.GetSubscriptions(userId);
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
    }
}
