using Airport_Board.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airport_Board.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private int _minFactor = 1;
        private int _maxFactor = 10000;        
        private string _buttonStartContent = "Старт";        
        private string _timeNow = "11:20";

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

        public string ButtonStartContent
        {
            get => _buttonStartContent;
            set => Set(ref _buttonStartContent, value);
        }

        public string TimeNow
        {
            get => _timeNow;
            set => Set(ref _timeNow, value);
        }
    }
}
