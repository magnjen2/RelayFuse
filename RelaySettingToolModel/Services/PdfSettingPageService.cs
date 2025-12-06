using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Graphics;
using UglyToad.PdfPig.Tokenization;
using static UglyToad.PdfPig.Core.PdfSubpath;

namespace RelaySettingToolModel
{
    public class PdfSettingPageService
    {
        public PdfSettingPageService()
        {




        }
        public void ProcessSettingPage(IPdfSettingPage settingPage)
        {
            // Gets all continuous lines of words on the page
            List<List<Word>> continuousLines = GeneralPdfServices.GetContinuousLinesOfWords(settingPage.Page);

            var columns = GeneralPdfServices.GetColumnsOfLinesByCount(continuousLines, 5);


            // Gets the setting paths (bold words in second column)
            settingPage.SettingTableEntries = this.GetSettingTableEntries(columns);



        }


        public List<IPdfSettingTableEntry> GetSettingTableEntries(List<(double, List<List<Word>>)> columns)
        {
            List<List<Word>> settingPaths = new List<List<Word>>();
            List<int> settingPathIndexes = new List<int>();

            var addressCol = columns[0].Item2;
            var disp_pathCol = columns[1].Item2;
            var valueCol = columns[2].Item2;
            var unitCol = columns[3].Item2;
            var commentCol = columns[4].Item2;


            foreach (List<Word> line in disp_pathCol)
            {
                List<Word> filteredLine = line.Where(w => !w.Letters.Any(l => l.Font.IsItalic)).ToList();

                bool notAllowedWord = filteredLine.Any(w => w.Text.Contains("Display-tekst:"));
                bool allBold = filteredLine.All(w => w.Letters.All(l => l.Font.IsBold));

                if (allBold && !notAllowedWord)
                {
                    settingPaths.Add(filteredLine);
                }
            }

            List<IPdfSettingTableEntry> settingTableEntries = new List<IPdfSettingTableEntry>();

            // Iterator for each setting-table path
            for (int i = 0; i < settingPaths.Count; i++)
            {
                var settingPath = settingPaths[i];

                // Find the y-coordinate of the display name (first word in settingPath)
                double? yTop = settingPaths[i].FirstOrDefault()?.BoundingBox.Bottom;
                double? yBottom;
                // Find the next display name y-coordinate (or set to 0 if last)
                if (i + 1 < settingPaths.Count)
                {
                    yBottom = settingPaths[i + 1].FirstOrDefault()?.BoundingBox.Top;
                }
                else
                {
                    yBottom = 0;
                }

                    List<List<Word>> displaynames = disp_pathCol.Where(x => x.First().BoundingBox.Top < yTop && x.First().BoundingBox.Bottom > yBottom).ToList();
                List<List<Word>> addresses = addressCol.Where(x => x.First().BoundingBox.Top < yTop && x.First().BoundingBox.Bottom > yBottom).ToList();
                List<List<Word>> values = valueCol.Where(x => x.First().BoundingBox.Top < yTop && x.First().BoundingBox.Bottom > yBottom).ToList();
                List<List<Word>> units = unitCol.Where(x => x.First().BoundingBox.Top < yTop && x.First().BoundingBox.Bottom > yBottom).ToList();
                List<List<Word>> comments = commentCol.Where(x => x.First().BoundingBox.Top < yTop && x.First().BoundingBox.Bottom > yBottom).ToList();


                // Iteeares once for each setting. Each setting is defined by a display name and its corresponding address and value.
                List<PdfRelaySetting> settings = new List<PdfRelaySetting>();
                for (int j = 0; j < displaynames.Count; j++)
                {
                    List<Word> displayName = displaynames[j];

                    double settingTop = displaynames[j].Last().BoundingBox.Top + 0.5;

                    double? settingBottom;

                    if (j + 1 < displaynames.Count)
                    {
                        settingBottom = displaynames[j + 1].Last().BoundingBox.Top - 0.5;
                    }
                    else
                    {
                        settingBottom = yBottom;
                    }

                    List<Word>? address = addresses.FirstOrDefault(a => Math.Abs(a.First().BoundingBox.Top - settingTop) < 1.0);

                    List<List<Word>> possibleValues = values.Where(v =>
                                            v.Last().BoundingBox.Top <= settingTop
                                         && v.Last().BoundingBox.Bottom >= settingBottom).ToList();


                    settings.Add(new PdfRelaySetting(address, displayName, possibleValues));
                }

                settingTableEntries.Add(new PdfSettingTableEntry(settingPath, settings));
            }

            return settingTableEntries;
        }

    

        private static List<string> _settingLinesExclutionList = new List<string>
        {
                        "Kommentarer",
                        "Releinnstillinger",
                        "Markeres",
                        "Display-tekst:",
                        "Adresse:",
                        "Kraftsensitiv"
        };
    }


}
