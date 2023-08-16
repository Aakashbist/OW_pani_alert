using HT.WaterAlerts.Core;
using HT.WaterAlerts.Domain;
using HT.WaterAlerts.Service.Interface;
using Microsoft.Extensions.Logging;

namespace HT.WaterAlerts.Service.Implementation
{
    public class MeasurementTypeService : IMeasurementTypeService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<MeasurementTypeService> _logger;

        public MeasurementTypeService(IUnitOfWork uow, ILogger<MeasurementTypeService> logger)
        {
            _uow = uow;
            _logger = logger;
        }
        public Guid GetMeasurementTypeIdByName(string name)
        {
            try
            {
                return _uow.GetRepository<MeasurementType>().GetFirst(x => x.Name == name).Id;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on while retriving Measurement Type ID", ex.Message);
                throw;
            }
        }
    }
}
