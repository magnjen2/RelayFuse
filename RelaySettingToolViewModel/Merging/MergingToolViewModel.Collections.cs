using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using RelayFuseInterfaces;
using RelayPlanDocumentModel;

namespace RelaySettingToolViewModel;
public partial class MergingToolViewModel
{
    // Collection of HMI table mergers
    private ObservableCollection<IHmiTableMergerViewModel> _hmiTableMergers = new();

    // Collection of all RP HMI tables
    private readonly ObservableCollection<IHmiTableViewModel> _excelHmiTableVMs = new();

    // Filtered collection for displaying non matched RP HMI tables.
    private readonly ICollectionView _nonMatchedHmiTablesView;

    public ICollectionView NonMatchedHmiTables => _nonMatchedHmiTablesView;

    public IEnumerable<IHmiTableViewModel> GetUnmatchedHmiTables() => _excelHmiTableVMs.Where(t => !IsTableAssigned(t));

    public ObservableCollection<IHmiTableMergerViewModel> HmiTableMergers
    {
        get => _hmiTableMergers;
        set
        {
            if (_hmiTableMergers == value)
                return;

            if (_hmiTableMergers != null)
            {
                _hmiTableMergers.CollectionChanged -= HmiTableMergersCollectionChanged;
                foreach (var merger in _hmiTableMergers)
                {
                    DetachFromHmiTableMerger(merger);
                }
            }

            _hmiTableMergers = value ?? new ObservableCollection<IHmiTableMergerViewModel>();
            foreach (var merger in _hmiTableMergers)
            {
                AttachToHmiTableMerger(merger);
            }
            _hmiTableMergers.CollectionChanged += HmiTableMergersCollectionChanged;

            RefreshNonMatchedView();
            OnPropertyChanged(nameof(HmiTableMergers));
        }
    }

    // Keeps the filtered list accurate whenever merger assignments change.
    private void HmiTableMergersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (IHmiTableMergerViewModel oldItem in e.OldItems)
            {
                DetachFromHmiTableMerger(oldItem);
            }
        }

        if (e.NewItems != null)
        {
            foreach (IHmiTableMergerViewModel newItem in e.NewItems)
            {
                AttachToHmiTableMerger(newItem);
            }
        }

        RefreshNonMatchedView();
        OnPropertyChanged(nameof(HmiTableMergers));
    }

    // Filter used by the UI CollectionView so only unassigned tables are shown.
    private bool FilterNonMatchedTable(object obj)
    {
        if (obj is not IHmiTableViewModel table)
            return false;

        return !IsTableAssigned(table);
    }

    private bool IsTableAssigned(IHmiTableViewModel table)
    {
        foreach (var merger in _hmiTableMergers)
        {
            if (ReferenceEquals(merger.ExcelHmiTable, table))
            {
                return true;
            }
        }
        return false;
    }

    // Hooks property change notifications so we react when a merger rebinds to a new table.
    private void AttachToHmiTableMerger(IHmiTableMergerViewModel merger)
    {
        if (merger is INotifyPropertyChanged notifier)
        {
            notifier.PropertyChanged += HmiTableMergerPropertyChanged;
        }
    }

    private void DetachFromHmiTableMerger(IHmiTableMergerViewModel merger)
    {
        if (merger is INotifyPropertyChanged notifier)
        {
            notifier.PropertyChanged -= HmiTableMergerPropertyChanged;
        }
    }

    private void HmiTableMergerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IHmiTableMergerViewModel.ExcelHmiTable))
        {
            RefreshNonMatchedView();
        }
    }

    private void RefreshNonMatchedView()
    {
        _nonMatchedHmiTablesView.Refresh();
        OnPropertyChanged(nameof(NonMatchedHmiTables));
    }
}
