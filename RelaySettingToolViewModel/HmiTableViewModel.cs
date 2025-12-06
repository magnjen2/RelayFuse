using RelayPlanDocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RelayFuseInterfaces;

namespace RelaySettingToolViewModel
{
    public interface IHmiTableViewModel : IDataGridItem
    {
        IHmiTable HmiTable { get; set; }
        List<IRelaySettingViewModel> SettingViewModels { get; set; }
        IHmiTableViewModel? MatchingHmiTableVM { get; set; }
        bool[] MatchConfidence { get; set; }
    }

    public class HmiTableViewModel : IHmiTableViewModel
    {
        public HmiTableViewModel(IHmiTable hmiTable)
        {
            HmiTable = hmiTable;
            Color = Colors.Transparent;
            SettingViewModels = hmiTable.Settings
                                    .Where(x => x.SelectedValue != string.Empty)
                                    .Select(s => new RelaySettingViewModel(s) as IRelaySettingViewModel)
                                    .ToList();
        }

        public IHmiTable HmiTable { get; set; }
        public List<IRelaySettingViewModel> SettingViewModels { get; set; } = new List<IRelaySettingViewModel>();
        public List<IRelaySettingViewModel> MatchedSettingVMs { get; set; } = new List<IRelaySettingViewModel>();
        public Color Color { get; set; }

        public IHmiTableViewModel? MatchingHmiTableVM { get; set; }

        public bool[] MatchConfidence { get; set; } = new bool[2];




    }

}
