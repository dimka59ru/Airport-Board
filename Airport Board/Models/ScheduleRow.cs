using System;
using System.Collections.Generic;
using System.Text;

namespace Airport_Board.Models
{
    internal class ScheduleRow
    {
        public AircraftSizes AircraftSize { get; set; }

        public FlightActions @Action { get; set; }

        public DateTime Time { get; set; }

        public string City { get; set; }
    }
}
