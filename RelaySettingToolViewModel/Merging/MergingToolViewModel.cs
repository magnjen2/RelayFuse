using Sip5Library.Sip5TeaxModels.Applications;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using RelayFuseInterfaces;
using RelayPlanDocumentModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RelaySettingToolViewModel
{
    public interface IMergingToolViewModel
    {
        IApplicationNode? ApplicationNode { get; set; }
        ObservableCollection<IHmiTableMergerViewModel> HmiTableMergers { get; set; }
        ICollectionView NonMatchedHmiTables { get; }
        ICommand StartCompareCommand { get; }

        IEnumerable<IHmiTableViewModel> GetUnmatchedHmiTables();
        void InitializeRP(IExcelDevice deviceInExcel);
        void InitializeTeax(IApplicationNode applicationNode);
    }

    public partial class MergingToolViewModel : ViewModelBase, IMergingToolViewModel
    {
        public MergingToolViewModel()
        {
            // Initialize collection hooks and expose the filtered view used by the UI.
            _hmiTableMergers.CollectionChanged += HmiTableMergersCollectionChanged;
            _nonMatchedHmiTablesView = CollectionViewSource.GetDefaultView(_excelHmiTableVMs);
            _nonMatchedHmiTablesView.Filter = FilterNonMatchedTable;
        }

        // Pulls TEAX data so HMI tables can be merged with RP tables.
        public void InitializeTeax(IApplicationNode applicationNode)
        {
            _applicationNode = applicationNode;
            _teaxRelayFuseService = new TeaxRelayFuseService(applicationNode.FunctionalApplication);

            foreach (var teaxHmiTable in _teaxRelayFuseService.GetHmiTables())
            {
                var teaxHmiTableVM = new HmiTableViewModel(teaxHmiTable);
                HmiTableMergers.Add(new HmiTableMergerViewModel(teaxHmiTableVM));
            }
            _teaxOK = true;
        }

        // Loads the RP tables view so unmatched tables can be surfaced and paired.
        public void InitializeRP(IExcelDevice deviceInExcel)
        {
            _selectedDevice = deviceInExcel;
            _excelHmiTableVMs.Clear();
            foreach (var table in _selectedDevice!.SettingPages.SelectMany(x => x.HmiTableSections!))
            {
                var excelHmiTableVM = new HmiTableViewModel(table);
                _excelHmiTableVMs.Add(excelHmiTableVM);
            }
            RefreshNonMatchedView();
            _rpOK = true;
        }

        public IApplicationNode? ApplicationNode
        {
            get => _applicationNode;
            set
            {
                if (_applicationNode != value)
                {
                    _applicationNode = value;
                    OnPropertyChanged(nameof(ApplicationNode));
                }
            }
        }

        private bool _teaxOK;
        private bool _rpOK;
        private IApplicationNode? _applicationNode;
        private IExcelDevice? _selectedDevice;
        private TeaxRelayFuseService? _teaxRelayFuseService;

        private ICommand? _startCompareCommand;
        // Command entry point that kicks off the CompareService when prerequisites are met.
        public ICommand StartCompareCommand => _startCompareCommand ??= new RelayCommand(
            execute: _ => StartCompare(),
            canExecute: _ => CanStartCompare());

        private bool CanStartCompare() => _rpOK && _teaxOK;

        private void StartCompare()
        {
            CompareService.MatchAllHmiTables(this);
        }
    }
}