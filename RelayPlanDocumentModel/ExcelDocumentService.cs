using ClosedXML.Excel;
using System;
using System.IO;

namespace RelayPlanDocumentModel
{
    public class ExcelDocumentService : IDisposable
    {
        private readonly XLWorkbook _templateWorkbook;
        private IXLWorksheet topTemplateSheet { get; set; }
        private IXLWorksheet midTemplateSheet { get; set; }
        private IXLWorksheet bottomTemplateSheet { get; set; }

        public ExcelDocumentService()
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExcelTemplates", "RelayPlanTemplate.xlsx");
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template file not found at path '{templatePath}'.", templatePath);
            }

            _templateWorkbook = new XLWorkbook(templatePath);
            topTemplateSheet = _templateWorkbook.Worksheet("TopSection");
            midTemplateSheet = _templateWorkbook.Worksheet("MidSection");
            bottomTemplateSheet = _templateWorkbook.Worksheet("BottomSection");
        }

        public void Dispose()
        {
            _templateWorkbook?.Dispose();
        }
    }
}
