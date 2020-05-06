using CovidPipeline.Covid_API.BAL;
using CovidPipeline.Utils;

namespace CovidPipeline
{
    public class Application
    {
        protected readonly ICovidBLService _covidBLService;
        public Application(ICovidBLService covidBLService)
        {
            _covidBLService = covidBLService;
        }
        public void Run()
        {
            //_covidBLService.InsertLocations();
            _covidBLService.InsertCases(Metrics.CONFIRMED_CASES);
            _covidBLService.InsertCases(Metrics.DEATHS);
            _covidBLService.InsertCases(Metrics.RECOVERIES);
        }

    }
}
