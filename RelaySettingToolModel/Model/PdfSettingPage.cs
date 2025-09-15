using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Graphics;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Tokenization;

namespace RelaySettingToolModel
{
    public interface IPdfSettingPage
    {
        Page Page { get; }
        List<IPdfSettingTableEntry> SettingTableEntries { get; set; }

    }
    public class PdfSettingPage : IPdfSettingPage
    {
        public PdfSettingPage(Page page)
        {
            Page = page;



        }

        public Page Page { get; }

        public List<IPdfSettingTableEntry> SettingTableEntries { get; set; } = new List<IPdfSettingTableEntry>();
    }



}
