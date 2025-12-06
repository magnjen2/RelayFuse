using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Graphics;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Tokenization;

namespace RelaySettingToolModel
{
    public class PlanDocumentModel
    {
        public PlanDocumentModel(string filePath)
        {
            Document = PdfDocument.Open(filePath);

        }

        public PdfDocument Document { get; set; }
        public List<PdfDeviceModel> Devices { get; set; } = new List<PdfDeviceModel>();

    }
    public class PdfDeviceModel
    {
        public PdfDeviceModel(string tocName, string deviceType, int tocStartPage)
        {
            TocName = tocName;
            DeviceType = deviceType;
            TocStartPage = tocStartPage;

        }
        public string TocName { get; }
        public string DeviceType { get; }
        public int TocStartPage { get; }
        public List<IPdfSettingPage> SettingPages { get; set; } = new List<IPdfSettingPage>();
        public List<PdfFunctionGroup> FunctionGroups { get; set; } = new List<PdfFunctionGroup>();
        public List<PdfFunctionBlock> FunctionBlocks { get; set; } = new List<PdfFunctionBlock>();

    }
    public class PdfFunctionGroup
    {
        PdfFunctionGroup(string displayName)
        {
            DisplayName = displayName;
        }
        public string DisplayName { get; }
        public List<PdfFunction> Functions { get; set; } = new List<PdfFunction>();
        public List<PdfFunctionBlock> FunctionBlocks { get; set; } = new List<PdfFunctionBlock>();
    }
    public class PdfFunction
    {
        PdfFunction(string displayName)
        {
            DisplayName = displayName;
        }
        public string DisplayName { get; }
        public List<PdfFunctionBlock> FunctionBlocks { get; set; } = new List<PdfFunctionBlock>();
    }
    public class PdfFunctionBlock
    {
        PdfFunctionBlock(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; }
        public List<PdfHmiTable> HmiTables { get; set; } = new List<PdfHmiTable>();
        public List<PdfRelaySetting> Settings { get; set; } = new List<PdfRelaySetting>();
    }
    public class PdfHmiTable
    {
        public PdfHmiTable(string hmiTableName)
        {
            HmiTableName = hmiTableName;
        }
        public string HmiTableName { get; }
        public List<PdfRelaySetting> Settings { get; set; } = new List<PdfRelaySetting>();
    }



}
