using System;
using System.Collections.Generic;
using System.Text;

namespace Airport_Board.Models
{
    internal class CountPassengers
    {
        private int lastFlight;

        public int LastFlight
        {
            get => lastFlight;
            set
            {
                lastFlight = value;
                All += lastFlight;
            }
        }

        public int LastDay { get; set; }

        public int All { get; private set; }
    }
}
