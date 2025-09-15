using System.ComponentModel;

namespace RelaySettingToolViewModel
{
    public class MergingToolViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Add properties or logic as needed for merging functionality
    }
}