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
        IRelaySettingViewModel? ExcelRelaySettingVM { get; set; }
        int MatchConfidence { get; set; }
        ICommand TakeRPDataCommand { get; }
        ICommand TakeTeaxDataCommand { get; }
        ICommand AttachSettingCommand { get; }
        IRelaySettingViewModel? TeaxRelaySettingVM { get; set; }

    }

    public class SettingMergerViewModel : ViewModelBase, ISettingMergerViewModel
    {
        public SettingMergerViewModel()
        {
            TakeRPDataCommand = new RelayCommand(OnTakeRPData);
            TakeTeaxDataCommand = new RelayCommand(OnTakeTeaxData);
            AttachSettingCommand = new RelayCommand(OnAttachSetting);
        }
        public SettingMergerViewModel(IRelaySettingViewModel teaxRelaySetting) : this()
        {
            TeaxRelaySettingVM = teaxRelaySetting;
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
            if (parameter is IRelaySettingViewModel setting)
            {
                ExcelRelaySettingVM = setting;
                RunSettingMatch();
            }
        }
        private void RunSettingMatch()
        {
            if (TeaxRelaySettingVM == null || ExcelRelaySettingVM == null)
                return;

            CompareService.TryMatchSetting(this, ExcelRelaySettingVM);
        }




        private IRelaySettingViewModel? _teaxRelaySetting;
        public IRelaySettingViewModel? TeaxRelaySettingVM
        {
            get => _teaxRelaySetting ?? null;
            set
            {
                _teaxRelaySetting = value;
                OnPropertyChanged(nameof(TeaxRelaySettingVM));
            }
        }
        private IRelaySettingViewModel? _excelRelaySetting;
        public IRelaySettingViewModel? ExcelRelaySettingVM
        {
            get => _excelRelaySetting ?? null;
            set
            {
                _excelRelaySetting = value;
                OnPropertyChanged(nameof(ExcelRelaySettingVM));
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
