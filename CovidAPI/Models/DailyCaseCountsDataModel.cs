using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CovidAPI.Models
{
    public class DailyCaseCountsDataModel
    {        
        public string Date { get; set; }
        public int Count { get; set; }
    }
}