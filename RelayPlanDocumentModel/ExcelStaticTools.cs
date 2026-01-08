using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            var stopwatch = Stopwatch.StartNew();
            if (worksheet == null) throw new ArgumentNullException(nameof(worksheet));
            if (startRows == null || startRows.Count == 0) throw new ArgumentException("startRows must contain at least one row.", nameof(startRows));
            if (endRows == null || endRows.Count == 0) throw new ArgumentException("endRows must contain at least one row.", nameof(endRows));
            if (columnCount <= 0) throw new ArgumentOutOfRangeException(nameof(columnCount), "columnCount must be positive.");

            var orderedStarts = startRows.OrderBy(r => r).ToList();
            var orderedEnds = endRows.OrderBy(r => r).ToList();

            var segments = new List<IXLRangeRows>();
            for (int i = 0; i < orderedStarts.Count; i++)
            {
                int startRow = orderedStarts[i];
                int? endRow = orderedEnds.FirstOrDefault(x => x > startRow);
                if (endRow is null || endRow.Value <= startRow)
                {
                    throw new InvalidOperationException($"No end row found after start row {startRow}. Ensure endRows contains a value greater than each start row.");
                }

                var rangeRows = worksheet.Range(startRow, 1, endRow.Value, columnCount).Rows();


                segments.Add(rangeRows);
            }
            stopwatch.Stop();
            Console.WriteLine($"GetRowSegments executed in {stopwatch.ElapsedMilliseconds} ms.");
            return segments;
        }

        public static List<int> FindDigsiPathRows(IXLRangeRows rows, int lastColumn = 17, bool strictMatch = true)
        {
            var stopwatch = Stopwatch.StartNew();
            if (rows == null) throw new ArgumentNullException(nameof(rows));
            if (lastColumn < 1) throw new ArgumentOutOfRangeException(nameof(lastColumn), "lastColumn must be at least 1.");

            var result = new List<int>();

            var cellBRegex = new Regex(@"^[\d\.]+(?:=>)?$");

            string CleanCellB(string value) => Regex.Replace(value, @"\([^)]*\)", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("\t", string.Empty);


            foreach (var row in rows)
            {


                // Not match if cell D is empty.
                var cellDstring = row.Cell(4).GetString();
                if(cellDstring == "Group Line1, DISTV1, General")
                {
                    Console.WriteLine("debug");
                }

                if (string.IsNullOrEmpty(cellDstring))
                {
                    continue;
                }

                // No match if cell H is not empty
                if (!string.IsNullOrEmpty(row.Cell(8).GetString()))
                {
                    continue;
                }

                // No match if cell K is not empty
                if (!string.IsNullOrEmpty(row.Cell(11).GetString()))
                {
                    continue;
                }

                // No match if cell L is not empty and does not contain the keyword
                string cellLstring = row.Cell(12).GetString();
                if (!string.IsNullOrEmpty(cellLstring))
                {
                    if (!cellLstring.ToLowerInvariant().Contains("setting group")) // NB: May need additional key words!!!
                    {
                        continue;
                    }
                }

                // No match if cell O is not empty and does not contain the keyword
                string cellOstring = row.Cell(15).GetString();
                if (!string.IsNullOrEmpty(cellOstring))
                {
                    if (!cellOstring.ToLowerInvariant().Contains("setting group")) // NB: May need additional key words!!!
                    {
                        continue;
                    }
                }

                var cellBstring = CleanCellB(row.Cell(2).GetString());

                if (cellBRegex.IsMatch(cellBstring))
                {
                    result.Add(row.RowNumber());
                    continue;
                }

            }

            stopwatch.Stop();
            Console.WriteLine($"FindDigsiPathRows executed in {stopwatch.ElapsedMilliseconds} ms.");
            return result;
        }

        public static List<int> FindUnusedRowSections(IXLWorksheet worksheet, int minConsecutive = 3)
        {
            var stopwatch = Stopwatch.StartNew();
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

            stopwatch.Stop();
            Console.WriteLine($"FindUnusedRowSections executed in {stopwatch.ElapsedMilliseconds} ms.");
            return unusedSections;
        }
    }
}

