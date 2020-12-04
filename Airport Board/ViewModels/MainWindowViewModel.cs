using Airport_Board.Infrastructure.Commands;
using Airport_Board.Models;
using Airport_Board.Services;
using Airport_Board.Services.Interfaces;
using Airport_Board.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private string _timePassed = "0d:00h:00m";

        private TimeSpan _timeSpan = TimeSpan.Zero;
        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Render);
        private double _defaultTimerInterval = 60000;

        private bool _airportStarted;
        private readonly IGetScheduleFromFileService _getScheduleFromFileService;
        private readonly IUserDialogService _userDialog;


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

        public IList<ScheduleRow> Schedule { get; set; }

        #endregion

        public FlightInfoViewModel FlightInfo { get; }
        public CountPassengersInfoViewModel PassengersInfoArrival { get; }
        public CountPassengersInfoViewModel PassengersInfoDeparture { get; }

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
                if (Schedule != null)
                {
                    StartWorkAirport();
                }
                else
                {
                    _userDialog.ShowInformation($"Расписание не загружено. Загрузите расписание.", "Внимание");
                }
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

                    Schedule = schedule;                   
                }
            }
        }
        private bool CanGetFileScheduleCommandExecute(object p) => true;

        #endregion

        public MainWindowViewModel(IGetScheduleFromFileService getScheduleFromFileService,
                                   IUserDialogService userDialog,
                                   FlightInfoViewModel flightInfo,
                                   CountPassengersInfoViewModel passengersInfoArrival,
                                   CountPassengersInfoViewModel passengersInfoDeparture)
        {
            _getScheduleFromFileService = getScheduleFromFileService;
            _userDialog = userDialog;

            FlightInfo = flightInfo;
            PassengersInfoArrival = passengersInfoArrival;
            PassengersInfoDeparture = passengersInfoDeparture;

            StartStopWorkCommand = new RelayCommand(OnStartStopWorkCommandExecuted, CanStartStopWorkCommandExecute);
            GetFileScheduleCommand = new RelayCommand(OnGetFileScheduleCommandExecuted, CanGetFileScheduleCommandExecute);


            Random rnd = new Random();

            dataPoints = new CountPassengers[24];
            for (int i = 0; i < dataPoints.Length; i++)
            {
                dataPoints[i] = new CountPassengers { Arrival = 10, Departure = 10 };
            }            

            TestDataPoints = new ObservableCollection<CountPassengers>(dataPoints);
        }

        CountPassengers[] dataPoints;

        //CountPassengers[] dataPoints;
        //public ObservableCollection<CountPassengers> TestDataPoints { get; set; }
        private ObservableCollection<CountPassengers> _testDataPoints;

        public ObservableCollection<CountPassengers> TestDataPoints
        {
            get => _testDataPoints;
            set => Set(ref _testDataPoints, value);
        }


        private void StartWorkAirport()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(_defaultTimerInterval / Factor);
            _timer.Tick -= Timer_Tick;
            _timer.Tick += Timer_Tick;
            _timer.Start();

            AirportStarted = true;
        }

        private void StopWorkAirport()
        {
            _timer.Tick -= Timer_Tick;
            _timer.Stop();            
            AirportStarted = false;
        }

        private int _dayPassed; 
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Прибавляем время
            _timeSpan = _timeSpan.Add(TimeSpan.FromMilliseconds(_defaultTimerInterval));
            TimePassed = string.Format("{0:D1}d:{1:D2}h:{2:D2}m", //{0:D1}d:{1:D2}h:{2:D2}m:{3:D2}s
                                            _timeSpan.Days,
                                            _timeSpan.Hours,
                                            _timeSpan.Minutes);

            

            // Сравнивая время, находим рейс
            var fligthInfo = Schedule.FirstOrDefault(x =>
            {
                var aircraftTime = x.Time.TimeOfDay;
                var nowTime = _timeSpan.Subtract(TimeSpan.FromDays(_timeSpan.Days)); // Уберем дни из прошедшего времени
                return aircraftTime == nowTime;
            });

            // Если количество дней стало больше с последней проверки, то обнуляем счетчики за последний день
            // Произойдет не ровно в 00:00:01. Зависит от _defaultTimerInterval / Factor
            if (_timeSpan.Days > _dayPassed)
            {
                PassengersInfoArrival.LastDay = 0;
                PassengersInfoDeparture.LastDay = 0;
                _dayPassed = _timeSpan.Days;
            }

            if (fligthInfo != null)
            {
                FlightInfo.UpdateInfo(fligthInfo); // Обновляем инфо о рейсе

                // Обновляем инфо о пассажирах
                if (fligthInfo.Action == FlightActions.Arrival)
                {
                    PassengersInfoArrival.LastFlight = FlightInfo.CountPassengers;
                    PassengersInfoArrival.LastDay += FlightInfo.CountPassengers;
                    

                    if (dataPoints.Length > _timeSpan.Hours)
                    {
                        dataPoints[_timeSpan.Hours].Arrival += FlightInfo.CountPassengers;
                    }

                    
                }
                else
                {
                    PassengersInfoDeparture.LastFlight = FlightInfo.CountPassengers;
                    PassengersInfoDeparture.LastDay += FlightInfo.CountPassengers;

                    if (dataPoints.Length > _timeSpan.Hours)
                    {
                        dataPoints[_timeSpan.Hours].Departure += FlightInfo.CountPassengers;
                    }
                    
                }


                var mxA = dataPoints.Max(x => x.Arrival);
                var mxD = dataPoints.Max(x => x.Departure);
                var mx = Math.Max(mxA, mxD);

                TestDataPoints.Clear();

                var newdp = dataPoints.Select(x => new CountPassengers
                {
                    Scale = 150 / mx,
                    Arrival = x.Arrival,
                    Departure = x.Departure
                });

                foreach (var item in newdp)
                {
                    TestDataPoints.Add(item);
                }


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
