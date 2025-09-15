using UglyToad.PdfPig.Content;



namespace RelaySettingToolModel
{
    public static class GeneralPdfServices
    {
        public static string JoinWordsToString(List<Word> words)
        {
            // Order words by their leftmost X coordinate to ensure left-to-right joining
            var orderedWords = words.OrderBy(w => w.BoundingBox.Left).ToList();
            return string.Join("", orderedWords.Select(w => w.Text));
        }

        public static List<string> MakeCVSStrings(List<Word> words)
        {
            var pathText = JoinWordsToString(words);
            // Split pathText into pathStrings using ',' as separator.
            // Remove any text inside parentheses and trim each string.
            return pathText
                        .Split(',')
                        .Select(s => System.Text.RegularExpressions.Regex.Replace(s, @"\s*\(.*?\)\s*", ""))
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList();
        }

        public static List<List<Word>> GetRowsOfWords(Page page, double tolerance = 10.5)
        {
            List<Word> words = page.GetWords().ToList();

            return words.GroupBy(w => Math.Round((w.BoundingBox.Top + (w.BoundingBox.Bottom - w.BoundingBox.Top) / 2) / tolerance) * tolerance)
                        .OrderByDescending(g => g.Key)
                        .Select(g => g.OrderBy(w => w.BoundingBox.Left).ToList())
                        .ToList();

        }


        public static List<List<Word>> GetContinuousLinesOfWords(Page page, double maxSpaceTolerance = 17.0, double rowTolerance = 10.5)
        {
            // Get all rows of words using the existing method
            var rows = GetRowsOfWords(page, rowTolerance);
            var result = new List<List<Word>>();

            foreach (var row in rows)
            {
                if (row.Count == 0)
                    continue;

                // Sort words left-to-right
                var orderedRow = row.OrderBy(w => w.BoundingBox.Left).ToList();
                var currentLine = new List<Word> { orderedRow[0] };

                for (int i = 1; i < orderedRow.Count; i++)
                {
                    var prevWord = orderedRow[i - 1];
                    var currWord = orderedRow[i];

                    // Calculate horizontal space between words
                    double space = currWord.BoundingBox.Left - prevWord.BoundingBox.Right;

                    if (space > maxSpaceTolerance)
                    {
                        // Start a new line if space is too large
                        result.Add(currentLine);
                        currentLine = new List<Word>();
                    }
                    currentLine.Add(currWord);
                }
                // Add the last line in the row
                if (currentLine.Count > 0)
                    result.Add(currentLine.OrderBy(x => x.BoundingBox.Left).ToList());
            }

            return result;
        }


        public static List<(double X, List<List<Word>> Columns)> GetColumnsOfLinesByCount(List<List<Word>> allLines, int topCount, double tolerance = 30.0)
        {

            // Group words by column X coordinate (using tolerance)
            var columnGroups = new List<(double X, List<List<Word>> Columns)>();
            for (int i = 0; i < allLines.Count; i++)
            {
                Word word = allLines[i].First();

                // Try to find an existing column group within tolerance
                var group = columnGroups.FirstOrDefault(g => Math.Abs(g.X - word.BoundingBox.Left) <= tolerance);
                if (group.Columns == null)
                {
                    // Create new group
                    columnGroups.Add((word.BoundingBox.Left, new List<List<Word>> { allLines[i] }));
                }
                else
                {
                    // Add word to existing group
                    group.Columns.Add(allLines[i]);
                }
            }

            // Order columns by word count descending, take top N, and return their X coordinates ordered by increasing value
            return columnGroups
                .OrderByDescending(g => g.Columns.Count)
                .Take(topCount)
                .OrderBy(x => x)
                .ToList();
        }

        //public static List<double> GetColumnLeftXCoordinates(Page page, double tolerance = 30.0)
        //{
        //    // Get all words on the page
        //    List<Word> words = page.GetWords().ToList();

        //    // Get all left X coordinates
        //    List<double> leftXs = words.Select(w => w.BoundingBox.Left).ToList();

        //    // Group X coordinates by proximity (tolerance)
        //    List<double> columnXs = new List<double>();
        //    foreach (double x in leftXs.OrderBy(x => x))
        //    {
        //        // If this x is not close to any existing column, add it as a new column
        //        if (!columnXs.Any(colX => Math.Abs(colX - x) <= tolerance))
        //        {
        //            columnXs.Add(x);
        //        }
        //    }
        //    return columnXs;
        //}

        //public static List<double> GetColumnLeftByWordCount(Page page, int topCount, double tolerance = 30.0)
        //{
        //    // Get all words on the page
        //    List<Word> words = page.GetWords().ToList();

        //    // Group words by column X coordinate (using tolerance)
        //    var columnGroups = new List<(double X, List<Word> Words)>();
        //    foreach (var word in words.OrderBy(w => w.BoundingBox.Left))
        //    {
        //        // Try to find an existing column group within tolerance
        //        var group = columnGroups.FirstOrDefault(g => Math.Abs(g.X - word.BoundingBox.Left) <= tolerance);
        //        if (group.Words == null)
        //        {
        //            // Create new group
        //            columnGroups.Add((word.BoundingBox.Left, new List<Word> { word }));
        //        }
        //        else
        //        {
        //            // Add word to existing group
        //            group.Words.Add(word);
        //        }
        //    }

        //    // Order columns by word count descending, take top N, and return their X coordinates ordered by increasing value
        //    return columnGroups
        //        .OrderByDescending(g => g.Words.Count)
        //        .Take(topCount)
        //        .Select(g => g.X)
        //        .OrderBy(x => x)
        //        .ToList();
        //}
    }
}