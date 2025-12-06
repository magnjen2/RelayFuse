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
        public ObservableCollection<DataGridRowViewModel> GridRows { get; } = new();
        public void ClearGrid() => GridRows.Clear();

        public void AddHmiTableRow(IHmiTableViewModel hmiTableVM, Color rowColor)
        {
            // Add one DigsiPath row per HmiTableViewModel
            GridRows.Add(new DataGridRowViewModel
            {
                IsGridOverlay = true,
                OverlayText = new DataGridCellViewModel { Content = hmiTableVM.HmiTable.DigsiPathString!, 
                                                          CellColor = rowColor ,
                                                          AssociatedItem = hmiTableVM},
                AssociatedItem = hmiTableVM
            });

        }
        public void AddSettingRow(IRelaySettingViewModel settingVM, Color cellColor)
        {
            // Add RelaySetting row

            var newSettingRow = new DataGridRowViewModel
            {
                IsGridOverlay = false,
                Column1 = new DataGridCellViewModel { Content = settingVM.RelaySetting.UniqueId, CellColor = cellColor, AssociatedItem = settingVM },
                Column2 = new DataGridCellViewModel { Content = settingVM.RelaySetting.DisplayName, CellColor = cellColor, AssociatedItem = settingVM },
                Column3 = new DataGridCellViewModel { Content = settingVM.RelaySetting.SelectedValue, CellColor = cellColor, AssociatedItem = settingVM },
                Column4 = new DataGridCellViewModel { Content = settingVM.RelaySetting.Unit, CellColor = cellColor, AssociatedItem = settingVM }
            };

            GridRows.Add(newSettingRow);
           
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
