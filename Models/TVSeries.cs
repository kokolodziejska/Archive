﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archive.Models
{
    public class TVSeries
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
    }
}
