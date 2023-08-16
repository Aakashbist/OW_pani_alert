using HT.Overwatch.Application.Interfaces;
using HT.Overwatch.Application.ResponseModels.Timeseries;
using HT.Overwatch.Domain.Model.Metadata;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HT.Overwatch.Application.Features.Timeseries
{
    public class GetTimeseries
    {
        public class Query : IRequest<IEnumerable<TimeseriesResponse>>
        {
        }
        public class Handler : IRequestHandler<Query, IEnumerable<TimeseriesResponse>>
        {
            private readonly IUnitOfWork _uow;

            public Handler(IUnitOfWork uow)
            {
                _uow = uow;
            }

            public Task<IEnumerable<TimeseriesResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var data = _uow.GetRepository<TimeSeries>()
                    .Get(x => x.LocationId != 0 && x.ParameterId != 0)
                    .Include(ts => ts.Location)
                         .ThenInclude(idl => idl.Site)
                             .ThenInclude(ids => ids.Region)
                    .Include(ts => ts.Parameter)
                    .OrderBy(ts => ts.Id);

                var result = Task.FromResult(data.Select(ts => new TimeseriesResponse
                {
                    Metric = string.Format("{0}/{1}/{2} ({3})",
                                               ts.Location.Site.Name,
                                               ts.Location.Name,
                                               ts.Parameter.Name,
                                               ts.Parameter.UnitOfMeasure),
                    VariableId = ts.Id,
                    VariableName = ts.Name,
                    SiteId = ts.Location.Site.Id,
                    SiteName = ts.Location.Site.Name,
                    LocationId = ts.Location.Id,
                    LocationName = ts.Location.Name,
                    ParamaterId = ts.Parameter.Id,
                    ParamaterName = ts.Parameter.Name,
                    ParameterUnits = ts.Parameter.UnitOfMeasure
                }).AsEnumerable());

                return result;
            }

        }
    }
}
