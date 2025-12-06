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
                .Where(vm => vm.HmiTable != null && vm.HmiTable.MatchingTable == null)
                .ToList();

            var rpTables = rpSideVM.HmiTableVMs
                .Where(vm => vm.HmiTable != null && vm.HmiTable.MatchingTable == null)
                .ToList();

            // Use HashSet for efficient removal
            var teaxSet = new HashSet<IHmiTableViewModel>(teaxTables);
            var rpSet = new HashSet<IHmiTableViewModel>(rpTables);

            foreach (var teaxHmiTable in teaxTables.ToList())
            {
                if (!teaxSet.Contains(teaxHmiTable))
                    continue;



                foreach (var rpHmiTable in rpSet)
                {
                    // TryMatchTable returns a confidence score (assumed signature: double TryMatchTable(IHmiTable a, IHmiTable b))
                    int[] matchConfidence = CompareService.TryMatchTable(teaxHmiTable, rpHmiTable);
                    if (matchConfidence[0] > 0 && matchConfidence[1] > 0)
                    {
                        teaxHmiTable.HmiTable.MatchingTable = rpHmiTable.HmiTable;
                        rpHmiTable.HmiTable.MatchingTable = teaxHmiTable.HmiTable;

                        teaxSet.Remove(teaxHmiTable);
                        rpSet.Remove(rpHmiTable);

                        break; // Move to next teaxHmiTable
                    }
                }

            }
        }



        public static int[] TryMatchTable(IHmiTableViewModel hmiTable1, IHmiTableViewModel hmiTable2)
        {
            int[] matchConfidence = new int[2] { 0, 0 };

            if (CompareStringArray(hmiTable1.HmiTable.DigsiPathList, hmiTable2.HmiTable.DigsiPathList)) // if Digsi paths match between teax and relay plan
            {
                matchConfidence[0] = 1; // Matching digsi path is indicated


                foreach (var setting1 in hmiTable1.SettingViewModels)
                {
                    foreach (var setting2 in hmiTable2.SettingViewModels)
                    {
                        var settingMatch = TryMatchSetting(setting1, setting2); // Trying to match settings within the hmi tables

                        matchConfidence[1] += settingMatch[0] + settingMatch[1]; // Increasing match confidence based on settings matched
                    }
                }
            }

            return matchConfidence;
        }




        public static int[] TryMatchSetting(IRelaySettingViewModel setting1, IRelaySettingViewModel setting2)
        {
            int[] isMatch = new int[2] { 0, 0 };

            if (setting1.RelaySetting.UniqueId == setting2.RelaySetting.UniqueId)
            {
                isMatch[0] = 1;
            }
            if (setting1.RelaySetting.DisplayName == setting2.RelaySetting.DisplayName)
            {
                isMatch[1] = 1;
            }
            setting1.MatchingSettingVM = setting2;
            setting2.MatchingSettingVM = setting1;
            setting1.MatchConfidence = isMatch[0] + isMatch[1];
            setting2.MatchConfidence = isMatch[0] + isMatch[1];

            return isMatch;
        }




        public static bool CompareStringArray(string[]? list1, string[]? list2)
        {
            if (list1 == null || list2 == null || list1.Length != list2.Length)
            {
                return false;
            }

            for (int i = 0; i < list1.Length; i++)
            {
                if (string.IsNullOrEmpty(list1[i]) || string.IsNullOrEmpty(list2[i]) || !MatchString1(list1[i], list2[i]))
                {
                    return false;
                }

            }

            return true;
        }


        public static bool MatchString1(string string1, string string2)
        {
            // Remove all whitespace and convert to lower case
            string s1 = new string(string1.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();
            string s2 = new string(string2.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLowerInvariant();

            return s1 == s2;
        }


        // For matching strings with minor differences
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
    }
}
