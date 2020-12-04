﻿using Airport_Board.Infrastructure.Commands;
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

            for (int i = 0; i < 24; i++)
            {
                TestDataPoints.Add(new HistogramItemViewModel { Header = i.ToString() });
            }
        }

        public ObservableCollection<HistogramItemViewModel> TestDataPoints { get; } = new ObservableCollection<HistogramItemViewModel>();

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

                // Очищаем гистограмму
                foreach (var x in TestDataPoints)
                {
                    x.Scale = 1;
                    x.Value1 = 0;
                    x.Value2 = 0;
                }
            }

            if (fligthInfo != null)
            {
                FlightInfo.UpdateInfo(fligthInfo); // Обновляем инфо о рейсе

                var value1 = TestDataPoints[_timeSpan.Hours].Value1;
                var value2 = TestDataPoints[_timeSpan.Hours].Value2;

                // Обновляем инфо о пассажирах
                if (fligthInfo.Action == FlightActions.Arrival)
                {
                    PassengersInfoArrival.LastFlight = FlightInfo.CountPassengers;
                    PassengersInfoArrival.LastDay += FlightInfo.CountPassengers;
                    value1 += FlightInfo.CountPassengers;  
                }
                else
                {
                    PassengersInfoDeparture.LastFlight = FlightInfo.CountPassengers;
                    PassengersInfoDeparture.LastDay += FlightInfo.CountPassengers;

                    value2 += FlightInfo.CountPassengers; 
                }

                var max = Math.Max(value1, value2);
                var maxItem1 = TestDataPoints.Max(x => x.Value1);
                var maxItem2 = TestDataPoints.Max(x => x.Value2);

                max = Math.Max(max, Math.Max(maxItem1, maxItem2));

                foreach (var x in TestDataPoints)
                {
                    x.Scale = 150.0 / max;
                }
                TestDataPoints[_timeSpan.Hours].Value1 = value1;
                TestDataPoints[_timeSpan.Hours].Value2 = value2;       


                //Debug.WriteLine($"{fligthInfo.Action} - {fligthInfo.AircraftSize} - {fligthInfo.City} - {fligthInfo.Time}");
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
