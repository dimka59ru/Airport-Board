using System.Collections.Generic;

namespace Airport_Board.Models
{
    internal class Airport
    {
        public IList<ScheduleRow> Schedule { get; set; }
        public CountPassengers CountPassengersArrival { get; set; }
        public CountPassengers CountPassengersDeparture { get; set; }

    }
}
