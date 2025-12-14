using DocumentFormat.OpenXml.Spreadsheet;
using RelayFuseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaySettingToolViewModel
{
    public static class CompareService
    {
        public static void MatchAllHmiTables(TeaxSideViewModel teaxSideVM, RPSideViewModel rpSideVM)
        {
            // Create working sets of IHmiTableViewModel where MatchingTable is null
            var teaxTables = teaxSideVM.HmiTableVMs
                .Where(vm => vm.HmiTable != null && vm.MatchingHmiTableVM == null)
                .ToList();

            var rpTables = rpSideVM.HmiTableVMs
                .Where(vm => vm.HmiTable != null && vm.MatchingHmiTableVM == null)
                .ToList();

            // Use HashSet for efficient removal
            var teaxSet = new HashSet<IHmiTableViewModel>(teaxTables);
            var rpSet = new HashSet<IHmiTableViewModel>(rpTables);

            foreach (var teaxHmiTable in teaxTables)
            {
                if (!teaxSet.Contains(teaxHmiTable))
                    continue;

                foreach (var rpHmiTable in rpTables)
                {
                    if (!rpSet.Contains(rpHmiTable))
                        continue;

                    // TryMatchTable returns a confidence score (assumed signature: double TryMatchTable(IHmiTable a, IHmiTable b))
                    int[] matchConfidence = CompareService.TryMatchTable(teaxHmiTable, rpHmiTable);
                    if (matchConfidence[0] > 0 && matchConfidence[1] > 0)
                    {
                        teaxHmiTable.MatchingHmiTableVM = rpHmiTable;
                        rpHmiTable.MatchingHmiTableVM = teaxHmiTable;

                        teaxSet.Remove(teaxHmiTable);
                        rpSet.Remove(rpHmiTable);

                        break; // Move to next teaxHmiTable
                    }
                }

            }
            Console.WriteLine("Debug");
        }



        // Helper to clean DigsiPath string in according to requirements
        private static string CleanStatnettSpecificPath(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string result = input.TrimStart();

            // Remove "Group " at the start
            if (result.StartsWith("Group "))
                result = result.Substring(6).TrimStart();

            // Remove everything from the first '(' (including the parenthesis and everything after)
            int parenIndex = result.IndexOf('(');
            if (parenIndex >= 0)
                result = result.Substring(0, parenIndex);

            return result;
        }

        public static int[] TryMatchTable(IHmiTableViewModel hmiTableTeax, IHmiTableViewModel hmiTableRp)
        {
            int[] matchConfidence = new int[2] { 0, 0 };

            var digsiPathTeax = hmiTableTeax.HmiTable.DigsiPathString;
            var digsiPathRp = hmiTableRp.HmiTable.DigsiPathString;

            if (digsiPathTeax == null || digsiPathTeax.Length == 0 || digsiPathRp == null || digsiPathRp.Length == 0)
                return matchConfidence;

            // Clean digsiPathRp using the helper
            digsiPathRp = CleanStatnettSpecificPath(digsiPathRp);

            //if (digsiPathTeax.Contains("Z1") && digsiPathRp.Contains("Z1"))
            //{
            //    Console.WriteLine("Debug");
            //}

            var matchResult = MatchString(digsiPathTeax, digsiPathRp);
            if (matchResult.isMatch && !matchResult.isExact)
            {
                Console.WriteLine("Debug");
            }

            if (matchResult.isMatch) // if Digsi paths match between teax and relay plan
            {
                matchConfidence[0] = 1; // Matching digsi path is indicated

                foreach (var setting1 in hmiTableTeax.SettingViewModels)
                {
                    foreach (var setting2 in hmiTableRp.SettingViewModels)
                    {
                        var settingMatch = TryMatchSetting(setting1, setting2); // Trying to match settings within the hmi tables

                        matchConfidence[1] += settingMatch; // Increasing match confidence based on settings matched
                    }
                }
            }

            return matchConfidence;
        }




        public static int TryMatchSetting(IRelaySettingViewModel setting1, IRelaySettingViewModel setting2)
        {
            int score = 0;

            var uniqueIdMatch = MatchVisibleUniqueId(setting1.RelaySetting.UniqueId, setting2.RelaySetting.UniqueId);

            var nameMatch = MatchString(setting1.RelaySetting.DisplayName, setting2.RelaySetting.DisplayName);

            if (uniqueIdMatch.isExact)
            {
                score += 2; // UniqueId match is strong indicator
            }
            else if (uniqueIdMatch.isMatch)
            {
                score += 1; // UniqueId partial match is weak indicator
            }

            if (nameMatch.isExact)
            {
                score += 2; // Name match is strong indicator
            }
            else if (nameMatch.isMatch)
            {
                score += 1; // Name partial match is weak indicator
            }

            if (score > 1)
            {
                setting1.MatchingSettingVM = setting2;
                setting2.MatchingSettingVM = setting1;
                setting1.MatchConfidence = score;
                setting2.MatchConfidence = score;
            }
            return score;
        }



        // -----------For matching strings with minor differences------------
        public static (bool isMatch, bool isExact) MatchString(string string1, string string2)
        {
            // Remove all whitespace and convert to lower case
            string s1 = new string(string1.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();
            string s2 = new string(string2.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();

            if (s1 == s2)
                return (true, true);

            // Allow one single character difference (Levenshtein distance == 1)
            if (IsOneEditAway(s1, s2))
                return (true, false);

            return (false, false);

            // Helper: returns true if strings are one edit (insert, delete, or substitute) away
            static bool IsOneEditAway(string a, string b)
            {
                int lenA = a.Length, lenB = b.Length;
                if (Math.Abs(lenA - lenB) > 1) return false;

                int i = 0, j = 0, edits = 0;
                while (i < lenA && j < lenB)
                {
                    if (a[i] != b[j])
                    {
                        if (++edits > 1) return false;
                        if (lenA > lenB) i++;
                        else if (lenA < lenB) j++;
                        else { i++; j++; }
                    }
                    else
                    {
                        i++; j++;
                    }
                }
                // Account for extra char at end
                if (i < lenA || j < lenB) edits++;
                return edits == 1;
            }
        }
        public static (bool isMatch, bool isExact) MatchVisibleUniqueId(string string1, string string2)
        {
            if (string.IsNullOrWhiteSpace(string1) || string.IsNullOrWhiteSpace(string2))
                return (false, false);

            var parts1 = string1.Split('.');
            var parts2 = string2.Split('.');

            // Exact match: all parts must match and count must be equal
            if (parts1.Length == parts2.Length && parts1.SequenceEqual(parts2))
                return (true, true);

            // Partial match: last two numbers must match (if both have at least two parts)
            if (parts1.Length >= 2 && parts2.Length >= 2)
            {
                if (parts1[^1] == parts2[^1] && parts1[^2] == parts2[^2])
                    return (true, false);
            }

            return (false, false);
        }
    }
}
