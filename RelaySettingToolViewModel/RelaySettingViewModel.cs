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
    public interface IRelaySettingViewModel : IDataGridItemBase
    {
        IRelaySetting RelaySetting { get; }
        IRelaySettingViewModel? MatchingSettingVM { get; set; }
        int MatchConfidence { get; set; }


    }

    public class RelaySettingViewModel : DataGridItemBase, IRelaySettingViewModel
    {
        public RelaySettingViewModel(IRelaySetting relaySetting)
        {
            RelaySetting = relaySetting;
            Color = Colors.Transparent;
        }
        public IRelaySetting RelaySetting { get; }
        public IRelaySettingViewModel? MatchingSettingVM { get; set; }
        public int MatchConfidence { get; set; }


    }
}