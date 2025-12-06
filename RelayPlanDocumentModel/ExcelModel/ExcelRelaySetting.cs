using ClosedXML.Excel;
using RelayFuseInterfaces;

namespace RelayPlanDocumentModel
{
    public interface IExcelRelaySetting : IRelaySetting
    {
        List<string>? AllSelectableValues { get; }
        List<IXLCell> AllSelectableValuesCells { get; set; }
        IXLCell? DisplayNameCell { get; set; }
        IXLCell SelectedValueCell { get; set; }
        IXLCell? UniqueIdCell { get; set; }
        IXLCell? UnitCell { get; set; }
    }

    public class ExcelRelaySetting : IExcelRelaySetting
    {
        public ExcelRelaySetting(IXLRangeRows settingRows, IExcelHmiTable hmiTable)
        {

            _hmiTable = hmiTable;

            // Convert to list and filter out rows where cell H (Cell(8)) is empty
            var filteredRows = settingRows.Where(x => !(string.IsNullOrEmpty(x.Cell(8).GetString())
                                                    && string.IsNullOrEmpty(x.Cell(4).GetString()))).ToList();

            if (filteredRows.Count == 0)
                throw new Exception("No valid setting rows found.");

            UniqueIdCell = filteredRows.First().Cell(2);
            DisplayNameCell = filteredRows.First().Cell(4);
            UnitCell = filteredRows.First().Cell(11);

            if (filteredRows.Count == 1)
            {
                var cellH = filteredRows.First().Cell(8);
                SelectedValueCell = cellH;
                AllSelectableValuesCells.Add(cellH);
                return;
            }

            foreach (IXLRangeRow settingRow in filteredRows)
            {
                var cellGstring = settingRow.Cell(7).GetString(); // Selector-cell
                var cellH = settingRow.Cell(8); // Value-cell

                AllSelectableValuesCells.Add(cellH);

                if (cellGstring.Contains("→") || cellGstring.Contains("®"))
                {
                    SelectedValueCell = cellH;
                }
                else if (!string.IsNullOrEmpty(cellGstring))
                {
                    throw new Exception($"Unexpected content in selector cell: '{cellGstring}'");
                }
            }
        }
        private IExcelHmiTable _hmiTable;
        public IXLCell? UniqueIdCell { get; set; }
        public string UniqueId
        {
            get
            {
                var cellValue = UniqueIdCell?.GetString().Trim(' ') ?? string.Empty;
                var hmiTableValue = _hmiTable?.UniqueId ?? string.Empty;

                if (!string.IsNullOrEmpty(cellValue) && !string.IsNullOrEmpty(hmiTableValue))
                {
                    return hmiTableValue + "." + cellValue;
                }
                return cellValue;
            }
            set
            {
                if (UniqueIdCell != null)
                    UniqueIdCell.Value = value;
            }
        }


        public string DigsiPathString => 
            _hmiTable != null && !string.IsNullOrEmpty(_hmiTable.DigsiPathString) 
            ? _hmiTable.DigsiPathList + "," + DisplayName 
            : string.Empty;


        public IXLCell? DisplayNameCell { get; set; }
        public string DisplayName
        {
            get => DisplayNameCell?.GetString() ?? throw new Exception("DisplayNameCell is null");
            set
            {
                if (DisplayNameCell != null)
                    DisplayNameCell.Value = value;
            }
        }
        public IXLCell? UnitCell { get; set; }
        public string Unit
        {
            get => UnitCell?.GetString() ?? throw new Exception("UnitCell is null");
            set
            {
                if (UnitCell != null)
                    UnitCell.Value = value;
            }
        }
        public IXLCell? SelectedValueCell { get; set; }
        public string SelectedValue
        {
            get => SelectedValueCell?.Value.ToString() ?? string.Empty; 
            
            set
            {
                if (SelectedValueCell != null)
                    SelectedValueCell.Value = value;
            }
        }
        public List<IXLCell>? AllSelectableValuesCells { get; set; } = new List<IXLCell>();
        public List<string> AllSelectableValues
        {
            get => AllSelectableValuesCells?.Select(c => c.GetString()).ToList() ?? throw new Exception("AllSelectableValuesCells is null");
        }

    }
}

