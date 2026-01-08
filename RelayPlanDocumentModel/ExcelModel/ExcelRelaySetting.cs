using ClosedXML.Excel;
using RelayFuseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RelayPlanDocumentModel
{
    public interface IExcelRelaySetting : IRelaySetting
    {
        IReadOnlyList<string> AllSelectableValues { get; }
        IReadOnlyList<IXLCell> AllSelectableValuesCells { get; }
        IXLCell DisplayNameCell { get; }
        IXLCell SelectedValueCell { get; }
        IXLCell UniqueIdCell { get; }
        IXLCell UnitCell { get; }
    }

    public class ExcelRelaySetting : IExcelRelaySetting
    {
        public ExcelRelaySetting(IXLRangeRows settingRows, IExcelHmiTable hmiTable)
        {
            ArgumentNullException.ThrowIfNull(settingRows);
            _hmiTable = hmiTable ?? throw new ArgumentNullException(nameof(hmiTable));

            var filteredRows = settingRows
                .Where(x => !(string.IsNullOrEmpty(x.Cell(8).GetString()) && string.IsNullOrEmpty(x.Cell(4).GetString())))
                .ToList();

            if (filteredRows.Count == 0)
                throw new InvalidOperationException("No valid setting rows found.");

            UniqueIdCell = filteredRows.First().Cell(2);
            DisplayNameCell = filteredRows.First().Cell(4);
            UnitCell = filteredRows.First().Cell(11);

            if (filteredRows.Count == 1)
            {
                var cellH = filteredRows.First().Cell(8);
                SelectedValueCell = cellH ?? throw new InvalidOperationException("Selected value cell not found in single-row setting.");
                _allSelectableValuesCells.Add(cellH);
                return;
            }

            bool selectedValueFound = false;
            foreach (IXLRangeRow settingRow in filteredRows)
            {
                var cellGstring = settingRow.Cell(7).GetString(); // Selector-cell
                var cellH = settingRow.Cell(8); // Value-cell

                _allSelectableValuesCells.Add(cellH);

                if (cellGstring.Contains("→") || cellGstring.Contains("®"))
                {
                    //if (selectedValueFound)
                    //{
                    //    throw new InvalidOperationException($"Multiple selected values detected for setting '{DisplayNameCell.GetString()}'.");
                    //}

                    SelectedValueCell = cellH;
                    selectedValueFound = true;
                }
                else if (!string.IsNullOrEmpty(cellGstring))
                {
                    throw new InvalidOperationException($"Unexpected content in selector cell: '{cellGstring}'.");
                }
            }

            if (!selectedValueFound)
            {
                var cellH = filteredRows.First().Cell(8);
                SelectedValueCell = cellH ?? throw new InvalidOperationException("Selected value cell not found in single-row setting.");
                _allSelectableValuesCells.Add(cellH);
                return;
            }
        }

        private readonly List<IXLCell> _allSelectableValuesCells = new List<IXLCell>();
        private readonly IExcelHmiTable _hmiTable;
        private IXLCell? _selectedValueCell;

        public IReadOnlyList<string> AllSelectableValues => _allSelectableValuesCells.Select(cell => cell.GetString()).ToArray();
        public IReadOnlyList<IXLCell> AllSelectableValuesCells => _allSelectableValuesCells;

        public IXLCell DisplayNameCell { get; private set; }
        public string DisplayName
        {
            get => DisplayNameCell?.GetString() ?? throw new InvalidOperationException("DisplayNameCell is null");
            set => DisplayNameCell.Value = value;
        }

        public IXLCell UnitCell { get; private set; }
        public string Unit
        {
            get => UnitCell?.GetString() ?? throw new InvalidOperationException("UnitCell is null");
            set => UnitCell.Value = value;
        }

        public IXLCell SelectedValueCell
        {
            get => _selectedValueCell ?? throw new InvalidOperationException("SelectedValueCell is not initialized.");
            private set => _selectedValueCell = value ?? throw new ArgumentNullException(nameof(value));
        }
        public string SelectedValue
        {
            get => SelectedValueCell.GetString();
            set => SelectedValueCell.Value = value;
        }

        public IXLCell UniqueIdCell { get; private set; }
        public string UniqueId
        {
            get
            {
                var cellValue = UniqueIdCell?.GetString().Trim(' ') ?? throw new InvalidOperationException("UniqueIdCell is not initialized.");
                var hmiTableValue = _hmiTable?.UniqueId ?? string.Empty;

                if (!string.IsNullOrEmpty(cellValue) && !string.IsNullOrEmpty(hmiTableValue))
                {
                    return hmiTableValue + "." + cellValue;
                }
                return cellValue;
            }
            set => UniqueIdCell.Value = value;
        }

        public string DigsiPathString =>
            _hmiTable != null && !string.IsNullOrEmpty(_hmiTable.DigsiPathString)
            ? string.Join(",", _hmiTable.DigsiPathList ?? Array.Empty<string>()) + "," + DisplayName
            : string.Empty;
    }
}

