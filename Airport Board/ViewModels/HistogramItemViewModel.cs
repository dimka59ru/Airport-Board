using Airport_Board.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Airport_Board.ViewModels
{
    internal class HistogramItemViewModel: ViewModelBase
    {
        private int _value1;
        private int _value2;
        private double _scale = 1;
        private string _header;

        public string Header
        {
            get => _header;
            set
            {
                Set(ref _header, value);               
            }
        }

        public double Scale
        {
            get => _scale;
            set
            {
                Set(ref _scale, value);
                OnPropertyChanged(nameof(Hight1));
                OnPropertyChanged(nameof(Hight2));
            }
        }
        public int Value1
        {
            get => _value1;
            set
            {
                Set(ref _value1, value);
                OnPropertyChanged(nameof(Hight1));
            }
        }

        public int Value2
        {
            get => _value2;
            set
            {
                Set(ref _value2, value);
                OnPropertyChanged(nameof(Hight2));
            }
        }

        public double Hight1
        {
            get => Value1 == 0 ? 1 : Value1 * Scale;            
        }

        public double Hight2
        {
            get => Value2 == 0 ? 1 : Value2 * Scale;
        }
    }
}
