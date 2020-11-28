using System;
using System.Collections.Generic;
using System.Text;

namespace Airport_Board.Models
{
    internal class ScheduleRow
    {
        public Aircrafts Aircraft { get; set; }

        public Actions Action { get; set; }

        public TimeSpan Time { get; set; }

        public string City { get; set; }
    }
}
