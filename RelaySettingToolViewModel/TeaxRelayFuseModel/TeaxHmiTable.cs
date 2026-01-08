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
            _digsiPathList = digsiPath;
            Settings = settings;
        }
        private string[] _digsiPathList;
        public string[] DigsiPathList => _digsiPathList;
        public List<IRelaySetting> Settings { get; set; } = new List<IRelaySetting>();
        public string? UniqueId => Settings.FirstOrDefault()?.UniqueId;
        public string? DigsiPathString => _digsiPathList != null ? string.Join(", ", _digsiPathList) : null;

    }

}
