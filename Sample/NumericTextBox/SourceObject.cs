using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NumericTextBox
{
    public class SourceObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public double DoubleValue
        {
            get { return _doubleValue; }
            set
            {
                if(_doubleValue == value) { return; }
                _doubleValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoubleValue)));
            }
        }
        private double _doubleValue;

        public double IntValue
        {
            get { return _intValue; }
            set
            {
                if(_intValue == value) { return; }
                _intValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IntValue)));
            }
        }
        private double _intValue;
    }
}
