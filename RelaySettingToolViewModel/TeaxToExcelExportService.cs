using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sip5Library.Sip5TeaxModels;
using RelayPlanDocumentModel;
using Sip5Library.Sip5TeaxModels.Applications.FunctionalApplication;
using Sip5Library.Sip5TeaxModels.Applications.FunctionalApplication.Setting;
using Sip5Library.Sip5TeaxModels.Applications;
using System.Drawing.Text;


namespace RelaySettingToolViewModel
{
    public class TeaxToExcelExportService
    {
        public TeaxToExcelExportService()
        {
        }
        public ExcelDocumentModel MakeExcelModel(ITeaxTreeRootBase treeRootBase, IHWUnitNode selectedHwUnit, int settingGrp)
        {
            var applicationNode = treeRootBase.GetApplicationNode(selectedHwUnit);
            _settingGrp = settingGrp;

            // Collect all settings with their path
            var settingsWithPaths = new List<(ISettingNodeBase setting, List<string> path)>();
            CollectSettingsWithPaths(applicationNode, new List<string>(), settingsWithPaths);

            // Group settings by (branch path, HmiTableName)
            var grouped = settingsWithPaths
                .GroupBy(x => new
                {
                    PathCsv = string.Join(",", x.path.Append(x.setting.HmiTableName)),
                    HmiTableName = x.setting.HmiTableName
                });

            var sections = new List<ExcelSettingSection>();

            foreach (var group in grouped)
            {
                var excelSettings = group.Select(x =>
                    new ExcelSettingModel(
                        address: x.setting.VisibleUniqueId,
                        displayName: x.setting.DisplayName,
                        settingValue: x.setting.GetSettingGroupNode(settingGrp)?.Value ?? string.Empty,
                        unit: x.setting is INumericSettingNode numSetting ? numSetting.BaseUnit.ToString() : string.Empty,         // Unit not available in interface, set empty or fetch if possible
                        comment: ""      // Not available during export to excel.
                    )).ToList();

                sections.Add(new ExcelSettingSection(group.Key.PathCsv, excelSettings));
            }

            // DeviceName and DeviceType
            var deviceName = selectedHwUnit.DisplayName;
            var deviceType = selectedHwUnit.ShortProductCode ?? selectedHwUnit.ProductCode;

            return new ExcelDocumentModel(deviceName, deviceType, sections);
        }

        private int _settingGrp;



        // Helper to traverse tree and collect all ISettingNodeBase nodes with their path
        private void CollectSettingsWithPaths(
            INodeWithDescendantsBase node,
            List<string> path,
            List<(ISettingNodeBase setting, List<string> path)> result)
        {
            // If node is a setting, add to result
            if (node is ISettingNodeBase settingNode && settingNode.GetSettingGroupNode(_settingGrp) != null)
                result.Add((settingNode, new List<string>(path)));

            // Traverse descendants
            foreach (var descendant in node.Descendants.Values)
            {
                if (descendant is INodeWithDescendantsBase child)
                {
                    // If child has DisplayName, add to path
                    var displayName = (child as IHasDisplayName)?.DisplayName;
                    if (!string.IsNullOrEmpty(displayName))
                        path.Add(displayName);

                    CollectSettingsWithPaths(child, path, result);

                    if (!string.IsNullOrEmpty(displayName))
                        path.RemoveAt(path.Count - 1);
                }
            }
        }
    }
}
