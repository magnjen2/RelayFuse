using System;
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
            SettingPages = pages ?? throw new ArgumentNullException(nameof(pages));
            if (SettingPages.Count == 0)
            {
                throw new ArgumentException("At least one settings page is required to build an ExcelDevice.", nameof(pages));
            }
        }

        private List<ISettingsPage> _settingPages = new List<ISettingsPage>();
        public List<ISettingsPage> SettingPages
        {
            get => _settingPages;
            set => _settingPages = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string DeviceType => SettingPages.FirstOrDefault()?.Devicetype?.GetString() ?? string.Empty;

        public string? Produktkode => SettingPages.FirstOrDefault()?.Produktkode?.GetString();
    }
}