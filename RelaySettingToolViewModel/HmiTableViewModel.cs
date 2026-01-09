using RelayFuseInterfaces;

namespace RelaySettingToolViewModel
{
    public interface IHmiTableViewModel
    {
        IHmiTable HmiTable { get; set; }
        List<IRelaySettingViewModel> RelaySettingViewModels { get; set; }
    }

    public class HmiTableViewModel : ViewModelBase, IHmiTableViewModel
    {
        public HmiTableViewModel(IHmiTable hmiTable)
        {
            _hmiTable = hmiTable;
            _relaySettingViewModels = _hmiTable.Settings.Select(s => new RelaySettingViewModel(s) as IRelaySettingViewModel).ToList();

        }

        private IHmiTable _hmiTable;
        public IHmiTable HmiTable
        {
            get => _hmiTable;
            set
            {
                _hmiTable = value;
                OnPropertyChanged(nameof(HmiTable));
            }
        }

        private List<IRelaySettingViewModel> _relaySettingViewModels;
        public List<IRelaySettingViewModel> RelaySettingViewModels
        {
            get => _relaySettingViewModels;
            set
            {
                _relaySettingViewModels = value;
                OnPropertyChanged(nameof(RelaySettingViewModels));
            }
        }


    }
}
