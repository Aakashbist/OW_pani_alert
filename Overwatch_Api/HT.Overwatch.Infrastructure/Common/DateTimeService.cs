using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Domain.Model;
using HT.Overwatch.Infrastructure.Persistence;

namespace HT.Overwatch.Infrastructure.Common
{
    public class DateTimeService:IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
