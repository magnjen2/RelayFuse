using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RelayPlanDocumentModel
{
    public interface ISettingsPage
    {
        IXLCell? Devicetype { get; set; }
        List<IExcelHmiTable> HmiTableSections { get; }
        IXLCell? Produktkode { get; set; }
    }

    public class SettingsPage : ISettingsPage
    {
        public SettingsPage() { }
        public SettingsPage(IXLRangeRows pageRows, IXLWorksheet worksheet)
        {
            ArgumentNullException.ThrowIfNull(pageRows);
            ArgumentNullException.ThrowIfNull(worksheet);

            if (!pageRows.Any())
            {
                throw new InvalidOperationException("SettingsPage cannot be created from an empty pageRows collection.");
            }
            if (pageRows.Last().RowNumber() - pageRows.First().RowNumber() < 10)
            {
                throw new InvalidOperationException("SettingsPage requires at least 10 rows.");
            }
            if (pageRows.Any(x => x.RowNumber() == 875))
            {
                Console.WriteLine("debug");
            }


            Produktkode = pageRows.ElementAt(1).Cell(2);
            Devicetype = pageRows.ElementAt(2).Cell(2);

            // Påmindelse: Det ser ut til at flere instanser lages av samme data. Kan flere SettingPage bli instansiert av samme data?

            List<int> digsiPathRows = ExcelStaticTools.FindDigsiPathRows(pageRows, 17, false);
            if (digsiPathRows.Count == 0)
            {
                throw new InvalidOperationException($"No Digsi path rows found for settings page starting at worksheet row {pageRows.First().RowNumber()}.");
            }

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
        public List<IExcelHmiTable> HmiTableSections { get; private set; } = new List<IExcelHmiTable>();
    }
}

