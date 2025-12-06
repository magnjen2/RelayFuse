using System.Windows.Media;

namespace RelaySettingToolViewModel
{
    public interface IDataGridItem
    {
        Color Color { get; set; }
        DataGridCellViewModel? AssociatedCell { get; set; }
    }
}
