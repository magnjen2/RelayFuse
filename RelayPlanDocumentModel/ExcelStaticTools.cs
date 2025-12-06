using ClosedXML.Excel;
using System.Text.RegularExpressions;

namespace RelayPlanDocumentModel
{
    public static class ExcelStaticTools
    {
        public static List<IXLRangeRows> GetRowSegments(
            IXLWorksheet worksheet,
            List<int> startRows,
            List<int> endRows,
            int columnCount = 17)
        {
            var segments = new List<IXLRangeRows>();
            for (int i = 0; i < startRows.Count; i++)
            {
                int startRow = startRows[i];
                int endRow = endRows.First(x => x > startRow);
                var rangeRows = worksheet.Range(startRow, 1, endRow, columnCount).Rows();
                segments.Add(rangeRows);
            }
            return segments;
        }

        public static List<int> FindDigsiPathRows(IXLRangeRows rows, int lastColumn = 17, bool strictMatch = true)
        {
            var result = new List<int>();

            var cellBRegex = new Regex(@"^[\d\.]+(?:=>)?$");

            foreach (var row in rows)
            {
                var cellB = row.Cell(2);
                string cellBstring = Regex.Replace(cellB.GetString(), @"\([^)]*\)", string.Empty)
                                                            .Replace(" ", string.Empty)
                                                            .Replace("\t", string.Empty);

                var cellD = row.Cell(4);
                string cellDstring = cellD.GetString();


                var cellL = row.Cell(12);
                string cellLstring = cellL.GetString();


                bool BisMatch = cellBRegex.IsMatch(cellBstring);
                bool DHaveContent = !string.IsNullOrEmpty(cellDstring);
                bool LisMatch = string.IsNullOrEmpty(cellLstring) || cellLstring.ToLower().Contains("setting group");

                bool otherCellsAreEmpty = true;
                for (int col = 1; col <= lastColumn; col++)
                {
                    if (col == 2 || col == 4 || col == 12) continue;
                    if (!string.IsNullOrWhiteSpace(row.Cell(col).GetString()))
                    {
                        otherCellsAreEmpty = false;
                        break;
                    }
                }

                if (!otherCellsAreEmpty)
                {
                    continue;
                }

                if (BisMatch && DHaveContent && LisMatch)
                {
                    result.Add(row.RowNumber()); continue;
                }

                if (!strictMatch && string.IsNullOrEmpty(cellBstring) && DHaveContent && LisMatch)
                {
                    result.Add(row.RowNumber()); continue;
                }

                if (otherCellsAreEmpty && DHaveContent && (string.IsNullOrEmpty(cellBstring) || !BisMatch))
                    throw new Exception("The digsi-path search function is not working reliably.");

                if (otherCellsAreEmpty && DHaveContent && (string.IsNullOrEmpty(cellBstring) || !BisMatch) && !LisMatch)
                {
                    Console.WriteLine();
                }

            }
            return result;
        }

        public static List<int> FindUnusedRowSections(IXLWorksheet worksheet, int minConsecutive = 3)
        {
            var unusedSections = new List<int>();
            int consecutiveUnused = 0;
            int sectionStartRow = -1;

            // Get the used range, or use the worksheet's row count if none
            int lastRow = worksheet.LastRowUsed()?.RowNumber() ?? worksheet.Rows().Count();

            for (int rowNum = 1; rowNum <= lastRow; rowNum++)
            {
                var row = worksheet.Row(rowNum);
                bool isUnused = row.Cells().All(cell =>
                    string.IsNullOrWhiteSpace(cell.GetString()) &&
                    cell.Value.Type != ClosedXML.Excel.XLDataType.Number);

                if (isUnused)
                {
                    if (consecutiveUnused == 0)
                        sectionStartRow = rowNum;
                    consecutiveUnused++;
                }
                else
                {
                    if (consecutiveUnused >= minConsecutive)
                        unusedSections.Add(sectionStartRow);
                    consecutiveUnused = 0;
                    sectionStartRow = -1;
                }
            }

            // Check for unused section at the end
            if (consecutiveUnused >= minConsecutive)
                unusedSections.Add(sectionStartRow);

            return unusedSections;
        }
    }
}

