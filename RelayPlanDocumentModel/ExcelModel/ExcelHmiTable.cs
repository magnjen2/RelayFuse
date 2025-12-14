using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using RelayFuseInterfaces;

namespace RelayPlanDocumentModel
{

    public interface IExcelHmiTable : IHmiTable
    {
        IXLCell? DigsiPathCell { get; set; }
        IXLCell? UniqueIdCell { get; set; }
    }

    public class ExcelHmiTable : IExcelHmiTable
    {
        public ExcelHmiTable() { }
        public ExcelHmiTable(IXLRangeRows hmiTableRows, IXLWorksheet worksheet)
        {
            UniqueIdCell = hmiTableRows.First().Cell(2);
            DigsiPathCell = hmiTableRows.First().Cell(4);

            // Find all row indices cell D has content, starting from row 2
            var rowsList = hmiTableRows.ToList();
            var startIndices = new List<int>();

            for (int i = 1; i < rowsList.Count; i++) // start from row nr.2 (index 1)
            {
                if (!string.IsNullOrWhiteSpace(rowsList[i].Cell(4).GetString()))
                {
                    startIndices.Add(i);
                }
            }

            // Build IXLRangeRows for each segment
            var settingRanges = new List<IXLRangeRows>();
            for (int i = 0; i < startIndices.Count; i++)
            {
                int start = startIndices[i];
                int end = (i + 1 < startIndices.Count) ? startIndices[i + 1] - 1 : rowsList.Count - 1;

                // Get the actual row numbers in the worksheet
                int wsStartRow = rowsList[start].RowNumber();
                int wsEndRow = rowsList[end].RowNumber();

                var settingRangeRows = worksheet.Range(wsStartRow, 1, wsEndRow, 17).Rows();
                settingRanges.Add(settingRangeRows);
            }

            foreach (var settingRange in settingRanges)
            {
                Settings.Add(new ExcelRelaySetting(settingRange, this));
            }


        }
        public List<IRelaySetting>? Settings { get; set; } = new List<IRelaySetting>();

        public IXLCell? UniqueIdCell { get; set; }
        public string? UniqueId
        {
            get
            {
                var value = UniqueIdCell?.GetString();
                if (value == null) return null;
                var cleaned = value.Trim().Replace(" ", "").Replace("=>", "");
                return cleaned.EndsWith(".") ? cleaned[..^1] : cleaned;
            }

        }
        private IXLCell? _digsiPathCell;
        public IXLCell? DigsiPathCell
        {
            get => _digsiPathCell;
            set
            {
                _digsiPathCell = value;
            }
        }
        public string? DigsiPathString
        {
            get
            {
                return _digsiPathCell?.GetString() ?? string.Empty;
            }
        }
        public string[]? DigsiPathList
        {
            get
            {
                // Split and trim each part
                return DigsiPathString!
                    .Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();
            }
            set
            {
                if (_digsiPathCell != null)
                    _digsiPathCell.Value = string.Join(", ", value);
            }
        }


    }
}

