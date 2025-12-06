using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace RelaySettingToolViewModel
{
    public class DataGridRowViewModel : ViewModelBase
    {
        public IDataGridItem? AssociatedItem { get; set; }
        public bool IsGridOverlay { get; set; } = false;
        public DataGridCellViewModel? OverlayText { get; set; }
        public DataGridCellViewModel? Column1 { get; set; }
        public DataGridCellViewModel? Column2 { get; set; }
        public DataGridCellViewModel? Column3 { get; set; }
        public DataGridCellViewModel? Column4 { get; set; }

        public DataGridCellViewModel[] Cells => new[]
        {
            OverlayText,
            Column1,
            Column2,
            Column3,
            Column4
        }.Where(cell => cell != null).ToArray()!;
    }
}