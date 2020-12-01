using Airport_Board.Infrastructure.Commands;
using Airport_Board.Models;
using Airport_Board.Services;
using Airport_Board.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
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

        private readonly IGetScheduleFromFileService _getScheduleFromFileService;


        #region Properties
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

        #endregion

        public FlightInfoViewModel FlightInfo { get; } = new FlightInfoViewModel();
        public CountPassengersInfoViewModel PassengersInfoArrival { get; } = new CountPassengersInfoViewModel();
        public CountPassengersInfoViewModel PassengersInfoDeparture { get; } = new CountPassengersInfoViewModel();

        #region Команды

        #region  StartStopWorkCommand Команда запуска аэропорта
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

        #region GetFileScheduleCommand Команда загрузки файла расписания

        #endregion
        public ICommand GetFileScheduleCommand { get; }

        private void OnGetFileScheduleCommandExecuted(object p)
        {
            string filePath;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;

            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                if (filePath.Length != 0)
                {
                    // todo: try catch                    

                    var _getScheduleFromFileService = new GetScheduleFromJsonFileService();
                    var schedule = _getScheduleFromFileService.GetScheduleFromFile(filePath).ToList();

                    _airport = new Airport
                    {
                        Schedule = schedule
                    };
                }
            }
        }
        private bool CanGetFileScheduleCommandExecute(object p) => true;

        #endregion

        public MainWindowViewModel(IGetScheduleFromFileService getScheduleFromFileService)
        {
            _getScheduleFromFileService = getScheduleFromFileService;
            
            StartStopWorkCommand = new RelayCommand(OnStartStopWorkCommandExecuted, CanStartStopWorkCommandExecute);
            GetFileScheduleCommand = new RelayCommand(OnGetFileScheduleCommandExecuted, CanGetFileScheduleCommandExecute);                  
        }


        private void StartWorkAirport()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(_defaultTimerInterval / Factor);
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
            if (_airport == null)
            {
                // Расписание не загружено
                //
                StopWorkAirport();
                return;
            }

            // Прибавляем время
            _timeSpan = _timeSpan.Add(TimeSpan.FromSeconds(1));
            TimePassed = string.Format("{0:D1}d:{1:D2}h:{2:D2}m:{3:D2}s",
                                            _timeSpan.Days,
                                            _timeSpan.Hours,
                                            _timeSpan.Minutes,
                                            _timeSpan.Seconds);
            

            // Сравнивая время, находим самолет
            var fligthInfo = _airport.Schedule.FirstOrDefault(x =>
            {
                var aircraftTime = x.Time.TimeOfDay;
                var nowTime = _timeSpan.Subtract(TimeSpan.FromDays(_timeSpan.Days)); // Уберем дни из прошедшего времени
                return aircraftTime == nowTime;
            });

            if (fligthInfo != null)
            {
                FlightInfo.UpdateInfo(fligthInfo); // Обновляем инфо о рейсе

                if (fligthInfo.Action == Actions.Arrival)
                {
                    _airport.CountPassengersArrival.LastFlight = FlightInfo.CountPassengers;
                    
                }
                else
                {
                    _airport.CountPassengersDeparture.LastFlight = FlightInfo.CountPassengers;
                }

                PassengersInfoArrival.LastDay = _airport.CountPassengersArrival.LastFlight;

                Debug.WriteLine($"{fligthInfo.Action} - {fligthInfo.AircraftSize} - {fligthInfo.City} - {fligthInfo.Time}");
            }
        }

        private void ChangeTimerInterval()
        {
            _timer.Stop();
            _timer.Interval = TimeSpan.FromMilliseconds(_defaultTimerInterval / Factor);
            _timer.Start();
        }
    }
}
