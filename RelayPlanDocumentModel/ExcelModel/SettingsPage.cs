using ClosedXML.Excel;

namespace RelayPlanDocumentModel
{
    public interface ISettingsPage
    {
        IXLCell? Devicetype { get; set; }
        List<IExcelHmiTable>? HmiTableSections { get; }
        IXLCell? Produktkode { get; set; }
    }

    public class SettingsPage : ISettingsPage
    {
        public SettingsPage() { }
        public SettingsPage(IXLRangeRows pageRows, IXLWorksheet worksheet)
        {
            Produktkode = pageRows.ElementAt(1).Cell(2);
            Devicetype = pageRows.ElementAt(2).Cell(2);

            List<IXLRangeRows> hmiTables = new List<IXLRangeRows>();

            List<int> digsiPathRows = ExcelStaticTools.FindDigsiPathRows(pageRows, 17, false);

            List<int> hmiSectionEndRows = digsiPathRows.Skip(1).Select(r => r - 1).ToList();
            hmiSectionEndRows.Add(pageRows.Last().RowNumber());

            var hmiTableSegments = ExcelStaticTools.GetRowSegments(worksheet, digsiPathRows, hmiSectionEndRows);
            foreach (var hmiTableRows in hmiTableSegments)
            {
                HmiTableSections.Add(new ExcelHmiTable(hmiTableRows, worksheet));
            }
        }
        public IXLCell? Produktkode { get; set; }
        public IXLCell? Devicetype { get; set; }
        public List<IExcelHmiTable>? HmiTableSections { get; private set; } = new List<IExcelHmiTable>();
    }
}

