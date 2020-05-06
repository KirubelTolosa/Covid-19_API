using Microsoft.OData.Edm;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CovidAPI.Models
{
    public class CasesCountDataModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }   
}