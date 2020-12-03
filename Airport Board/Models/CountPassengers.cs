
using System;
using System.Collections.Generic;
using System.Text;

namespace Airport_Board.Models
{
    internal class CountPassengers
    {
        private double departure;
        private double arrival;

        private const int scale = 1;

        public double Scale { get; set; } = 1;

        public double Departure
        {
            get => departure; 
            set
            {
                departure = value;
                RelativeSizeDeparture = departure * Scale;
            }
        }
        public double Arrival
        {
            get => arrival; 
            set
            {
                arrival = value;
                RelativeSizeArrival = arrival * Scale;
            }
        }

        public double RelativeSizeArrival { get; set; }
        public double RelativeSizeDeparture { get; set; }


    }
}
