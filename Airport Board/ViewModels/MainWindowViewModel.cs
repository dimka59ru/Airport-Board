using Airport_Board.Infrastructure.Commands;
using Airport_Board.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

namespace Airport_Board.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private int _minFactor = 1;
        private int _maxFactor = 10000;        
        private double _factor = 1.0;
        
        private string _timeNow = "0d:00h:00m:00s";

        private TimeSpan _timeSpan = TimeSpan.Zero;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private double _defaultTimerInterval = 1000;

        private bool _airportStarted;
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

        public string TimeNow
        {
            get => _timeNow;
            set => Set(ref _timeNow, value);
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
            _timeSpan = _timeSpan.Add(TimeSpan.FromSeconds(1)); 
            TimeNow = string.Format("{0:D1}d:{1:D2}h:{2:D2}m:{3:D2}s",
                                            _timeSpan.Days,
                                            _timeSpan.Hours,
                                            _timeSpan.Minutes,
                                            _timeSpan.Seconds); 
        }

        private void ChangeTimerInterval()
        {
            _timer.Stop();
            _timer.Interval = TimeSpan.FromMilliseconds(_defaultTimerInterval/Factor);
            _timer.Start();
        }
    }
}
