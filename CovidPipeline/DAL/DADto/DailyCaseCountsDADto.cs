using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covid_API.DAL.DADto
{
    public class DailyCaseCountsDADto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
