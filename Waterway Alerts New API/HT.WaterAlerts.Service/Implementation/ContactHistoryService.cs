using HT.WaterAlerts.Common.Email;
using HT.WaterAlerts.Core;
using HT.WaterAlerts.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace HT.WaterAlerts.Service
{
    public class ContactHistoryService : IContactHistoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ContactHistoryService> _logger;
        private readonly ISmtpClient _smtpClient;

        public ContactHistoryService(IUnitOfWork uow, ILogger<ContactHistoryService> logger, ISmtpClient smtpClient)
        {
            _uow = uow;
            _logger = logger;
            _smtpClient = smtpClient;
        }

        public IEnumerable<ContactHistoryDTO> GetContactHistories(Guid userId)
        {
            try
            {
                var items = _uow.GetRepository<ContactHistory>()
                    .Get(d => d.UserId == userId, o => o.CreatedDate, SortOrder.Ascending)
                    .Include(a => a.AlertLevel).ThenInclude(t => t.MeasurementSite);
                return items.Select(x => new ContactHistoryDTO().MapToContactHistoryDTO(x));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on GetSites", ex.Message);
                throw;
            }
        }

        public DataTableResponseDTO GetContactHistoriesForManualSites(DataTableRequestDTO request)
        {
            try
            {
                var param = ServiceHelper.GetDataTableParams(request);
                var criteria = ServiceHelper.GetFilterPredicate<ContactHistory>(param.Filter + " AND !AlertLevel.MeasurementSite.IsAutomated");
                var orderBy = ServiceHelper.GetOrderPredicate<AdminContactHistoryDTO>(param.OrderColumn);
                var items = _uow.GetRepository<ContactHistory>()
                    .Get(criteria)
                    .Include(c => c.AlertLevel).ThenInclude(a => a.MeasurementSite)
                    .Include(c => c.User).Include(c => c.CreatedUser)
                    .GroupBy(x => x.BatchNumber)
                    .Select(x => new AdminContactHistoryDTO()
                            {
                                Id = x.Key,
                                AlertLevel = x.Select(y => y.AlertLevel.Name).FirstOrDefault(),
                                Content = x.Select(y => y.Content).FirstOrDefault(),
                                CreatedByUser = x.Select(y => y.CreatedUser.FirstName + " " + y.CreatedUser.LastName).FirstOrDefault(),
                                Site = x.Select(y => y.AlertLevel.MeasurementSite.Name).FirstOrDefault(),
                                CreatedDate = x.Select(y => y.CreatedDate).FirstOrDefault(),
                                Recipent = x.Select(y => y.User.FirstName + " " + y.User.LastName).ToList(),
                            });

                if (param.OrderDirection == SortOrder.Ascending)
                {
                    items = items.OrderBy(orderBy).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
                }
                else
                {
                    items = items.OrderByDescending(orderBy).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
                }
               

                int totalPageCount = _uow.GetRepository<ContactHistory>().Get(criteria).GroupBy(c => c.BatchNumber).Count();
                int totalPage = (int)Math.Ceiling((double)totalPageCount / request.PageSize);
                return new DataTableResponseDTO
                {
                    TotalCount = totalPageCount,
                    TotalPage = totalPage,
                    CurrentPage = request.PageNumber,
                    Data = items.ToArray()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on GetSites", ex.Message);
                throw;
            }
        }

        public void SaveContactHistories(ContactHistoryPostDTO contactHistory)
        {
            try
            {
                AlertLevel? level = _uow.GetRepository<AlertLevel>().Get(a => a.Id == contactHistory.AlertLevelId)
                                                                    .Include(s => s.Subscriptions).ThenInclude(s=>s.User)
                                                                    .FirstOrDefault();
                string batchNumber = contactHistory.AlertLevelId + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                foreach (var subscription in level.Subscriptions)
                {
                    ContactHistory history = new ContactHistory()
                    {
                        CreatedDate = DateTime.Now,
                        AlertLevelId = contactHistory.AlertLevelId,
                        UserId = subscription.UserId,
                        Content = "Subject: " + contactHistory.Subject + "\n" + contactHistory.Content.RemoveHtmlAttributes(),
                        AlertType = "email",
                        BatchNumber = batchNumber,
                        CreatedUserId = contactHistory.CreatedUserId

                    };
                    _uow.GetRepository<ContactHistory>().Add(history);
                    _uow.SaveChanges();

                    try
                    {
                        _smtpClient.Send(subscription.User.Email, contactHistory.Subject.RemoveHtmlAttributes(), contactHistory.Content);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error occured while sending mail on SaveContactHistories", ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on SaveContactHistories", ex.Message);
                throw;
            }
        }
    }
}
