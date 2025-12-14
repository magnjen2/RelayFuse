namespace RelayFuseInterfaces
{
    public interface IHmiTable
    {
        List<IRelaySetting> Settings { get; set; }
        string? DigsiPathString { get; }
        string[]? DigsiPathList { get; set; }
        string? UniqueId { get; }
    }
}

