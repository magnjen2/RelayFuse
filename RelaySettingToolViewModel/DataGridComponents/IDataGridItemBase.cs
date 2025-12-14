using System.Windows.Media;

namespace RelaySettingToolViewModel
{
    public interface IDataGridItemBase
    {
        Color Color { get; set; }
        bool IsGridOverlay { get; set; }
        DataGridCellViewModel? OverlayText { get; set; }
        DataGridCellViewModel? Column1 { get; set; }
        DataGridCellViewModel? Column2 { get; set; }
        DataGridCellViewModel? Column3 { get; set; }
        DataGridCellViewModel? Column4 { get; set; }
        DataGridCellViewModel[] Cells { get; }
    }
    public class DataGridItemBase : IDataGridItemBase
    {
        public Color Color { get; set; }
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
