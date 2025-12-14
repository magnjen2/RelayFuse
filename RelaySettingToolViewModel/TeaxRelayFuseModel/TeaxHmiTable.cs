using RelayPlanDocumentModel;
using System;
using System.Windows.Media;
using RelayFuseInterfaces;


namespace RelaySettingToolViewModel
{
    public interface ITeaxHmiTable : IHmiTable
    {
    }

    public class TeaxHmiTable : ITeaxHmiTable
    {
        public TeaxHmiTable(string[] digsiPath, List<IRelaySetting> settings)
        {
            DigsiPathList = digsiPath;
            Settings = settings;
        }

        public string[] DigsiPathList { get; set; }
        public List<IRelaySetting> Settings { get; set; } = new List<IRelaySetting>();
        public string? UniqueId => Settings.FirstOrDefault()?.UniqueId;
        public string? DigsiPathString => DigsiPathList != null ? string.Join(", ", DigsiPathList) : null;

    }

}
