using RelayPlanDocumentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RelaySettingToolViewModel
{
    public class DataGridViewModel
    {
        public ObservableCollection<IDataGridItemBase> GridRows { get; } = new();
        public void ClearGrid() => GridRows.Clear();

        public void AddHmiTableRow(IHmiTableViewModel hmiTableVM, Color rowColor)
        {
            // Add one DigsiPath row per HmiTableViewModel


            hmiTableVM.IsGridOverlay = true;
            hmiTableVM.OverlayText = new DataGridCellViewModel
            {
                Content = hmiTableVM.HmiTable.DigsiPathString!,
                CellColor = rowColor,
                AssociatedItem = hmiTableVM
            };
            GridRows.Add(hmiTableVM);

        }
        public void AddSettingRow(IRelaySettingViewModel settingVM, Color cellColor)
        {

            settingVM.IsGridOverlay = false;
            settingVM.Column1 = new DataGridCellViewModel { Content = settingVM.RelaySetting.UniqueId, CellColor = cellColor, AssociatedItem = settingVM };
            settingVM.Column2 = new DataGridCellViewModel { Content = settingVM.RelaySetting.DisplayName, CellColor = cellColor, AssociatedItem = settingVM };
            settingVM.Column3 = new DataGridCellViewModel { Content = settingVM.RelaySetting.SelectedValue, CellColor = cellColor, AssociatedItem = settingVM };
            settingVM.Column4 = new DataGridCellViewModel { Content = settingVM.RelaySetting.Unit, CellColor = cellColor, AssociatedItem = settingVM };

            GridRows.Add(settingVM);

        }
        public void AddEmptyRow()
        {
            var emptyRow = new DataGridItemBase
            {
                IsGridOverlay = false,
                Column1 = new DataGridCellViewModel { Content = string.Empty, CellColor = Colors.Transparent },
                Column2 = new DataGridCellViewModel { Content = string.Empty, CellColor = Colors.Transparent },
                Column3 = new DataGridCellViewModel { Content = string.Empty, CellColor = Colors.Transparent },
                Column4 = new DataGridCellViewModel { Content = string.Empty, CellColor = Colors.Transparent }
            };
            GridRows.Add(emptyRow);
        }

        public void LoadMultipleTables(IEnumerable<IHmiTableViewModel> hmiTableVMs)
        {
            foreach (IHmiTableViewModel hmiTableVM in hmiTableVMs.Where(x => x.SettingViewModels != null))
            {
                if (string.IsNullOrEmpty(hmiTableVM.HmiTable.DigsiPathString))
                {
                    throw new Exception("Digsipath not available.");
                }
                // Add one DigsiPath row per HmiTableViewModel
                AddHmiTableRow(hmiTableVM, Colors.LightGray);


                // Add RelaySetting rows
                foreach (var settingVM in hmiTableVM.SettingViewModels)
                {
                    AddSettingRow(settingVM, Colors.Transparent);
                }
            }
        }

    }
}
