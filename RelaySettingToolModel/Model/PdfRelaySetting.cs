using UglyToad.PdfPig.Content;

namespace RelaySettingToolModel
{
    public class PdfRelaySetting
    {
        public PdfRelaySetting(List<Word>? address, List<Word> displayName, List<List<Word>> possibleValues)
        {
            Address = address;
            DisplayName = displayName;
            PossibleValues = possibleValues;

        }
        public List<Word>? Address { get; }
        public List<Word> DisplayName { get; }
        public List<Word> SettingValue { get; } = new(); // => PossibleValues.Startswith("...");
        public List<List<Word>> PossibleValues { get; set; } = new();

    }



}
