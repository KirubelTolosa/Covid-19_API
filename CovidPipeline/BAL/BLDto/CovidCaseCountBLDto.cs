using System;

namespace CovidPipeline.Covid_API.BAL
{
    public class CovidCaseCountBLDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }    
}
