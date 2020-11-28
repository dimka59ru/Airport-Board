using Airport_Board.Infrastructure.Commands;
using Airport_Board.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

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

        #region Команды

        #region Команда запуска аэропорта
        public ICommand StartWorkCommand { get; }

        private void OnStartWorkCommandExecuted(object p)
        {
            System.Diagnostics.Debug.WriteLine("Работа аэропорта запущена");
        }

        private bool CanStartWorkCommandExecute(object p) => true;

        #endregion

        #endregion

        public MainWindowViewModel()
        {
            StartWorkCommand = new RelayCommand(OnStartWorkCommandExecuted, CanStartWorkCommandExecute);
        }
    }
}
