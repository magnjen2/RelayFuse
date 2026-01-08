using RelayFuseInterfaces;
using RelayPlanDocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RelaySettingToolViewModel
{
    public interface ISettingMergerViewModel
    {
        IRelaySetting? ExcelRelaySetting { get; set; }
        int MatchConfidence { get; set; }
        ICommand TakeRPDataCommand { get; }
        ICommand TakeTeaxDataCommand { get; }
        ICommand AttachSettingCommand { get; }
        IRelaySetting? TeaxRelaySetting { get; set; }
        bool UniqueIdMatch { get; set; }
        bool DisplayNameMatch { get; set; }
        bool ValueMatch { get; set; }

    }

    public class SettingMergerViewModel : ViewModelBase, ISettingMergerViewModel
    {
        public SettingMergerViewModel()
        {
            TakeRPDataCommand = new RelayCommand(OnTakeRPData);
            TakeTeaxDataCommand = new RelayCommand(OnTakeTeaxData);
            AttachSettingCommand = new RelayCommand(OnAttachSetting);
        }
        public SettingMergerViewModel(IRelaySetting teaxRelaySetting) : this()
        {
            TeaxRelaySetting = teaxRelaySetting;
        }

        public ICommand TakeRPDataCommand { get; }
        public ICommand TakeTeaxDataCommand { get; }
        public ICommand AttachSettingCommand { get; }

        private void OnTakeRPData(object? obj)
        {
            // TODO: Implement TakeRP logic
        }

        private void OnTakeTeaxData(object? obj)
        {
            // TODO: Implement TakeTeax logic
        }

        private void OnAttachSetting(object? parameter)
        {
            if (parameter is IRelaySetting setting)
            {
                ExcelRelaySetting = setting;
                RunSettingMatch();
            }
        }
        private void RunSettingMatch()
        {
            if (TeaxRelaySetting == null || ExcelRelaySetting == null)
                return;

            CompareService.TryMatchSetting(this, ExcelRelaySetting);
            ValueMatch = TeaxRelaySetting.SelectedValue == ExcelRelaySetting.SelectedValue;
        }




        private IRelaySetting? _teaxRelaySetting;
        public IRelaySetting? TeaxRelaySetting
        {
            get => _teaxRelaySetting ?? null;
            set
            {
                _teaxRelaySetting = value;
                OnPropertyChanged(nameof(TeaxRelaySetting));
            }
        }
        private IRelaySetting? _excelRelaySetting;
        public IRelaySetting? ExcelRelaySetting
        {
            get => _excelRelaySetting ?? null;
            set
            {
                _excelRelaySetting = value;
                OnPropertyChanged(nameof(ExcelRelaySetting));
            }
        }

        private bool _uniqueIdMatch = false;
        public bool UniqueIdMatch
        {
            get => _uniqueIdMatch;
            set
            {
                _uniqueIdMatch = value;
                OnPropertyChanged(nameof(UniqueIdMatch));
            }
        }
        private bool _displayNameMatch = false;
        public bool DisplayNameMatch
        {
            get => _displayNameMatch;
            set
            {
                _displayNameMatch = value;
                OnPropertyChanged(nameof(DisplayNameMatch));
            }
        }
        private bool _valueMatch = false;
        public bool ValueMatch
        {
            get => _valueMatch;
            set
            {
                _valueMatch = value;
                OnPropertyChanged(nameof(ValueMatch));
            }
        }

        private int _matchConfidence;
        public int MatchConfidence
        {
            get => _matchConfidence;
            set
            {
                _matchConfidence = value;
                OnPropertyChanged(nameof(MatchConfidence));
            }
        }
    }
}
