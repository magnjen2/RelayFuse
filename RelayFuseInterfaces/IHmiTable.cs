namespace RelayFuseInterfaces
{
    public interface IHmiTable
    {
        IHmiTable MatchingTable { get; set; }
        List<IRelaySetting> Settings { get; set; }
        string? DigsiPathString { get; }
        string[]? DigsiPathList { get; set; }
        string? UniqueId { get; }
    }
}

