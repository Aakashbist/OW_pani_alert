
using HT.Overwatch.Domain.Model;
using HT.Overwatch.Domain.Model.TimeSeriesStorage;

namespace HT.Overwatch.Integration.Test;

public static class TestData
{
    public static readonly string Locations = "At Dam,At Dam Leakage Weir,Power station";
    public static readonly string Parameters = "Water Leve,Channel Level,Turbidity,Turbidity High Alert,Power Sation Discharge,Forecast Inflow Hi,Forecast Inflow Low,Forecast Inflow Med";
    public static readonly string Sites = "Clark Dam,Bradys Dam,Tungatinah Power Station";

    public static List<Region> GetRegions()
    {
        return new List<Region>
        {
            Region.CreateRegion(1,"Tasmania",null),
            Region.CreateRegion(2,"Victoria",null),
        };
    }

    public static List<Site> GetSites()
    {
        return new List<Site>
        {
           Site.CreateSite(1,1,"Clark Dam",null),
           Site.CreateSite(2,1,"Bradys Dam",null),
           Site.CreateSite(4,1,"Tungatinah Power Station",null),
        };
    }

    public static List<Parameter> GetParameters()
    {
        return new List<Parameter>
        {
           Parameter.CreateParameter(1,"Bore Water Level",null,"meter",null),
           Parameter.CreateParameter(2,"Joint Displacement",null,"mm",null),
           Parameter.CreateParameter(3,"Water Level",null,"meter",null),
           Parameter.CreateParameter(4,"Rainfall",null,"mm",null),
           Parameter.CreateParameter(5,"Channel Level",null,"meter",null),
           Parameter.CreateParameter(6,"Forecast Inflow Hi",null,"m3/s",null),
           Parameter.CreateParameter(7,"Forecast Inflow Low",null,"m3/s",null),
           Parameter.CreateParameter(8,"Forecast Inflow Med",null,"m3/s",null),
           Parameter.CreateParameter(9,"Air Temperature Forecast",null,"Degree C",null),
           Parameter.CreateParameter(10,"Turbidity",null,"NTU",null),
           Parameter.CreateParameter(11,"Turbidity High Alert",null,"NTU",null),
           Parameter.CreateParameter(12,"Power Station Discharge",null,"m3/s",null),
        };
    }

    public static List<Location> GetLocations()
    {
        return new List<Location>
        {
           Location.CreateLocation(1,"P01-AGD",1,"Clark Dam","datum",44.09987,-345.989,4.0),
           Location.CreateLocation(2,"P02-AGD",1,"Clark Dam","datum",44.09987,-345.989,4.0),
           Location.CreateLocation(3,"P03-AGD",1,"Clark Dam","datum",44.09987,-345.989,4.0),
           Location.CreateLocation(4,"P04-AGD",1,"Clark Dam","datum",44.09987,-345.989,4.0),
           Location.CreateLocation(5,"P05-AGD",1,"Clark Dam","datum",44.09987,-345.989,4.0),
           Location.CreateLocation(11,"At Dam",2,"Bradys Dam","datum",44.09987,-345.989,4.0),
           Location.CreateLocation(13,"At Dam Leakage Weir",2,"Bradys Dam","datum",44.09987,-345.989,4.0),
           Location.CreateLocation(18,"Power Station",4,null,"datum",44.09987,-345.989,4.0),

        };
    }

    public static List<QuickLink> GetQuickLinks()
    {
        return new List<QuickLink>
        {
          QuickLink.CreateQuickLink(1, 1, "https://hydrotasmania.sharepoint.com/sites/IntraH/Aboutus/Ourbusiness/DamSafety/Pages/Emergency.aspx", "Emergency"),
          QuickLink.CreateQuickLink(2, 1, "http://images.hydrotasmania.com.au/", "HT Image Browser"),
          QuickLink.CreateQuickLink(4, 2, "http://damsafetyreports/reports/Spillway/DSEP/StoragesInflowFcastReport.pdf", "Storage Inflow Forest Report"),
          QuickLink.CreateQuickLink(5, 2, "https://ecm0.hydrotasmania.com.au/otcs/cs.exe/Open/8199791", "Recommendation Database"),
          QuickLink.CreateQuickLink(6, 2, "https://ecm0.hydrotasmania.com.au/otcs/cs.exe/Open/8199791", "Recommendation Database"),
          QuickLink.CreateQuickLink(1000, 4, "https://www.someurl.com", "Quick Link Test 1"),
          QuickLink.CreateQuickLink(2000, 4, "https://www.someurl.com", "Quick Link Test 2"),
          QuickLink.CreateQuickLink(3000, 4, "https://www.someurl.com", "Quick Link Test 3"),
          QuickLink.CreateQuickLink(5000, 4, "https://www.someurl.com", "Quick Link Test 4"),
          QuickLink.CreateQuickLink(6000, 4, "https://www.someurl.com", "Quick Link Test 5"),
        };
    }

    /*
     For testing purpose 
        Locations are ["At Dam", "At Dam Leakage Weir","Power station"];
        Parameters are ["Water Level", "Channel Level","Turbidity", "Turbidity High Alert","Power Sation Discharge","Forecast Inflow Hi","Forecast Inflow Low","Forecast Inflow Med"];
        Sites are ["Clark Dam","Bradys Dam","Tungatinah Power Station" ];
        And generates around 28 hours of data
     */

    public static List<TimeSeries> GetTimeSeries()
    {
        return new List<TimeSeries>
        {
          //TimeSeries.CreateTimeSeries(3,"",null,2,1),
          //TimeSeries.CreateTimeSeries(4,"",null,1,1),
          //TimeSeries.CreateTimeSeries(5,"",null,3,1),
          //TimeSeries.CreateTimeSeries(6,"",null,5,1),
          //TimeSeries.CreateTimeSeries(12,"",null,10,2),
          TimeSeries.CreateTimeSeries(13,"Fault Incident",null,11,3),
          TimeSeries.CreateTimeSeries(15,"Primary",null,13,5),
          TimeSeries.CreateTimeSeries(17,"Secondary",null,11,6),
          TimeSeries.CreateTimeSeries(18,"Primary",null,11,7),
          TimeSeries.CreateTimeSeries(19,"Primary",null,11,8),
          TimeSeries.CreateTimeSeries(27,"Fault Incident",null,13,10),
          TimeSeries.CreateTimeSeries(28,"Fault Incident",null,13,11),
          TimeSeries.CreateTimeSeries(34,"Fault Incident",null,18,12),
        };
    }

    public static List<TimeSeriesValue> GetTimeSeriesValues()
    {
        return PrepareTimeSeriesValues();

    }

    private static List<TimeSeriesValue> PrepareTimeSeriesValues()
    {
        List<TimeSeriesValue> result = new List<TimeSeriesValue>();
        int[] TimeSeriesIds = new int[] { 13, 15, 17, 18, 19, 27, 28, 34 };
        int dataAmount = 320; // this should alway be multiple of 10 of TimeseriesIds.length 

        int dataPerSiteAmount = dataAmount / TimeSeriesIds.Length;

        DateTime currentTime = DateTime.Now;

        DateTime startingTime = currentTime.AddSeconds(-currentTime.Second).AddMilliseconds(-currentTime.Millisecond);

        DateTime[] startingTimes = new DateTime[dataPerSiteAmount];

        Random rnd = new Random();

        for (int i = 0; i < dataPerSiteAmount; i++)
        {
            startingTimes[i] = startingTime;

            startingTime = startingTime.AddMinutes(45);
        }

        for (int i = 0; i < dataAmount; i++)
        {
            result.Add(TimeSeriesValue.CreateTimeSeriesValue(startingTimes[i % dataPerSiteAmount].ToUniversalTime(), TimeSeriesIds[i / dataPerSiteAmount], rnd.Next(300), 140, 4));
            result.Add(TimeSeriesValue.CreateTimeSeriesValue(startingTimes[i % dataPerSiteAmount].AddMinutes(15).ToUniversalTime(), TimeSeriesIds[i / dataPerSiteAmount], rnd.Next(300), 140, 4));
            result.Add(TimeSeriesValue.CreateTimeSeriesValue(startingTimes[i % dataPerSiteAmount].AddMinutes(30).ToUniversalTime(), TimeSeriesIds[i / dataPerSiteAmount], rnd.Next(300), 140, 4));
        }

        return result;
    }

    public static List<Contract.DTO.RowFilterOptions> CreateRowFilterOptions()
    {

        return new List<Contract.DTO.RowFilterOptions>
                {
                    new Contract.DTO.RowFilterOptions
                    {
                        Site = new Contract.DTO.Site
                        {
                            Id=2,
                            Name="Bradys Dam"
                        },
                        Location = new Contract.DTO.Location
                        {
                            Id=11,
                             Name="At Dam"
                        },
                        Parameter = new Contract.DTO.Parameter
                        {
                            Id=3,
                            Name = "Water Level"},
                        Variable = new Contract.DTO.Variable
                        {
                            Id=13,
                            Name = "Fault Incident"
                        },
                    },
                    new Contract.DTO.RowFilterOptions
                    {
                        Site = new Contract.DTO.Site
                        {
                            Id=2,
                            Name="Bradys Dam"
                        },
                        Location = new Contract.DTO.Location
                        {
                            Id=13,
                             Name="At Dam Leakage Weir"
                        },
                        Parameter = new Contract.DTO.Parameter
                        {
                            Id=5,
                            Name = "Channel Level"},
                        Variable = new Contract.DTO.Variable
                        {
                            Id=15,
                            Name = "Primary"
                        },
                    }
                };
    }


}
