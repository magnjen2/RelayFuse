using RelayFuseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaySettingToolViewModel
{
    public interface IRelaySettingViewModel
    {
        IRelaySetting RelaySetting { get; set; }
        bool UniqueIdMatch { get; set; }
        bool DisplayNameMatch { get; set; }
        bool ValueMatch { get; set; }

    }

    public class RelaySettingViewModel : ViewModelBase, IRelaySettingViewModel
    {
        public RelaySettingViewModel(IRelaySetting relaySetting)
        {
            _relaySetting = relaySetting;
        }

        private IRelaySetting _relaySetting;
        public IRelaySetting RelaySetting
        {
            get => _relaySetting;
            set
            {
                _relaySetting = value;
                OnPropertyChanged(nameof(RelaySetting));
            }
        }   

        private bool _uniqueIdMatch;
        public bool UniqueIdMatch
        {
            get
            {
                if (_uniqueIdMatch)
                {
                    Console.WriteLine("debug");
                }
                return _uniqueIdMatch;
            }
            set
            {
                if (_uniqueIdMatch != value)
                {
                    _uniqueIdMatch = value;
                    OnPropertyChanged(nameof(UniqueIdMatch));
                }
            }
        }
        private bool _displayNameMatch;
        public bool DisplayNameMatch
        {
            get => _displayNameMatch;
            set
            {
                if (_displayNameMatch != value)
                {
                    _displayNameMatch = value;
                    OnPropertyChanged(nameof(DisplayNameMatch));
                }
            }
        }
        private bool _valueMatch;
        public bool ValueMatch
        {
            get => _valueMatch;
            set
            {
                if (_valueMatch != value)
                {
                    _valueMatch = value;
                    OnPropertyChanged(nameof(ValueMatch));
                }
            }
        }
    }
}
