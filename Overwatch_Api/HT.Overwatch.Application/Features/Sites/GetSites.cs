using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Application.ResponseModels.Common;
using HT.Overwatch.Application.ResponseModels.SiteTimeSeries;
using HT.Overwatch.Domain.Model.Metadata;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HT.Overwatch.Application.Features.Sites
{
    public class GetSites
    {
        public class Query : IRequest<IEnumerable<KeyValueResponse>> 
        {
            public string? Name { get; set; }
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
                var data = _uow.GetRepository<Site>().Get(s => s.Name.ToLower() != "undefined" && (request.Name != null ? s.Name.ToLower().Contains(request.Name.ToLower()) : true), o => o.Name, Common.Enums.SortOrder.Ascending);
                return Task.FromResult(data.Select(site => new KeyValueResponse
                {
                    Id = site.Id,
                    Name = site.Name,
                }).AsEnumerable());
            }
        }
    }

}
