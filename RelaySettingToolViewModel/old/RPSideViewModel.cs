//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using DocumentFormat.OpenXml.Wordprocessing;
//using RelayPlanDocumentModel;
//using Microsoft.Win32;
//using System.Text.RegularExpressions;
//using System.Collections.ObjectModel;
//using RelayFuseInterfaces;

//namespace RelaySettingToolViewModel
//{
//    public class RPSideViewModel : ViewModelBase
//    {
//        public RPSideViewModel()
//        {

//        }
//        public RPSideViewModel(IExcelDevice deviceInExcel)
//        {

//            _selectedDevice = deviceInExcel;
//            foreach (var hmiTable in _selectedDevice!.SettingPages.SelectMany(x => x.HmiTableSections!))
//            {
//                HmiTableVMs.Add(new HmiTableViewModel(hmiTable as IHmiTable));
//            }

//            //DataGridVM.LoadMultipleTables(HmiTableVMs);


//        }
//        private IExcelDevice? _selectedDevice;


//        private List<IHmiTableViewModel> hmiTableVMs = new List<IHmiTableViewModel>();
//        public List<IHmiTableViewModel> HmiTableVMs
//        {
//            get => hmiTableVMs;
//            set
//            {
//                if (hmiTableVMs != value)
//                {
//                    hmiTableVMs = value;
//                    OnPropertyChanged(nameof(HmiTableVMs));
//                }
//            }
//        }
//        //private DataGridViewModel? _dataGridVM = new DataGridViewModel();
//        //public DataGridViewModel? DataGridVM
//        //{
//        //    get => _dataGridVM;
//        //    set
//        //    {
//        //        if (_dataGridVM != value)
//        //        {
//        //            _dataGridVM = value;
//        //            OnPropertyChanged(nameof(DataGridVM));
//        //        }
//        //    }
//        //}





//    }
//}