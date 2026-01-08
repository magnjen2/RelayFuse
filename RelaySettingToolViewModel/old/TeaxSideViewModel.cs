//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Navigation;
//using Sip5Library.Sip5TeaxModels.Applications;

//namespace RelaySettingToolViewModel
//{
//    public class TeaxSideViewModel : ViewModelBase
//    {
//        public TeaxSideViewModel()
//        {

//        }

//        public TeaxSideViewModel(IApplicationNode applicationNode)
//        {
//            _applicationNode = applicationNode;
//            _teaxRelayFuseService = new TeaxRelayFuseService(applicationNode.FunctionalApplication);
//            List<ITeaxHmiTable> teaxHmiTables = _teaxRelayFuseService.GetHmiTables();
            
//            _hmiTablVMs = teaxHmiTables.Select(t => new HmiTableViewModel(t) as IHmiTableViewModel).ToList();

//            //_dataGridVM = new DataGridViewModel();

//            //_dataGridVM.LoadMultipleTables(_hmiTablVMs);
//        }

//        //private DataGridViewModel? _dataGridVM;
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

//        private IApplicationNode? _applicationNode;
//        public IApplicationNode? ApplicationNode
//        {
//            get => _applicationNode;
//            set
//            {
//                if (_applicationNode != value)
//                {
//                    _applicationNode = value;
//                    OnPropertyChanged(nameof(ApplicationNode));
//                }
//            }
//        }
//        private TeaxRelayFuseService? _teaxRelayFuseService;

//        private List<IHmiTableViewModel> _hmiTablVMs = new List<IHmiTableViewModel>();
//        public List<IHmiTableViewModel> HmiTableVMs
//        {
//            get => _hmiTablVMs;
//            set
//            {
//                if (_hmiTablVMs != value)
//                {
//                    _hmiTablVMs = value;
//                    OnPropertyChanged(nameof(HmiTableVMs));
//                }
//            }
//        }
//    }
//}
