using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Application.ResponseModels.QuickLinkResponse;
using HT.Overwatch.Domain.Model;
using MediatR;

namespace HT.Overwatch.Application.Features.QuickLinks
{
    public class GetQuickLinksBySite
    {
        public class Query : IRequest<IEnumerable<QuickLinkResponse>>
        {
            public int SiteId { get; set; }
        }

        public class Handler : IRequestHandler<Query, IEnumerable<QuickLinkResponse>>
        {
            private readonly IUnitOfWork _uow;
            public Handler(IUnitOfWork uow)
            {
                _uow = uow;
            }
            public Task<IEnumerable<QuickLinkResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var data = _uow.GetRepository<QuickLink>().Get(s => s.SiteId == request.SiteId);

                // if (!data.Any()) throw new NotFoundException("QuickLinks", request.SiteId);
                //$"Entity \"{name}\" with key ({key}) was not found."

                return Task.FromResult(data.Select(quickLink => new QuickLinkResponse
                {
                    Id = quickLink.Id,
                    Name = quickLink.Name,
                    Url = quickLink.Url,
                    SiteId = quickLink.SiteId,
                }).AsEnumerable());
            }
        }
    }
}
