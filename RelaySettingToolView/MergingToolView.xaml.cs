using System.Windows.Controls;
using RelaySettingToolViewModel;

namespace RelaySettingToolView
{
    public partial class MergingToolView : UserControl
    {
        public MergingToolView()
        {
            InitializeComponent();
            DataContext = new MergingToolViewModel();
        }
    }
}