using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RelayFuseInterfaces;
using RelayPlanDocumentModel;
using RelaySettingToolViewModel;

namespace RelaySettingToolView
{
    public partial class MergingToolView : UserControl
    {
        private Point _dragStartPoint;

        public MergingToolView()
        {
            InitializeComponent();
            DataContext = new MergingToolViewModel();
        }

        private void NonMatchedListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void NonMatchedListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var position = e.GetPosition(null);
            if (Math.Abs(position.X - _dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(position.Y - _dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            if (sender is not ListBox listBox)
                return;

            var data = GetDragItem(listBox, e.OriginalSource as DependencyObject);
            if (data is IExcelHmiTable table)
            {
                DragDrop.DoDragDrop(listBox, new DataObject(typeof(IExcelHmiTable), table), DragDropEffects.Move);
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