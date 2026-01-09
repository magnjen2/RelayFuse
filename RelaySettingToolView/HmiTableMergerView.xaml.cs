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
using RelayPlanDocumentModel;
using RelaySettingToolViewModel;

namespace RelaySettingToolView
{
    /// <summary>
    /// Interaction logic for HmiTableMergerView.xaml
    /// </summary>
    public partial class HmiTableMergerView : UserControl
    {
        private Point _settingDragStartPoint;

        public HmiTableMergerView()
        {
            InitializeComponent();
        }

        private void HmiTableMergerView_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(IHmiTableViewModel)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void HmiTableMergerView_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(IHmiTableViewModel)))
                return;

            var table = e.Data.GetData(typeof(IHmiTableViewModel)) as IHmiTableViewModel;
            if (table == null)
                return;

            if (DataContext is IHmiTableMergerViewModel vm && vm.AttachHmiTableCommand != null)
            {
                if (vm.AttachHmiTableCommand.CanExecute(table))
                {
                    vm.AttachHmiTableCommand.Execute(table);
                }
            }
        }

        private void NonMatchedSettings_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _settingDragStartPoint = e.GetPosition(null);
        }

        private void NonMatchedSettings_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var position = e.GetPosition(null);
            if (Math.Abs(position.X - _settingDragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(position.Y - _settingDragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            if (sender is not ListBox listBox)
                return;

            var data = GetDragItem(listBox, e.OriginalSource as DependencyObject);
            if (data is IRelaySettingViewModel setting)
            {
                DragDrop.DoDragDrop(listBox, new DataObject(typeof(IRelaySettingViewModel), setting), DragDropEffects.Move);
            }
        }

        private static object? GetDragItem(ListBox listBox, DependencyObject? originalSource)
        {
            var listBoxItem = ItemsControl.ContainerFromElement(listBox, originalSource) as ListBoxItem;
            if (listBoxItem != null)
            {
                return listBoxItem.DataContext;
            }

            return listBox.SelectedItem;
        }
    }
}
