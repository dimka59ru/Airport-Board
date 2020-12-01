using Airport_Board.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airport_Board.ViewModels
{
    internal class CountPassengersInfoViewModel : ViewModelBase
    {
        private int _lastFligth;
        private int _lastDay;
        private int _all;

        public int LastFlight
        {
            get => _lastFligth;
            set => Set(ref _lastFligth, value);
        }

        public int LastDay
        {
            get => _lastDay;
            set => Set(ref _lastDay, value);
        }

        public int All
        {
            get => _all;
            set => Set(ref _all, value);
        }

    }
}
