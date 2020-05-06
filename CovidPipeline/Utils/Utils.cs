using Covid_API.BAL.BLDto;
using Covid_API.DAL.DADto;
using CovidPipeline.Covid_API.BAL;
using CovidPipeline.Covid_API.DAL;
using System.Collections.Generic;

namespace CovidPipeline.Utils
{
    public class Utilities
    {
        public static List<CovidCaseCountDADto> MapCaseCountsBLDTOtoDADTO(List<CovidCaseCountBLDto> bLRecords)
        {
            List<CovidCaseCountDADto> dALRecords = new List<CovidCaseCountDADto>();
            foreach (var rec in bLRecords)
            {
                dALRecords.Add(rec.ToCaseCountstDADto());
            }
            return dALRecords;
        }
        public static List<CovidLocationDADto> MapLocationsBLDTOtoDADTO(List<CovidLocationBLDto> bLRecords)
        {
            List<CovidLocationDADto> dALRecords = new List<CovidLocationDADto>();
            foreach (var rec in bLRecords)
            {
                dALRecords.Add(rec.ToLocationsDADto());
            }
            return dALRecords;
        }
        public static List<CovidCaseCountBLDto> MapCasecountsDADTotoBLDto(List<CovidCaseCountDADto> bLRecords)
        {
            List<CovidCaseCountBLDto> bLLRecords = new List<CovidCaseCountBLDto>();
            foreach (var rec in bLRecords)
            {
                bLLRecords.Add(rec.ToCaseCountsBLDto());
            }
            return bLLRecords;
        }
        public static List<NationalCasesBLDto> MapNationalCasesDADTOtoBLDTO(List<NationalCasesDADto> dALRecords)
        {
            List<NationalCasesBLDto> bLLRecords = new List<NationalCasesBLDto>();
            foreach (var rec in dALRecords)
            {
                bLLRecords.Add(rec.ToNationalCasesBLDto());
            }
            return bLLRecords;
        }
        public static List<DailyCaseCountsBLDto> MapDailyCasesDADTOtoBLDTO(List<DailyCaseCountsDADto> dALRecords)
        {            
            List<DailyCaseCountsBLDto> bLLRecords = new List<DailyCaseCountsBLDto>();
            foreach (var rec in dALRecords)
            {                
                bLLRecords.Add(rec.ToDailyCasesBLDto());
            }
            return bLLRecords;
        }

        public static List<GlobalTotalCountsBLDto> MapGlobalTotalCountsDADTOtoBLDTO(List<GlobalTotalCountsDADto> dALRecords)
        {
            List<GlobalTotalCountsBLDto> bLLRecords = new List<GlobalTotalCountsBLDto>();
            foreach (var rec in dALRecords)
            {
                //Extension method not working - Review 
                GlobalTotalCountsBLDto tempRec = new GlobalTotalCountsBLDto
                {
                    Count = rec.Count
                };
                bLLRecords.Add(tempRec);
            }
            return bLLRecords;
        }

    }
}
