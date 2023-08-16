using HT.Overwatch.Application.Common.Exceptions;
using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Application.ResponseModels.Common;
using HT.Overwatch.Domain.Model.Metadata;
using MediatR;

namespace HT.Overwatch.Application.Features.Parameters
{
    public class GetParameters
    {
        public class Query : IRequest<IEnumerable<KeyValueResponse>>
        {
            public int SiteId { get; set; }
            public int LocationId { get; set; }
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
                   .Get(x => request.SiteId == x.Location.Site.Id && request.LocationId == x.LocationId);

                if (!data.Any()) throw new NotFoundException("Parameters", request.SiteId);
                //$"Entity \"{name}\" ({key}) was not found."

                return Task.FromResult(data.Select(z => new KeyValueResponse
                {
                    Id = z.ParameterId,
                    Name = z.Parameter.Name
                }).Distinct().AsEnumerable());
            }
        }
    }
}
