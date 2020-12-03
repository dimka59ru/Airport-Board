using Airport_Board.Models;
using Airport_Board.ViewModels.Base;
using FontAwesome5;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airport_Board.ViewModels
{
    internal class FlightInfoViewModel : ViewModelBase
    {
        ScheduleRow _fligthInfo;
        private string _action;
        private string _city;
        private string _aircraftSize;
        private string _time;
        private string _icon;
        private int _countPassengers;

        public string @Action
        {
            get => _action;
            set => Set(ref _action, value);
        }

        public string City
        {
            get => _city;
            set => Set(ref _city, value);
        }

        public string AircraftSize
        {
            get => _aircraftSize;
            set => Set(ref _aircraftSize, value);
        }

        public string Time
        {
            get => _time;
            set => Set(ref _time, value);
        }


        public string FontAwesomeIcon
        {
            get => _icon;
            set => Set(ref _icon, value);
        }

        public int CountPassengers
        {
            get => _countPassengers;
            set => Set(ref _countPassengers, value);
        }

        public FlightInfoViewModel()
        {
        }

        public void UpdateInfo(ScheduleRow scheduleRow)
        {
            _fligthInfo = scheduleRow;

            @Action = _fligthInfo.Action.ToString();
            City = _fligthInfo.City;
            AircraftSize = _fligthInfo.AircraftSize.ToString();

            Time = string.Format("{0:D2}:{1:D2}", 
                                _fligthInfo.Time.Hour,
                                _fligthInfo.Time.Minute);

            if (_fligthInfo.Action == FlightActions.Arrival)
            {
                FontAwesomeIcon = "Solid_PlaneArrival";
            }
            else
            {
                FontAwesomeIcon = "Solid_PlaneDeparture";
            }

            Random rnd = new Random();
            // Генерируем количество пассажиров
            if (_fligthInfo.AircraftSize == AircraftSizes.Small)
            {
                CountPassengers = rnd.Next(1, 50);
            }
            else if (_fligthInfo.AircraftSize == AircraftSizes.Middle)
            {
                CountPassengers = rnd.Next(50, 100);
            }
            else if (_fligthInfo.AircraftSize == AircraftSizes.Big)
            {
                CountPassengers = CountPassengers = rnd.Next(100, 200);
            }
        }       
    }
}
