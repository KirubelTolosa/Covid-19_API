using CovidPipeline.Covid_API.BAL;
using CovidPipeline.Utils;
using CovidAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Amazon.Glacier;

namespace CovidAPI.Controllers
{
    [RoutePrefix("api/Covid")]
    public class CovidController : ApiController
    {
        private ICovidBLService _covidBLService;
        public CovidController(ICovidBLService covidBLService)
        {
            this._covidBLService = covidBLService;
        }

        // GET: api/Covid
        //[Route("")]
        public string[] Get()
        {
            return new string[] { "api/covid/Confirmed_Cases", "api/covid/Deaths", "api/covid/Recoveries", "api/covid/{metrics}/GlobalCount", "api/covid/{metrics}/Country/{Date : Optional}", "api/covid/{metrics}/Country/DailyCount"};
        }

        [Route("{metrics}")]
        public IEnumerable<NationalCasesDataModel> Get([FromUri] Metrics metrics)
        {
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;
            var errorResponse = Request.CreateErrorResponse(statusCode, "Resource not found!");           
            return (metrics == Metrics.CONFIRMED_CASES) ?
                (_covidBLService.GetCountOfCasesForAllNations(Metrics.CONFIRMED_CASES).Select(Item => new NationalCasesDataModel() { Count = Item.Count, Country = Item.Country }).ToList()) : (metrics == Metrics.DEATHS) ?
                (_covidBLService.GetCountOfCasesForAllNations(Metrics.DEATHS).Select(Item => new NationalCasesDataModel() { Count = Item.Count, Country = Item.Country }).ToList()) : (metrics == Metrics.RECOVERIES) ?
                _covidBLService.GetCountOfCasesForAllNations(Metrics.RECOVERIES).Select(Item => new NationalCasesDataModel() { Count = Item.Count, Country = Item.Country }).ToList() : throw new HttpResponseException(errorResponse); ;
        }

        [Route("{metrics}/GlobalCount")]
        public IEnumerable<GlobalTotalCountsDataModel>GetGlobalCaseCounts([FromUri] Metrics metrics)
        {
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;
            var errorResponse = Request.CreateErrorResponse(statusCode, "Resource not found!");          
            return (metrics == Metrics.CONFIRMED_CASES) ?
                    _covidBLService.GetGlobalTotalCounts(Metrics.CONFIRMED_CASES).Select(Item => new GlobalTotalCountsDataModel() { GlobalCases = "Confirmed", Count = Item.Count}).ToList() 
                    : (metrics == Metrics.DEATHS) ?
                    _covidBLService.GetGlobalTotalCounts(Metrics.DEATHS).Select(Item => new GlobalTotalCountsDataModel() { GlobalCases = "Deaths", Count = Item.Count}).ToList()
                    : (metrics == Metrics.RECOVERIES) ?
                    _covidBLService.GetGlobalTotalCounts(Metrics.RECOVERIES).Select(Item => new GlobalTotalCountsDataModel() { GlobalCases = "Recoveries", Count = Item.Count}).ToList()
                    : throw new HttpResponseException(errorResponse); ;                               
        }
       
        [Route("{metrics}/{Country}/{dateTime:DateTime?}")]
        public List<NationalCasesDataModel> GetCountryCaseCounts([FromUri] Metrics metrics, string Country, DateTime? dateTime = null)
        {
            if (dateTime == null)
                dateTime = DateTime.MinValue;
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;
            var errorResponse = Request.CreateErrorResponse(statusCode, "Resource not found!");
            return (metrics == Metrics.CONFIRMED_CASES) ? _covidBLService.GetCountOfCasesByCountry(Metrics.CONFIRMED_CASES, Country, dateTime).Select(Item => new NationalCasesDataModel() { Country = Item.Country, Count = Item.Count}).ToList() : (metrics == Metrics.DEATHS) ?
                _covidBLService.GetCountOfCasesByCountry(Metrics.DEATHS, Country, dateTime).Select(Item => new NationalCasesDataModel() {Country = Item.Country, Count = Item.Count}).ToList() : (metrics == Metrics.RECOVERIES) ?
                _covidBLService.GetCountOfCasesByCountry(Metrics.RECOVERIES, Country, dateTime).Select(Item => new NationalCasesDataModel() { Country = Item.Country, Count = Item.Count}).ToList() : throw new HttpResponseException(errorResponse); ;
        }

        [Route("{metrics}/{Country}/DailyCount")]
        public List<DailyCaseCountsDataModel> GetDailyCountOfCasesByCountry([FromUri] Metrics metrics, string Country)
        {
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;
            var errorResponse = Request.CreateErrorResponse(statusCode, "Resource not found!");
            return (metrics == Metrics.CONFIRMED_CASES) ? _covidBLService.GetDailyCaseCountsByCountry(Metrics.CONFIRMED_CASES, Country).Select(Item => new DailyCaseCountsDataModel() { Date = Item.Date.ToShortDateString(), Count = Item.Count }).ToList() : (metrics == Metrics.DEATHS) ?
                _covidBLService.GetDailyCaseCountsByCountry(Metrics.DEATHS, Country).Select(Item => new DailyCaseCountsDataModel() { Date = Item.Date.ToShortDateString(), Count = Item.Count }).ToList() : (metrics == Metrics.RECOVERIES) ?
                _covidBLService.GetDailyCaseCountsByCountry(Metrics.RECOVERIES, Country).Select(Item => new DailyCaseCountsDataModel() { Date = Item.Date.ToShortDateString(), Count = Item.Count }).ToList() : throw new HttpResponseException(errorResponse); ;
        }
        
        /*
         GET: api/Covid/5
        public IEnumerable<Models.NationalCases> Get(string metrics)
        {
            return (metrics == "ConfirmedCases") ?
                (_covidBLService.GetNationalCaseCounts(Metrics.CONFIRMED_CASES).Select(Item => new Models.NationalCases() { Count = Item.Count, Country = Item.Country }).ToList()) : (metrics == "Deaths") ?
                (_covidBLService.GetNationalCaseCounts(Metrics.DEATHS).Select(Item => new Models.NationalCases() { Count = Item.Count, Country = Item.Country }).ToList()) :
                (_covidBLService.GetNationalCaseCounts(Metrics.RECOVERIES).Select(Item => new Models.NationalCases() { Count = Item.Count, Country = Item.Country }).ToList());
        }

        // POST: api/Covid
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Covid/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Covid/5
        public void Delete(int id)
        {
        }*/
    }
}
