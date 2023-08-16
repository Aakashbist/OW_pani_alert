using HT.WaterAlerts.Core;
using HT.WaterAlerts.Domain;
using HT.WaterAlerts.Service.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace HT.WaterAlerts.Service
{
    public class MeasurementSiteService : IMeasurementSiteService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<MeasurementSiteService> _logger;
        private readonly IMeasurementTypeService _measurementTypeService;
        private readonly IMeasurementUnitService _measurementUnitService;

        private const string MeasurementTypeName = "Flow Rate";
        private const string MeasurementUnitName = "Cumecs";
        private const string OutageStartedTemplateName = "Outage Started";
        private const string OutageStoppedTemplateName = "Outage Stopped";

        public MeasurementSiteService(IUnitOfWork uow, ILogger<MeasurementSiteService> logger, IMeasurementTypeService measurementTypeService, IMeasurementUnitService measurementUnitService)
        {
            _uow = uow;
            _logger = logger;
            _measurementTypeService = measurementTypeService;
            _measurementUnitService = measurementUnitService;
        }

        public void CreateSite(MeasurementSitesDTO siteDto)
        {
            var typeId = _measurementTypeService.GetMeasurementTypeIdByName(MeasurementTypeName);
            var unitId = _measurementUnitService.GetMeasurementUnitIdByName(MeasurementUnitName);
            var templates = _uow.GetRepository<Template>().GetAll();
            var outageStartedTemplate = templates.First(x => x.Name == OutageStartedTemplateName).Id;
            var outageStoppedTemplate = templates.First(x => x.Name == OutageStoppedTemplateName).Id;


            var measurementSite = MeasurementSite.CreateMeasurementSite(siteDto.Name, siteDto.Description, siteDto.Status, siteDto.Coordinates, typeId, unitId, outageStartedTemplate, outageStoppedTemplate);
            var template = Template.CreateTemplate(siteDto.Name);
            var alertLevel = AlertLevel.CreateAlertLevel(measurementSite, template, typeId);

            _uow.GetRepository<MeasurementSite>().Add(measurementSite);
            _uow.GetRepository<Template>().Add(template);
            _uow.GetRepository<AlertLevel>().Add(alertLevel);
            _uow.SaveChanges();
        }

        public IEnumerable<MeasurementSitesDTO> GetSites()
        {
            try
            {
                var items = _uow.GetRepository<MeasurementSite>()
                    .Get(d => d.Status, o => o.Name, SortOrder.Ascending)
                    .Include(t => t.MeasurementType).Include(u => u.MeasurementUnit)
                    .Include(a => a.AlertLevels).ThenInclude(t => t.MeasurementType);
                return items.Select(x => new MeasurementSitesDTO().MapToMeasurementSitesDTO(x));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on GetSites", ex.Message);
                throw;
            }
        }

        public DataTableResponseDTO GetSites(DataTableRequestDTO request)
        {
            var param = ServiceHelper.GetDataTableParams(request);
            var criteria = ServiceHelper.GetFilterPredicate<MeasurementSite>(param.Filter + " AND !IsAutomated");
            var orderBy = ServiceHelper.GetOrderPredicate<MeasurementSite>(param.OrderColumn);
            var items = _uow.GetRepository<MeasurementSite>().Get(criteria, orderBy, param.OrderDirection, request.PageNumber, request.PageSize);
            items = items.Include(t => t.MeasurementType).Include(u => u.MeasurementUnit);
            var sites = items.Select(x => new MeasurementSitesDTO().MapToOnlyManualMeasurementSitesDTO(x));
            int totalPageCount = _uow.GetRepository<MeasurementSite>().Count(criteria);
            int totalPage = (int)Math.Ceiling((double)totalPageCount / request.PageSize);
            return new DataTableResponseDTO
            {
                TotalCount = totalPageCount,
                TotalPage = totalPage,
                CurrentPage = request.PageNumber,
                Data = sites.ToArray()
            };
        }

        public void UpdateSite(MeasurementSitesDTO siteDto)
        {
            try
            {
                MeasurementSite site = _uow.GetRepository<MeasurementSite>().GetById(siteDto.Id);
                if (site is not null)
                {
                    site.Description = siteDto.Description;
                    site.Status = siteDto.Status;
                    site.Coordinates = siteDto.Coordinates;
                    site.Name = siteDto.Name;

                    _uow.GetRepository<MeasurementSite>().Update(site);
                    _uow.SaveChanges();
                }
                else
                {
                    CreateSite(siteDto);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on Update Measurement site", ex.Message);
                throw;
            }
        }
    }
}
