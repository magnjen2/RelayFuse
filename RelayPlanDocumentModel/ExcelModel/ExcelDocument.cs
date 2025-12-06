using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RelayPlanDocumentModel
{
    public interface IExcelDocument
    {
        IXLCell? Anleggseier { get; set; }
        IXLCell? Dato { get; set; }
        IXLCell? Enhet { get; set; }
        IXLCell? ReleplanNavn { get; set; }
        IXLCell? Stasjon { get; set; }
        IXLCell? Utgave { get; set; }
        IXLWorkbook? Workbook { get; set; }
        void Save(string filePath);
        List<IGrouping<string, IXLRow>> DeviceTypeRows { get; set; }
        IExcelDevice GetDevice(IGrouping<string, IXLRow> deviceTypeRows);
    }

    public class ExcelDocument : IExcelDocument
    {
        public ExcelDocument() { }
        public ExcelDocument(string filePath)
        {
            Workbook = new XLWorkbook(filePath);
            var worksheet = Workbook.Worksheets.FirstOrDefault();

            if (worksheet != null)
            {
                Anleggseier = worksheet.Cell("E6");
                Stasjon = worksheet.Cell("E7");
                Enhet = worksheet.Cell("E8");
                ReleplanNavn = worksheet.Cell("N6");
                Dato = worksheet.Cell("D12");
                Utgave = worksheet.Cell("B12");

                _unusedRowSections = ExcelStaticTools.FindUnusedRowSections(worksheet, 3);

                var titlesRows = worksheet.RowsUsed()
                    .Where(row => row.Cell(2).GetString().Contains("Adresse:")
                    && row.Cell(4).GetString().Contains("Display-tekst:")
                    && row.Cell(12).GetString().Contains("Kommentarer:"));

                // Load Sip5Types.csv into a HashSet<string>
                var sip5Types = new HashSet<string>(
                    System.IO.File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExcelTemplates", "Sip5Types.csv"))
                        .Select(line => line.Trim())
                        .Where(line => !string.IsNullOrEmpty(line))
                );

                DeviceTypeRows = titlesRows
                    .Select(row => worksheet.Row(row.RowNumber() - 2))
                    .Where(row =>
                    {
                        var cellValue = row.Cell(2).GetString();
                        return sip5Types.Any(type => cellValue.Contains(type, StringComparison.OrdinalIgnoreCase));
                    })
                    .GroupBy(r => r.Cell(2).GetString())
                    .ToList();

            }
        }

        public IExcelDevice GetDevice(IGrouping<string, IXLRow> deviceTypeRows)
        {
            var worksheet = Workbook.Worksheets.FirstOrDefault();

            _pageStartRows = deviceTypeRows.Select(r => r.RowNumber() - 2).ToList();



            var pageSegments = ExcelStaticTools.GetRowSegments(worksheet, _pageStartRows, _unusedRowSections);

            var pagesList = new List<ISettingsPage>();
            foreach (var pageRows in pageSegments)
            {
                pagesList.Add(new SettingsPage(pageRows, worksheet));
            }

            return new ExcelDevice(pagesList);
        }

        public List<IGrouping<string, IXLRow>> DeviceTypeRows { get; set; } = new List<IGrouping<string, IXLRow>>();

        private List<int> _unusedRowSections = new List<int>();
        private List<int> _pageStartRows = new List<int>();
        public IXLCell? Anleggseier { get; set; }
        public IXLCell? Stasjon { get; set; }
        public IXLCell? Enhet { get; set; }
        public IXLCell? ReleplanNavn { get; set; }
        public IXLCell? Utgave { get; set; }
        public IXLCell? Dato { get; set; }
        public IXLWorkbook? Workbook { get; set; }

        public void Save(string filePath)
        {
            Workbook?.SaveAs(filePath);
        }
    }
}

