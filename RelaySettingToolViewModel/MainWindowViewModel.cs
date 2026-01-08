using Microsoft.Win32;
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

            _selectedHwUnit = HwUnits?.FirstOrDefault();
            _selectedDeviceType = DeviceTypeRows?.FirstOrDefault();


        }


        public ICommand OpenTeaxFileCommand { get; }
        public ICommand OpenRPlanFileCommand { get; }


        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged(nameof(IsBusy));
                }
            }
        }

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
                    MergingToolViewModel.InitializeTeax(applicationNode);
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


        private async void OpenTeaxFile(object? parameter)
        {
            _hwUnits = new List<IHWUnitNode>() { new PlaceholderHWUnitNode() };

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Teax files (*.teax)|*.teax"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string textContent = File.ReadAllText(filePath);
                XElement rootNode = XElement.Load(filePath);

                IsBusy = true;
                OnPropertyChanged(nameof(IsBusy));
                await Task.Yield();
                try
                {
                    TreeRootBase = await Task.Run(() => { return new TeaxTreeRootBase(rootNode); });
                }
                finally
                {
                    IsBusy = false; OnPropertyChanged(nameof(IsBusy));
                }

                HwUnits!.AddRange(TreeRootBase.HardwareContainer.HardwareUnits);
                OnPropertyChanged(nameof(HwUnits));
                SelectedHwUnit = HwUnits?.FirstOrDefault();
                IsTeaxFileLoaded = true;
            }
        }

        private bool CanOpenRPlanFile(object? parameter)
        {
            return true;
        }
        private async void OpenRPlanFile(object? parameter)
        {
            _deviceTypeRows = new List<IGrouping<string, IXLRow>>() { new PlaceholderDeviceTypeGrouping() };

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
                    IsBusy = true;
                    OnPropertyChanged(nameof(IsBusy));
                    await Task.Yield();
                    try
                    {
                        Document = await Task.Run(() => new ExcelDocument(filePath));
                    }
                    finally
                    {
                        IsBusy = false;
                    }

                    DeviceTypeRows!.AddRange(Document!.DeviceTypeRows);
                    OnPropertyChanged(nameof(DeviceTypeRows));
                    SelectedDeviceType = DeviceTypeRows.FirstOrDefault();
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
                    OnPropertyChanged(nameof(Document));
                }
            }
        }




        //------ Relay plan device Type Selection -----
        private List<IGrouping<string, IXLRow>>? _deviceTypeRows = new List<IGrouping<string, IXLRow>>() { new PlaceholderDeviceTypeGrouping() };
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
            if (!(SelectedDeviceType is PlaceholderDeviceTypeGrouping) && SelectedDeviceType != null)
            {
                var deviceInExcel = _document?.GetDevice(SelectedDeviceType!);
                MergingToolViewModel.InitializeRP(deviceInExcel!);
            }
        }



        private class PlaceholderDeviceTypeGrouping : IGrouping<string, IXLRow>
        {
            public string Key => "Select Device";

            public IEnumerator<IXLRow> GetEnumerator()
            {
                yield break;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
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

