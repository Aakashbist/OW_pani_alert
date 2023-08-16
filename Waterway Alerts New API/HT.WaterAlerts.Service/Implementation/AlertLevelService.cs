using HT.WaterAlerts.Core;
using HT.WaterAlerts.Domain;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace HT.WaterAlerts.Service
{
    public class AlertLevelService : IAlertLevelService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AlertLevelService> _logger;

        public AlertLevelService(IUnitOfWork uow, ILogger<AlertLevelService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public DataTableResponseDTO GetAlertLevels(DataTableRequestDTO request)
        {
            try
            {

                var param = ServiceHelper.GetDataTableParams(request);
                var criteria = ServiceHelper.GetFilterPredicate<AlertLevel>(param.Filter + " AND !MeasurementSite.IsAutomated AND MeasurementSite.Status");
                var orderBy = ServiceHelper.GetOrderPredicate<AlertLevel>(param.OrderColumn);
                var items = _uow.GetRepository<AlertLevel>()
                    .Get(criteria, orderBy, param.OrderDirection, request.PageNumber, request.PageSize)
                    .Include(t => t.MeasurementType)
                    .Include(m => m.Template).Include(a => a.Subscriptions).ThenInclude(u => u.User)
                    .Include(s => s.MeasurementSite).ThenInclude(b => b.MeasurementUnit)
                    .Include(s => s.MeasurementSite).ThenInclude(c => c.MeasurementType);

                var levels = items.Select(x => new AlertLevelSubscriptionsDTO().MapToAlertLevelSubscriptionsDTO(x));
                int totalPageCount = _uow.GetRepository<AlertLevel>().Count(criteria);
                int totalPage = (int)Math.Ceiling((double)totalPageCount / request.PageSize);

                return new DataTableResponseDTO
                {
                    TotalCount = totalPageCount,
                    TotalPage = totalPage,
                    CurrentPage = request.PageNumber,
                    Data = levels.ToArray()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on GetAlertLevels", ex.Message);
                throw;
            }
        }

        public void UpdateAlertLevelsPartial(Guid id, JsonPatchDocument patchLevels)
        {
            try
            {
                AlertLevel level = _uow.GetRepository<AlertLevel>().GetById(id);
                patchLevels.ApplyTo(level);
                _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on UpdateAlertLevelsPartial", ex.Message);
                throw;
            }
        }
    }
}
