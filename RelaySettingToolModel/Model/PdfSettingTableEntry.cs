using UglyToad.PdfPig.Content;
using RelaySettingToolModel;

public interface IPdfSettingTableEntry
{

    List<Word> SettingPath { get; }
    List<string> SettingPathAsCSV { get; }
    List<PdfRelaySetting> Settings { get; }
}

namespace RelaySettingToolModel
{
    // This class is used as an intermediate data holder while parsing setting pages.
    public class PdfSettingTableEntry : IPdfSettingTableEntry
    {
        public PdfSettingTableEntry(List<Word> settingPath, List<PdfRelaySetting> settings)
        {
            SettingPath = settingPath;
            Settings = settings;
        }
        public List<Word> SettingPath { get; }
        public List<string> SettingPathAsCSV => GeneralPdfServices.MakeCVSStrings(SettingPath);
        public List<PdfRelaySetting> Settings { get; }



    }
}
