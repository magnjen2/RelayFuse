using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelayPlanDocumentModel
{
    public class ExcelDocumentService
    {
        private IXLWorksheet topTemplateSheet { get;  set; }
        private IXLWorksheet midTemplateSheet { get;  set; }
        private IXLWorksheet bottomTemplateSheet { get;  set; }

        public ExcelDocumentService()
        {
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ExcelTemplates", "RelayPlanTemplate.xlsx");
            var templateWorkbook = new XLWorkbook(templatePath);
            topTemplateSheet = templateWorkbook.Worksheet("TopSection");
            midTemplateSheet = templateWorkbook.Worksheet("MidSection");
            bottomTemplateSheet = templateWorkbook.Worksheet("BottomSection");

        }



    }
}
