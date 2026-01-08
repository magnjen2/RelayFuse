//using System;
//using System.Windows.Input;
//using System.Windows.Media;

//namespace RelaySettingToolViewModel
//{
//    public class DataGridCellViewModel : ViewModelBase
//    {
//        public IDataGridItemBase? AssociatedItem { get; set; }
//        public string Content { get; set; } = string.Empty;
//        public Color CellColor { get; set; } = Colors.Transparent;
//        public DataGridCellViewModel TwinDataCell { get; set; } = null!;

//        private bool _isSelected = false;
//        public bool IsSelected
//        {
//            get => _isSelected;
//            set
//            {
//                if (_isSelected != value)
//                {
//                    _isSelected = value;
//                    OnPropertyChanged(nameof(IsSelected));
//                }
//            }
//        }
//    }
//}
