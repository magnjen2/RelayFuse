using Microsoft.Win32;
using RelaySettingToolModel;
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

namespace RelaySettingToolViewModel
{
    public class RelaySettingMainViewModel : INotifyPropertyChanged
    {
        public ICommand OpenTeaxFileCommand { get; }
        public ICommand OpenPdfFileCommand { get; }

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

        public event PropertyChangedEventHandler? PropertyChanged;

        public IHWUnitNode? SelectedHwUnit
        {
            get => _selectedHwUnit;
            set
            {
                if (_selectedHwUnit != value)
                {
                    _selectedHwUnit = value;
                    OnPropertyChanged(nameof(SelectedHwUnit));
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




        public RelaySettingMainViewModel()
        {
            OpenTeaxFileCommand = new RelayCommand(OpenTeaxFile);
            OpenPdfFileCommand = new RelayCommand(OpenPdfFile, CanOpenPdfFile);

            SelectedHwUnit = HwUnits?.FirstOrDefault();

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

            var _hwUnitList =  new List<IHWUnitNode>() { new PlaceholderHWUnitNode() };
            _hwUnitList.AddRange(TreeRootBase.HardwareContainer.HardwareUnits);
            HwUnits = _hwUnitList;
            SelectedHwUnit = HwUnits?.FirstOrDefault();
            IsTeaxFileLoaded = true;

        }

        private bool CanOpenPdfFile(object? parameter)
        {
            return IsTeaxFileLoaded && !(SelectedHwUnit is PlaceholderHWUnitNode);
        }

        private void OpenPdfFile(object? parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string pdfPath = openFileDialog.FileName;
                ProcessPdf(pdfPath);
            }
        }

        private void ProcessPdf(string pdfPath)
        {
            var pdfService = new PdfDocumentService();
            var documentModel = new PdfDocumentModel(pdfPath);

            documentModel.Devices  = pdfService.GetDevicesFromToc(documentModel.Document.GetPage(1));


            List<string> allFgNames = TreeRootBase!
                .GetApplicationNodeByHwUnitId(SelectedHwUnit!.Id)
                .FunctionalApplication
                .FunctionGroupNodes
                .Select(x=>x.DisplayName).ToList();

            pdfService.ProcessPdfDocument(documentModel, allFgNames);
        }












        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            private readonly Func<object?, bool>? _canExecute;

            public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
            public void Execute(object? parameter) => _execute(parameter);
            public event EventHandler? CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
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

