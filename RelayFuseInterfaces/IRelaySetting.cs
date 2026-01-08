namespace RelayFuseInterfaces
{
    public interface IRelaySetting
    {
        string UniqueId { get; set; }
        string DisplayName { get; set; }
        string SelectedValue { get; set; }
        string Unit { get; set; }
        string DigsiPathString { get; }
    }

}

