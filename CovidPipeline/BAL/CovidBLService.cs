using Covid_API.BAL.BLDto;
using CovidPipeline.Covid_API.BAL;
using CovidPipeline.Covid_API.DAL;
using CovidPipeline.Utils;
using CsvHelper;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;

namespace CovidPipeline.BAL
{
    public class CovidBLService : ICovidBLService
    {
        private string path = ConfigurationManager.AppSettings["Data_LocalCopy"];
        private ICovidDataRepository covidDataRepository;        

        public CovidBLService(ICovidDataRepository covidDataRepository)
        {
            this.covidDataRepository = covidDataRepository;
        }
        public List<CovidCaseCountBLDto> FetchCasesFromAPI(Metrics metrics)
        {
            WebRequest request = WebRequest.Create(ConfigurationManager.AppSettings["covidConfirmedCasesAPI"]);
            if (metrics == Metrics.DEATHS)
            {
                request = WebRequest.Create(ConfigurationManager.AppSettings["covidDeathsAPI"]);
            }
            else if (metrics == Metrics.RECOVERIES)
            {
                request = WebRequest.Create(ConfigurationManager.AppSettings["covidRecoveriesAPI"]);
            }
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader readerStream = new StreamReader(dataStream);
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            string responseFromServer = readerStream.ReadToEnd();
            //Console.WriteLine(responseFromServer);
            response.Close();

            TextReader reader = new StringReader(responseFromServer);
            CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            dynamic records = csvReader.GetRecords<dynamic>();
            List<CovidCaseCountBLDto> bALCaseCountRecords = new List<CovidCaseCountBLDto>();
            int id = 1;
            Date latestDate = covidDataRepository.GetLastUpdateDate(metrics).Date;
            foreach (var rec in records)
            {
                IDictionary<string, object> caseCounts = rec;

                foreach (var count in caseCounts)
                {
                    if (count.Key != "Province/State" && count.Key != "Country/Region" && count.Key != "Lat" && count.Key != "Long" && DateTime.Compare(DateTime.ParseExact(count.Key, "M/d/yy", System.Globalization.CultureInfo.InvariantCulture), latestDate) > 0)
                    {

                        CovidCaseCountBLDto caseRecord = new CovidCaseCountBLDto
                        {
                            Id = id,
                            Date = DateTime.ParseExact(count.Key, "M/d/yy", System.Globalization.CultureInfo.InvariantCulture),
                            Count = Convert.ToInt32(count.Value)                            
                        };
                        
                        bALCaseCountRecords.Add(caseRecord);
                    }
                }
                id++;
            }
            return bALCaseCountRecords;
        }
        public List<CovidCaseCountBLDto> FetchCasesFromFile(Metrics metrics)
        {
            //Needs fixing 
            TextReader reader = new StreamReader(path);
            CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            dynamic records = csvReader.GetRecords<dynamic>();
            var bALRecords = FetchLocationsFromFile().Item1;
            var headerRow = FetchLocationsFromFile().Item2;
            List<CovidCaseCountBLDto> bALCaseCountRecords = new List<CovidCaseCountBLDto>();
            int id = 1;
            Date latestDate = covidDataRepository.GetLastUpdateDate(metrics).Date;
            foreach (var rec in records)
            {
                IDictionary<string, object> caseCounts = rec;
                foreach (var cases in caseCounts)
                {
                    if (cases.Key != "State" && cases.Key != "Country" && cases.Key != "Lat" && cases.Key != "Long" && DateTime.Compare((DateTime.ParseExact(cases.Key, "M/d/yy", System.Globalization.CultureInfo.InvariantCulture)), latestDate) > 0)
                    {
                        CovidCaseCountBLDto caseRecord = new CovidCaseCountBLDto
                        {
                            Id = id,
                            Date = DateTime.ParseExact(cases.Key, "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture),
                            Count = Convert.ToInt32(cases.Value)
                        };
                        bALCaseCountRecords.Add(caseRecord);
                    }
                }
                id++;
            }
            return bALCaseCountRecords;
        }
        public (List<CovidLocationBLDto>, string[]) FetchLocationsFromFile()
        {
            TextReader reader = new StreamReader(path);
            CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            var csvRecords = csvReader.GetRecords<CovidLocationBLDto>();
            csvReader.Read();
            csvReader.ReadHeader();
            string[] headerRow = csvReader.Context.HeaderRecord;
            List<CovidLocationBLDto> bALRecords = new List<CovidLocationBLDto>();
            foreach (var rec in csvRecords)
            {
                bALRecords.Add(rec);
            }
            return (bALRecords, headerRow);
        }
        public void InsertLocations()
        {
            covidDataRepository.InsertLocations(FetchLocationsFromFile().Item1);
        }
        public void InsertCases(Metrics metrics)
        {
            covidDataRepository.InsertCases(FetchCasesFromAPI(metrics), metrics);
        }
        public List<NationalCasesBLDto> GetCountOfCasesForAllNations(Metrics metrics)
        {            
            return Utils.Utilities.MapNationalCasesDADTOtoBLDTO(covidDataRepository.GetCountOfCasesForAllNations(metrics));
        }        
        public List<NationalCasesBLDto> GetCountOfCasesByCountry(Metrics metrics, string Country, DateTime? Date)
        {
            return Utils.Utilities.MapNationalCasesDADTOtoBLDTO(covidDataRepository.GetCountOfCasesByCountry(metrics, Country, Date));
        }
        public List<GlobalTotalCountsBLDto> GetGlobalTotalCounts(Metrics metrics)
        {
            return Utils.Utilities.MapGlobalTotalCountsDADTOtoBLDTO(covidDataRepository.GetGlobalTotalCounts(metrics));
        }
        public List<DailyCaseCountsBLDto> GetDailyCaseCountsByCountry(Metrics metrics, string Country)
        {
            return Utils.Utilities.MapDailyCasesDADTOtoBLDTO(covidDataRepository.GetDailyCaseCountsByCountry(metrics, Country));
        }
    }
}
