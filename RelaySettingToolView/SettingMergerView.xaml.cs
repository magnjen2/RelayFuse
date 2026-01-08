using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RelayFuseInterfaces;
using RelaySettingToolViewModel;

namespace RelaySettingToolView
{
    /// <summary>
    /// Interaction logic for SettingMergerView.xaml
    /// </summary>
    public partial class SettingMergerView : UserControl
    {
        public SettingMergerView()
        {
            InitializeComponent();
        }

        private void SettingMergerView_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(IRelaySetting)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void SettingMergerView_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(IRelaySetting)))
                return;

            var setting = e.Data.GetData(typeof(IRelaySetting)) as IRelaySetting;
            if (setting == null)
                return;

            if (DataContext is ISettingMergerViewModel vm && vm.AttachSettingCommand != null)
            {
                if (vm.AttachSettingCommand.CanExecute(setting))
                {
                    vm.AttachSettingCommand.Execute(setting);
                }
            }
        }
    }
}
