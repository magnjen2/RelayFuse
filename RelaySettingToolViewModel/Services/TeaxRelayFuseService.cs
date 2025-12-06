using RelayPlanDocumentModel;
using Sip5Library.Sip5TeaxModels;
using Sip5Library.Sip5TeaxModels.Applications;
using RelayFuseInterfaces;

namespace RelaySettingToolViewModel
{
    public interface ITeaxRelayFuseService
    {
        List<ITeaxHmiTable> GetHmiTables();
    }

    public class TeaxRelayFuseService : ITeaxRelayFuseService
    {
        public TeaxRelayFuseService(IFunctionalApplicationNode funcAppNode)
        {
            _funcAppNode = funcAppNode;
        }
        private IFunctionalApplicationNode _funcAppNode;

        public List<ITeaxHmiTable> GetHmiTables()
        {
            // Step 1: Get all relevant setting nodes below the functional application node
            var settingNodes = _funcAppNode.NodesBelowOfType<ISettingNodeBase>().Where(x => x.IsVisible && !x.IsReadOnly).ToList();

            // Step 3: Create TeaxRelaySetting objects with DigsiPath
            var relaySettings = settingNodes
                .Select(sn =>
                {
                    var digsiPath = GetDigsiPath(sn);
                    var setting = new TeaxRelaySetting(sn, digsiPath);
                    return setting;
                })
                .ToList();

            // Step 4: Group settings by DigsiPath
            var groupedSettings = relaySettings
                .GroupBy(setting => setting.DigsiPath, new SequenceEqualityComparer<string>())
                .ToList();

            // Step 5: Create TeaxHmiTable objects from groups
            var hmiTables = groupedSettings
                .Select(group =>
                {
                    var digsiPath = group.Key;
                    var settingsList = group.Cast<IRelaySetting>().ToList();
                    var table = new TeaxHmiTable(digsiPath.ToArray(), settingsList);
                    return (ITeaxHmiTable)table;
                })
                .ToList();

            return hmiTables;
        }

        // Helper for grouping by sequence equality
        private class SequenceEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
        {
            public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                return x.SequenceEqual(y);
            }

            public int GetHashCode(IEnumerable<T> obj)
            {
                if (obj is null) return 0;
                unchecked
                {
                    int hash = 19;
                    foreach (var item in obj)
                        hash = hash * 31 + (item?.GetHashCode() ?? 0);
                    return hash;
                }
            }
        }

        private string[] GetDigsiPath(ISettingNodeBase settingNode)
        {
            // Use a stack-allocated list for efficiency
            var segments = new List<string>(8);

            // Traverse up the parent chain, collecting DisplayNames
            var currentNode = settingNode.Parent as IDigsiPathMember;
            while (currentNode != null)
            {
                segments.Add(currentNode.DisplayName);
                currentNode = currentNode.Parent as IDigsiPathMember;
            }

            // Add HmiTableName if present
            if (!string.IsNullOrEmpty(settingNode.HmiTableName))
                segments.Add(settingNode.HmiTableName);

            // Reverse to get correct order (root to leaf)
            segments.Reverse();

            return segments.ToArray();
        }
    }

}
