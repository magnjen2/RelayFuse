using RelayFuseInterfaces;
using RelayPlanDocumentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace RelaySettingToolViewModel
{
    public interface IHmiTableMergerViewModel
    {
        IExcelHmiTable? ExcelHmiTable { get; set; }
        ObservableCollection<IRelaySetting> NonMatchedSettings { get; }
        ICollectionView NonMatchedSettingsView { get; }
        ICommand RefreshCommand { get; }
        ICommand AttachHmiTableCommand { get; }
        ObservableCollection<SettingMergerViewModel> SettingMergers { get; set; }
        ITeaxHmiTable? TeaxHmiTable { get; set; }
        int[] MatchConfidence { get; set; }
    }

    public partial class HmiTableMergerViewModel : ViewModelBase, IHmiTableMergerViewModel
    {
        public HmiTableMergerViewModel()
        {
            RefreshCommand = new RelayCommand(OnRefresh);
            AttachHmiTableCommand = new RelayCommand(OnAttachHmiTable);
            InitializeCollections();
        }
        public HmiTableMergerViewModel(ITeaxHmiTable teaxHmiTable) : this()
        {
            TeaxHmiTable = teaxHmiTable;
            foreach (var setting in teaxHmiTable.Settings)
            {
                SettingMergers.Add(new SettingMergerViewModel(setting));
            }
        }

        public ICommand RefreshCommand { get; } = null!;
        public ICommand AttachHmiTableCommand { get; } = null!;
        private void OnRefresh(object? obj)
        {
        }

        private ITeaxHmiTable? _teaxHmiTable;
        public ITeaxHmiTable? TeaxHmiTable
        {
            get => _teaxHmiTable ?? null;
            set
            {
                _teaxHmiTable = value;
                OnPropertyChanged(nameof(TeaxHmiTable));
            }
        }
        private IExcelHmiTable? _excelHmiTable;
        public IExcelHmiTable? ExcelHmiTable
        {
            get => _excelHmiTable ?? null;
            set
            {
                if (_excelHmiTable == value)
                    return;

                _excelHmiTable = value;
                RecalculateNonMatchedSettings();
                OnPropertyChanged(nameof(ExcelHmiTable));
            }
        }

        private void OnAttachHmiTable(object? parameter)
        {
            if (parameter is IExcelHmiTable table)
            {
                ExcelHmiTable = table;
                MatchTable();
            }
        }

        private int[] _matchConfidence = new int[2] { 0, 0 };
        public int[] MatchConfidence
        {
            get => _matchConfidence;
            set
            {
                _matchConfidence = value;
                OnPropertyChanged(nameof(MatchConfidence));
            }
        }
        private void MatchTable()
        {
            if(ExcelHmiTable is null)
                return;

            MatchConfidence[0] = 1;
            CompareService.MatchAllSettings(this, ExcelHmiTable, MatchConfidence);
        }
    }
}