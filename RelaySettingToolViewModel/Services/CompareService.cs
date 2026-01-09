using DocumentFormat.OpenXml.Spreadsheet;
using RelayFuseInterfaces;
using RelayPlanDocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaySettingToolViewModel
{
    public static class CompareService
    {
        public static void MatchAllHmiTables(MergingToolViewModel mergingToolViewModel)
        {
            var tableMergerVMs = mergingToolViewModel.HmiTableMergers;

            foreach (var tableMergerVM in tableMergerVMs)
            {
                foreach (var nonMatchedTable in mergingToolViewModel.GetUnmatchedHmiTables())
                {
                    int[] matchConfidence = CompareService.TryMatchTable(tableMergerVM!, nonMatchedTable!);
                    if (matchConfidence[0] > 0)//&& matchConfidence[1] > 0)
                    {
                        tableMergerVM.ExcelHmiTable = nonMatchedTable;
                        tableMergerVM.MatchConfidence = matchConfidence;
                        break;
                    }
                }
            }
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

        public static int[] TryMatchTable(IHmiTableMergerViewModel hmiTableMerger, IHmiTableViewModel excelHmiTableVM)
        {
            int[] matchConfidence = new int[2] { 0, 0 };

            var digsiPathTeax = hmiTableMerger.TeaxHmiTable!.HmiTable.DigsiPathString;
            var rpDigsiPath = excelHmiTableVM.HmiTable.DigsiPathString;

            if (digsiPathTeax == null || digsiPathTeax.Length == 0 || rpDigsiPath == null || rpDigsiPath.Length == 0)
                return matchConfidence;

            // Clean rpDigsiPath using the helper
            rpDigsiPath = CleanStatnettSpecificPath(rpDigsiPath);

            var matchResult = MatchString(digsiPathTeax, rpDigsiPath);


            if (matchResult.isMatch) // if Digsi paths match between teax and relay plan
            {
                matchConfidence[0] = 1; // Matching digsi path is indicated

                MatchAllSettings(hmiTableMerger, excelHmiTableVM, matchConfidence);
            }

            return matchConfidence;

        }


        public static void MatchAllSettings(IHmiTableMergerViewModel hmiTableMerger, IHmiTableViewModel excelHmiTableVM, int[] matchConfidence)
        {
            foreach (var settingMerger1 in hmiTableMerger.SettingMergers)
            {
                foreach (var settingVM in excelHmiTableVM.RelaySettingViewModels)
                {
                    var settingMatch = TryMatchSetting(settingMerger1, settingVM); // Trying to match settings within the hmi tables

                    matchConfidence[1] += settingMatch; // Increasing match confidence based on settings matched
                }
            }
        }


        public static int TryMatchSetting(ISettingMergerViewModel settingMergerVM, IRelaySettingViewModel excelSettingVM)
        {
            IRelaySettingViewModel teaxSettingVM = settingMergerVM.TeaxRelaySettingVM!;
            IRelaySetting teaxRelaySetting = teaxSettingVM.RelaySetting;
            IRelaySetting excelRelaySetting = excelSettingVM.RelaySetting;

            int score = 0;

            (bool isMatch, bool isExact) uniqueIdMatch = MatchVisibleUniqueId(teaxRelaySetting.UniqueId, excelRelaySetting.UniqueId);

            (bool isMatch, bool isExact) nameMatch = MatchString(teaxRelaySetting.DisplayName, excelRelaySetting.DisplayName);

            (bool isMatch, bool isExact) valueMatch = (false, false);

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
                settingMergerVM.ExcelRelaySettingVM = excelSettingVM;
                settingMergerVM.MatchConfidence = score;

                valueMatch = MatchString(teaxRelaySetting.SelectedValue, excelRelaySetting.SelectedValue);

                teaxSettingVM.UniqueIdMatch = uniqueIdMatch.isExact;
                teaxSettingVM.DisplayNameMatch = nameMatch.isExact;
                teaxSettingVM.ValueMatch = valueMatch.isExact;
                excelSettingVM.UniqueIdMatch = uniqueIdMatch.isExact;
                excelSettingVM.DisplayNameMatch = nameMatch.isExact;
                excelSettingVM.ValueMatch = valueMatch.isExact;
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
