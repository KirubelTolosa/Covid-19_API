using Covid_API.BAL.BLDto;
using Covid_API.DAL.DADto;
using CovidPipeline.Covid_API.BAL;
using CovidPipeline.Covid_API.DAL;

namespace CovidPipeline.Utils
{
    public static class Extensions
    {
        public static CovidCaseCountDADto ToCaseCountstDADto(this CovidCaseCountBLDto rec)
        {
            return new CovidCaseCountDADto
            {
                Id = rec.Id,
                Date = rec.Date,
                Count = rec.Count
            };
        }
        public static CovidLocationDADto ToLocationsDADto(this CovidLocationBLDto rec)
        {
            return new CovidLocationDADto
            {
                Country = rec.Country,
                State = rec.State,
                Lat = rec.Lat,
                Long = rec.Long
            };
        }
        public static CovidCaseCountBLDto ToCaseCountsBLDto(this CovidCaseCountDADto rec)
        {
            return new CovidCaseCountBLDto
            {
                Id = rec.Id,
                Date = rec.Date,
                Count = rec.Count
            };
        }
        public static DailyCaseCountsBLDto ToDailyCasesBLDto(this DailyCaseCountsDADto rec)
        {
            return new DailyCaseCountsBLDto
            {
                Date = rec.Date,
                Count = rec.Count
            };
        }
        public static NationalCasesBLDto ToNationalCasesBLDto(this NationalCasesDADto rec)
        {
            return new NationalCasesBLDto
            {
                Country = rec.Country,                
                Count = rec.Count
            };
        }
        public static GlobalTotalCountsBLDto ToGlobalCountsBLDto(this GlobalCaseCountsDADto rec)
        {
            return new GlobalTotalCountsBLDto
            {
                Count = rec.Count
            };
        }

    }
}
