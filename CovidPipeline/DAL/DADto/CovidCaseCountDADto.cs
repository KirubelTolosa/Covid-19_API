using System;

namespace CovidPipeline.Covid_API.DAL
{
    public class CovidCaseCountDADto
    {
        public string dbCompositeKey { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }    
}
