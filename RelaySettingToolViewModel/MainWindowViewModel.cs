using Microsoft.Win32;
//using RelaySettingToolModel;
using Sip5Library.Sip5TeaxModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using RelayPlanDocumentModel;
using ClosedXML.Excel;
using System.Collections.ObjectModel;

namespace RelaySettingToolViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            OpenTeaxFileCommand = new RelayCommand(OpenTeaxFile);
            OpenRPlanFileCommand = new RelayCommand(OpenRPlanFile, CanOpenRPlanFile);
            ExportToExcelCommand = new RelayCommand(ExportToExcel, CanOpenRPlanFile);
            SelectedHwUnit = HwUnits?.FirstOrDefault();

            MergingToolViewModel.TeaxSideViewModel = new TeaxSideViewModel();
            MergingToolViewModel.RPSideViewModel = new RPSideViewModel();

        }


        public ICommand OpenTeaxFileCommand { get; }
        public ICommand OpenRPlanFileCommand { get; }
        public ICommand ExportToExcelCommand { get; }



        private bool _isTeaxFileLoaded;
        public bool IsTeaxFileLoaded
        {
            get => _isTeaxFileLoaded;
            private set
            {
                if (_isTeaxFileLoaded != value)
                {
                    _isTeaxFileLoaded = value;
                    OnPropertyChanged(nameof(IsTeaxFileLoaded));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }
        private bool _isRelayPlanLoaded;
        public bool IsRelayPlanLoaded
        {
            get => _isRelayPlanLoaded;
            private set
            {
                if (_isRelayPlanLoaded != value)
                {
                    _isRelayPlanLoaded = value;
                    OnPropertyChanged(nameof(IsRelayPlanLoaded));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private TeaxTreeRootBase? _treeRootBase;
        public TeaxTreeRootBase? TreeRootBase
        {
            get => _treeRootBase;
            private set
            {
                if (_treeRootBase != value)
                {
                    _treeRootBase = value;
                    OnPropertyChanged(nameof(TreeRootBase));
                }
            }
        }
        private List<IHWUnitNode>? _hwUnits = new List<IHWUnitNode>() { new PlaceholderHWUnitNode() };
        public List<IHWUnitNode>? HwUnits
        {
            get => _hwUnits;
            private set
            {
                if (_hwUnits != value)
                {
                    _hwUnits = value;
                    OnPropertyChanged(nameof(HwUnits));
                }
            }
        }

        private IHWUnitNode? _selectedHwUnit;
        public IHWUnitNode? SelectedHwUnit
        {
            get => _selectedHwUnit;
            set
            {
                if (_selectedHwUnit != value)
                {
                    _selectedHwUnit = value;
                    OnPropertyChanged(nameof(SelectedHwUnit));
                    OnHwUnitSelected();
                }
            }
        }
        private void OnHwUnitSelected()
        {
            if (!(_selectedHwUnit is PlaceholderHWUnitNode) && _selectedHwUnit != null)
            {
                var applicationNode = TreeRootBase?.GetApplicationNode(_selectedHwUnit);
                if (applicationNode != null)
                {
                    MergingToolViewModel.TeaxSideViewModel = new TeaxSideViewModel(applicationNode);
                }
            }
        }


        private MergingToolViewModel _mergingToolViewModel = new MergingToolViewModel();
        public MergingToolViewModel MergingToolViewModel
        {
            get => _mergingToolViewModel;
            set
            {
                if (_mergingToolViewModel != value)
                {
                    _mergingToolViewModel = value;
                    OnPropertyChanged(nameof(MergingToolViewModel));
                }
            }
        }


        private void OpenTeaxFile(object? parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Teax files (*.teax)|*.teax"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string textContent = File.ReadAllText(filePath);

                ProcessTeaxFile(filePath);

            }
        }

        private void ProcessTeaxFile(string filePath)
        {
            XElement rootNode = XElement.Load(filePath);
            TreeRootBase = new TeaxTreeRootBase(rootNode);

            var _hwUnitList = new List<IHWUnitNode>() { new PlaceholderHWUnitNode() };
            _hwUnitList.AddRange(TreeRootBase.HardwareContainer.HardwareUnits);
            HwUnits = _hwUnitList;
            SelectedHwUnit = HwUnits?.FirstOrDefault();
            IsTeaxFileLoaded = true;

        }

        private bool CanOpenRPlanFile(object? parameter)
        {
            return true;
        }
        private void OpenRPlanFile(object? parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string extension = Path.GetExtension(filePath).ToLowerInvariant();

                if (extension == ".xlsx" || extension == ".xls")
                {
                    Document = new ExcelDocument(filePath);
                }
                IsRelayPlanLoaded = true;
            }
        }
        private ExcelDocument? _document;
        public ExcelDocument? Document
        {
            get => _document;
            set
            {
                if (_document != value)
                {
                    _document = value;
                    DeviceTypeRows = Document?.DeviceTypeRows;
                    OnPropertyChanged(nameof(Document));
                }
            }
        }

        private List<IGrouping<string, IXLRow>>? _deviceTypeRows;
        public List<IGrouping<string, IXLRow>>? DeviceTypeRows
        {
            get => _deviceTypeRows;
            set
            {
                _deviceTypeRows = value;
                OnPropertyChanged(nameof(DeviceTypeRows));
            }
        }
        private IGrouping<string, IXLRow>? _selectedDeviceType;
        public IGrouping<string, IXLRow>? SelectedDeviceType
        {
            get => _selectedDeviceType;
            set
            {
                if (_selectedDeviceType != value)
                {
                    _selectedDeviceType = value;
                    OnPropertyChanged(nameof(SelectedDeviceType));
                    OnDeviceTypeSelected();
                }
            }
        }
        private void OnDeviceTypeSelected()
        {
            var deviceInExcel = _document?.GetDevice(SelectedDeviceType!);
            MergingToolViewModel.RPSideViewModel = new RPSideViewModel(deviceInExcel!);
        }


        // Add the private method for Excel export
        private void ExportToExcel(object? parameter)
        {
            //    var exportService = new TeaxToExcelExportService();
            //    var excelDocumentModel = exportService.MakeExcelModel(TreeRootBase!, SelectedHwUnit!, 1);
            //    MergingToolViewModel.RPSectionViewModel = new RPSectionViewModel(excelDocumentModel);

        }


        private class PlaceholderHWUnitNode : IHWUnitNode
        {
            public string DisplayName => "Select Device";
            public Guid Id => Guid.Empty;
            public string ProductCode => string.Empty;
            public string ShortProductCode => string.Empty;
            public string SerialNumber => string.Empty;
            public bool Volatile => false;
            public XElement TeaxNode => new XElement("Placeholder");
            public INodeWithDescendantsBase Parent => new NodeWithDescendantsBase();
            public T GoToParentOfType<T>() where T : INodeBase => default!;
        }
    }
}

