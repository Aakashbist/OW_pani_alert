using HT.Overwatch.Application.Common.Exceptions;
using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Application.ResponseModels.Common;
using HT.Overwatch.Domain.Model.Metadata;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HT.Overwatch.Application.Features.Locations
{
    public class GetLocations
    {
        public class Query : IRequest<IEnumerable<KeyValueResponse>>
        {
            public int SiteId { get; set; }
        }


        public class Handler : IRequestHandler<Query, IEnumerable<KeyValueResponse>>
        {
            private readonly IUnitOfWork _uow;
            public Handler(IUnitOfWork uow)
            {
                _uow = uow;
            }
            public Task<IEnumerable<KeyValueResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var data = _uow.GetRepository<TimeSeries>()
                 .Get(x => request.SiteId == x.Location.Site.Id)
                 .Include(x => x.Location);

                if (!data.Any()) throw new NotFoundException("Locations", request.SiteId);
                //$"Entity \"{name}\" ({key}) was not found."

                return Task.FromResult(data.Select(z => new KeyValueResponse
                {
                    Id = z.LocationId,
                    Name = z.Location.Name
                }).Distinct().AsEnumerable());
            }
        }
    }
}
