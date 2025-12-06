using System.Collections.Generic;
using System.Linq;

namespace RelayPlanDocumentModel
{
    public interface IExcelDevice
    {
        List<ISettingsPage> SettingPages { get; set; }
        string DeviceType { get; }
        string? Produktkode { get; }
    }

    public class ExcelDevice : IExcelDevice
    {
        public ExcelDevice(List<ISettingsPage> pages)
        {
            SettingPages = pages;
        }

        public List<ISettingsPage> SettingPages { get; set; }

        public string DeviceType => SettingPages.FirstOrDefault()?.Devicetype?.GetString() ?? string.Empty;

        public string? Produktkode => SettingPages.FirstOrDefault()?.Produktkode?.GetString();
    }
}