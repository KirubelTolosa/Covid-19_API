﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covid_API.BAL.BLDto
{
    public class DailyCaseCountsBLDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
