using HT.WaterAlerts.Core;
using HT.WaterAlerts.Domain;
using HT.WaterAlerts.Service.Interface;
using Microsoft.Extensions.Logging;

namespace HT.WaterAlerts.Service.Implementation
{
    public class MeasurementUnitService : IMeasurementUnitService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<MeasurementUnitService> _logger;


        public MeasurementUnitService(IUnitOfWork uow, ILogger<MeasurementUnitService> logger)
        {
            _uow = uow;
            _logger = logger;
        }
        public Guid GetMeasurementUnitIdByName(string name)
        {

            try
            {
                return _uow.GetRepository<MeasurementUnit>().GetFirst(x => x.Name == name).Id;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on while retriving Measurement Unit ID", ex.Message);
                throw;
            }
        }
    }
}
