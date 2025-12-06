using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;


namespace RelaySettingToolViewModel
{
    public class MergingToolViewModel : ViewModelBase
    {
        public MergingToolViewModel()
        {

        }
        public MergingToolViewModel(TeaxSideViewModel teaxSideViewModel, RPSideViewModel rPSideViewModel)
        {
            TeaxSideViewModel = teaxSideViewModel;
            RPSideViewModel = rPSideViewModel;

        }


        private TeaxSideViewModel? _teaxSideViewModel;
        public TeaxSideViewModel? TeaxSideViewModel
        {
            get => _teaxSideViewModel;
            set
            {
                if (_teaxSideViewModel != value)
                {
                    _teaxSideViewModel = value;
                    OnPropertyChanged(nameof(TeaxSideViewModel));
                }
            }
        }
        private RPSideViewModel? _rpSectionViewModel;
        public RPSideViewModel? RPSideViewModel
        {
            get => _rpSectionViewModel;
            set
            {
                if (_rpSectionViewModel != value)
                {
                    _rpSectionViewModel = value;
                    OnPropertyChanged(nameof(RPSideViewModel));
                }
            }
        }
        private ICommand? _startCompareCommand;
        public ICommand StartCompareCommand => _startCompareCommand ??= new RelayCommand(
            execute: param => StartCompare(),
            canExecute: param => CanStartCompare()
            );
        private bool CanStartCompare()
        {
            return TeaxSideViewModel != null && RPSideViewModel != null;
        }
        private void StartCompare()
        {
            CompareService.MatchAllHmiTables(TeaxSideViewModel!, RPSideViewModel!);

            var matchedTeaxTables = TeaxSideViewModel!.HmiTableVMs
                .Where(vm => vm.HmiTable != null && vm.HmiTable.MatchingTable != null)
                .ToList();
            var notMatchedTeaxTables = TeaxSideViewModel!.HmiTableVMs
                .Where(vm => vm.HmiTable != null && vm.HmiTable.MatchingTable == null)
                .ToList();
            var notMatchedRPTables = RPSideViewModel!.HmiTableVMs
                .Where(vm => vm.HmiTable != null && vm.HmiTable.MatchingTable == null)
                .ToList();



            foreach(var table in matchedTeaxTables)
            {
                TeaxSideViewModel.DataGridVM!.AddHmiTableRow(table, Colors.LightGreen);
                RPSideViewModel.DataGridVM!.AddHmiTableRow(table.MatchingHmiTableVM!, Colors.LightGreen);


            }

            // TODO: Implement loading matched tables into DataGridVMs
        }


    }
}