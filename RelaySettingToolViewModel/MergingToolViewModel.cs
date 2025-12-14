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
            if (TeaxSideViewModel!.DataGridVM == null || RPSideViewModel!.DataGridVM == null) return;

            TeaxSideViewModel!.DataGridVM!.ClearGrid();
            RPSideViewModel!.DataGridVM!.ClearGrid();

            CompareService.MatchAllHmiTables(TeaxSideViewModel!, RPSideViewModel!);

            var matchedTeaxTables = TeaxSideViewModel!.HmiTableVMs
                .Where(vm => vm.HmiTable != null && vm.MatchingHmiTableVM != null)
                .ToList();
            var notMatchedTeaxTables = TeaxSideViewModel!.HmiTableVMs
                .Where(vm => vm.HmiTable != null && vm.MatchingHmiTableVM == null)
                .ToList();
            var notMatchedRPTables = RPSideViewModel!.HmiTableVMs
                .Where(vm => vm.HmiTable != null && vm.MatchingHmiTableVM == null)
                .ToList();



            foreach(var table in matchedTeaxTables)
            {
                TeaxSideViewModel.DataGridVM!.AddHmiTableRow(table, Colors.Green);
                RPSideViewModel.DataGridVM!.AddHmiTableRow(table.MatchingHmiTableVM!, Colors.Green);

                foreach(var setting in table.SettingViewModels)
                {
                    var matchedSetting = setting.MatchingSettingVM;
                    if(matchedSetting != null)
                    {
                        TeaxSideViewModel.DataGridVM!.AddSettingRow(setting, Colors.Green);
                        RPSideViewModel.DataGridVM!.AddSettingRow(matchedSetting, Colors.Green);
                    }
                    if(matchedSetting == null)
                    {
                        TeaxSideViewModel.DataGridVM!.AddSettingRow(setting, Colors.LightYellow);
                        RPSideViewModel.DataGridVM!.AddEmptyRow();
                    }
                }

            }

            foreach(var table in notMatchedTeaxTables)
            {
                TeaxSideViewModel.DataGridVM!.AddHmiTableRow(table, Colors.LightCoral);
                RPSideViewModel.DataGridVM!.AddEmptyRow();
                foreach(var setting in table.SettingViewModels)
                {
                    TeaxSideViewModel.DataGridVM!.AddSettingRow(setting, Colors.LightYellow);
                    RPSideViewModel.DataGridVM!.AddEmptyRow();
                }
            }
            foreach(var table in notMatchedRPTables)
            {
                TeaxSideViewModel.DataGridVM!.AddEmptyRow();
                RPSideViewModel.DataGridVM!.AddHmiTableRow(table, Colors.LightCoral);
                foreach(var setting in table.SettingViewModels)
                {
                    TeaxSideViewModel.DataGridVM!.AddEmptyRow();
                    RPSideViewModel.DataGridVM!.AddSettingRow(setting, Colors.LightYellow);
                }
            }

        }


    }
}