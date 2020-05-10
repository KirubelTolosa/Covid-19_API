using Covid_API.BAL.BLDto;
using Covid_API.DAL.DADto;
using CovidPipeline.Covid_API.BAL;
using CovidPipeline.Utils;
using System;
using System.Collections.Generic;

namespace CovidPipeline.Covid_API.DAL
{
    public interface ICovidDataRepository
    {
        void InsertLocations(List<CovidLocationBLDto> bALRecords);
        void InsertCases(List<CovidCaseCountBLDto> bALRecords, Metrics metrics);        
        DateTime GetLastUpdateDate(Metrics metrics);
        List<NationalCasesDADto> GetCountOfCasesForAllNations(Metrics metrics);
        List<NationalCasesDADto> GetCountOfCasesByCountry(Metrics metrics, string Country, DateTime? Date);
        List<DailyCaseCountsDADto> GetDailyCaseCountsByCountry(Metrics metrics, string Country);
        List<GlobalTotalCountsDADto> GetGlobalTotalCounts(Metrics metrics);
        Dictionary<string, int> GetLocationTableCompositeKeys();
    }
}
