using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using RelayFuseInterfaces;
using RelayPlanDocumentModel;
using ICollectionView = System.ComponentModel.ICollectionView;

namespace RelaySettingToolViewModel;

public partial class HmiTableMergerViewModel
{
    private ObservableCollection<SettingMergerViewModel> _settingMergers = new();
    private ObservableCollection<IRelaySetting> _nonMatchedSettings = new();
    private ICollectionView _nonMatchedSettingsView = null!;

    public ObservableCollection<SettingMergerViewModel> SettingMergers
    {
        get => _settingMergers;
        set
        {
            if (_settingMergers == value)
                return;

            if (_settingMergers != null)
            {
                _settingMergers.CollectionChanged -= SettingMergersCollectionChanged;
                foreach (var merger in _settingMergers)
                {
                    DetachFromSettingMerger(merger);
                }
            }

            _settingMergers = value ?? new ObservableCollection<SettingMergerViewModel>();
            foreach (var merger in _settingMergers)
            {
                AttachToSettingMerger(merger);
            }
            _settingMergers.CollectionChanged += SettingMergersCollectionChanged;

            RecalculateNonMatchedSettings();
            RefreshNonMatchedSettingsView();
            OnPropertyChanged(nameof(SettingMergers));
        }
    }

    public ObservableCollection<IRelaySetting> NonMatchedSettings => _nonMatchedSettings;

    public ICollectionView NonMatchedSettingsView => _nonMatchedSettingsView;

    private void InitializeCollections()
    {
        foreach (var merger in _settingMergers)
        {
            AttachToSettingMerger(merger);
        }

        _settingMergers.CollectionChanged += SettingMergersCollectionChanged;
        _nonMatchedSettings.CollectionChanged += NonMatchedSettingsCollectionChanged;

        _nonMatchedSettingsView = CollectionViewSource.GetDefaultView(_nonMatchedSettings);
        _nonMatchedSettingsView.Filter = FilterNonMatchedSetting;

        RecalculateNonMatchedSettings();
    }

    private void SettingMergersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (SettingMergerViewModel oldItem in e.OldItems)
            {
                DetachFromSettingMerger(oldItem);
            }
        }

        if (e.NewItems != null)
        {
            foreach (SettingMergerViewModel newItem in e.NewItems)
            {
                AttachToSettingMerger(newItem);
            }
        }

        RecalculateNonMatchedSettings();
        RefreshNonMatchedSettingsView();
        OnPropertyChanged(nameof(SettingMergers));
    }

    private void NonMatchedSettingsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshNonMatchedSettingsView();
        OnPropertyChanged(nameof(NonMatchedSettings));
    }

    private void AttachToSettingMerger(SettingMergerViewModel merger)
    {
        if (merger is INotifyPropertyChanged notifier)
        {
            notifier.PropertyChanged += SettingMergerPropertyChanged;
        }
    }

    private void DetachFromSettingMerger(SettingMergerViewModel merger)
    {
        if (merger is INotifyPropertyChanged notifier)
        {
            notifier.PropertyChanged -= SettingMergerPropertyChanged;
        }
    }

    private void SettingMergerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SettingMergerViewModel.ExcelRelaySetting))
        {
            RecalculateNonMatchedSettings();
            RefreshNonMatchedSettingsView();
        }
    }

    private bool FilterNonMatchedSetting(object obj)
    {
        if (obj is not IRelaySetting setting)
            return false;

        return !IsSettingAssigned(setting);
    }

    private bool IsSettingAssigned(IRelaySetting setting)
    {
        foreach (var merger in _settingMergers)
        {
            if (ReferenceEquals(merger.ExcelRelaySetting, setting))
            {
                return true;
            }
        }

        return false;
    }

    private void RecalculateNonMatchedSettings()
    {
        var excelSettings = _excelHmiTable?.Settings ?? Enumerable.Empty<IRelaySetting>();

        var desired = new HashSet<IRelaySetting>(
            excelSettings.Where(setting => !IsSettingAssigned(setting)),
            ReferenceEqualityComparer.Instance);

        for (int i = _nonMatchedSettings.Count - 1; i >= 0; i--)
        {
            var existing = _nonMatchedSettings[i];
            if (!desired.Remove(existing))
            {
                _nonMatchedSettings.RemoveAt(i);
            }
        }

        foreach (var setting in desired)
        {
            _nonMatchedSettings.Add(setting);
        }

        RefreshNonMatchedSettingsView();
        OnPropertyChanged(nameof(NonMatchedSettings));
    }

    private void RefreshNonMatchedSettingsView()
    {
        _nonMatchedSettingsView?.Refresh();
        OnPropertyChanged(nameof(NonMatchedSettingsView));
    }
}
