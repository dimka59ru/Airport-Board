using Airport_Board.Infrastructure.Commands;
using Airport_Board.Models;
using Airport_Board.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using System.Windows.Threading;

namespace Airport_Board.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private int _minFactor = 1;
        private int _maxFactor = 10000;        
        private double _factor = 1.0;
        
        private string _timePassed = "0d:00h:00m:00s";

        private TimeSpan _timeSpan = TimeSpan.Zero;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private double _defaultTimerInterval = 1000;

        private bool _airportStarted;

        private Airport _airport;

        public bool AirportStarted
        {
            get => _airportStarted;
            set
            {
                Set(ref _airportStarted, value);
                OnPropertyChanged(nameof(ButtonStartStopContent));
            }
        }

        public int MinFactor
        {
            get => _minFactor;
            set => Set(ref _minFactor, value);
        }

        public int MaxFactor
        {
            get => _maxFactor;
            set => Set(ref _maxFactor, value);
        }

        public double Factor
        {
            get => _factor;
            set
            {
                Set(ref _factor, value);
                ChangeTimerInterval();
            }
        }

        public string ButtonStartStopContent => AirportStarted ? "Стоп" : "Старт";        

        public string TimePassed
        {
            get => _timePassed;
            set => Set(ref _timePassed, value);
        }

        #region Команды

        #region Команда запуска аэропорта
        public ICommand StartStopWorkCommand { get; }

        private void OnStartStopWorkCommandExecuted(object p)
        {
            if (AirportStarted)
            {
                StopWorkAirport();
            }
            else
            {
                StartWorkAirport();
            }
        }

        private bool CanStartStopWorkCommandExecute(object p) => true;

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            StartStopWorkCommand = new RelayCommand(OnStartStopWorkCommandExecuted, CanStartStopWorkCommandExecute);


            //var options = new JsonSerializerOptions
            //{
            //    WriteIndented = true,
            //    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            //};

            // Читаем
            var jsonString = File.ReadAllText(@"C:\Users\Дмитрий\OneDrive\Рабочий стол\1.json");
            var schedule = JsonSerializer.Deserialize<List<ScheduleRow>>(jsonString);

            _airport = new Airport
            {
                Schedule = schedule
            };
        }


        private void StartWorkAirport()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(_defaultTimerInterval/Factor);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            AirportStarted = true;
        }

        private void StopWorkAirport()
        {
            _timer.Stop();
            AirportStarted = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Прибавляем время
            _timeSpan = _timeSpan.Add(TimeSpan.FromSeconds(1)); 
            TimePassed = string.Format("{0:D1}d:{1:D2}h:{2:D2}m:{3:D2}s",
                                            _timeSpan.Days,
                                            _timeSpan.Hours,
                                            _timeSpan.Minutes,
                                            _timeSpan.Seconds);

            // Сравнивая время, находим самолет
            var aircraft = _airport.Schedule.FirstOrDefault(x =>
            {
                var aircraftTime = x.Time.TimeOfDay;
                var nowTime = _timeSpan.Subtract(TimeSpan.FromDays(_timeSpan.Days)); // Уберем дни из прошедшего времени
                return aircraftTime == nowTime;
            });

            if (aircraft != null)
            {
                Debug.WriteLine($"{aircraft.Action} - {aircraft.AircraftSize} - {aircraft.City} - {aircraft.Time}");
            }
        }

        private void ChangeTimerInterval()
        {
            _timer.Stop();
            _timer.Interval = TimeSpan.FromMilliseconds(_defaultTimerInterval/Factor);
            _timer.Start();
        }
    }
}
