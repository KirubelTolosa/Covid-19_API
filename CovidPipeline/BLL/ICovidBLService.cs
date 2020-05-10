using Covid_API.BAL.BLDto;
using CovidPipeline.Utils;
using System;
using System.Collections.Generic;

namespace CovidPipeline.Covid_API.BAL
{
    public interface ICovidBLService
    {
        List<CovidCaseCountBLDto> FetchCasesFromAPI(Metrics metrics);
        List<CovidCaseCountBLDto> FetchCasesFromFile(Metrics metrics);
        (List<CovidLocationBLDto>, string[]) FetchLocationsFromFile();
        void InsertLocations();
        void InsertCases(Metrics metrics);
        List<NationalCasesBLDto> GetCountOfCasesForAllNations(Metrics metrics);
        List<NationalCasesBLDto> GetCountOfCasesByCountry(Metrics metrics, string Country, DateTime? Date);
        List<GlobalTotalCountsBLDto> GetGlobalTotalCounts(Metrics metrics);
        List<DailyCaseCountsBLDto> GetDailyCaseCountsByCountry(Metrics metrics, string Country);


    }
}
