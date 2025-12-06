using DocumentFormat.OpenXml.Wordprocessing;
using RelayFuseInterfaces;
using RelayPlanDocumentModel;
using Sip5Library.Sip5TeaxModels.Applications;
using System.Windows;
using System.Windows.Media;


namespace RelaySettingToolViewModel
{
    public interface ITeaxRelaySetting : IRelaySetting
    {
        string[] DigsiPath { get; set; }
        ISettingNodeBase SettingNode { get; }

    }

    public class TeaxRelaySetting : ITeaxRelaySetting
    {
        public TeaxRelaySetting(ISettingNodeBase settingNode, string[] digsiPath, int settingGroupIndex = 1)
        {
            SettingNode = settingNode;
            _settingGroupindex = settingGroupIndex;
            DigsiPath = digsiPath;
        }
        public ISettingNodeBase SettingNode { get; private set; }
        private int _settingGroupindex { get; set; }
        public string DisplayName
        {
            get
            {
                return SettingNode.DisplayName;
            }
            set
            {
                // DisplayName is read-only, do nothing
            }
        }
        public string SelectedValue
        {
            get
            {
                return SettingNode.GetSettingGroupNode(_settingGroupindex)?.Value ?? string.Empty;
            }
            set
            {
                var settingGroupNode = SettingNode.GetSettingGroupNode(_settingGroupindex);
                if (settingGroupNode != null)
                {
                    settingGroupNode.Value = value;
                }
            }
        }
        public string UniqueId
        {
            get
            {
                return SettingNode.VisibleUniqueId;
            }
            set
            {
                // UniqueId is read-only, do nothing
            }
        }
        public string Unit
        {
            get
            {
                return SettingNode is INumericSettingNode numericNode ? numericNode.BaseUnit.BaseUnitToString() : string.Empty;
            }
            set
            {
                // Unit is read-only, do nothing
            }
        }
        public string[] DigsiPath { get; set; }
        public string DigsiPathString
        {
            get => string.Join(", ", DigsiPath);
        }
    }

}
