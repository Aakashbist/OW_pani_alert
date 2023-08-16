using HT.WaterAlerts.Core;
using HT.WaterAlerts.Domain;
using Microsoft.Extensions.Logging;

namespace HT.WaterAlerts.Service
{
    public class TemplateService : ITemplateService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<TemplateService> _logger;

        public TemplateService(IUnitOfWork uow, ILogger<TemplateService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public Guid CreateTemplate(TemplateDTO template)
        {
            try
            {
                Template temp = new Template();
                temp.Name = template.Name;
                temp.Email = template.Email;
                temp.CreatedDate = DateTime.Now;

                _uow.GetRepository<Template>().Add(temp);
                _uow.SaveChanges();
                return temp.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on CreateTemplate", ex.Message);
                throw;
            }
        }

        public void UpdateTemplate(TemplateDTO template)
        {
            try
            {

                Template temp = _uow.GetRepository<Template>().GetById(template.Id);
                if (temp == null)
                    throw (new Exception("No data found with id " + template.Id));

                temp.Name = template.Name;
                temp.Email = template.Email;
                temp.ModifiedDate = DateTime.Now;

                _uow.GetRepository<Template>().Update(temp);
                int x = _uow.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on UpdateTemplate", ex.Message);
                throw;
            }
        }
    }
}
