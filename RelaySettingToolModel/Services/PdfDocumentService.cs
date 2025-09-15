using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Graphics;
using UglyToad.PdfPig.Tokenization;



namespace RelaySettingToolModel
{
    public interface IRelayPlanPdfService
    {

    }
    public class PdfDocumentService
    {
        public PdfDocumentService() { }

        public void ProcessPdfDocument(PdfDocumentModel documentModel, List<string> targetFgNames)
        {
            // Create device list from table of contents on first page
            List<PdfDeviceModel> pdfDevices = GetDevicesFromToc(documentModel.Document.GetPage(1));

            // If there is no table of contents, there is only one device in the document
            if (pdfDevices.Count == 0)
            {
                pdfDevices.Add(new PdfDeviceModel("Unknown Device", "UnknownType", 1));
            }

            // Find setting pages for each device
            SearchForSettingPages(documentModel.Document, pdfDevices);

            var settingPageService = new PdfSettingPageService();

            foreach (PdfDeviceModel device in pdfDevices)
            {
                foreach (IPdfSettingPage settingPage in device.SettingPages)
                {
                    settingPageService.ProcessSettingPage(settingPage);
                }
            }
        }


        private void SearchForSettingPages(PdfDocument document, List<PdfDeviceModel> deviceList)
        {
            var regex1 = new Regex(@"^\d{4}\.\d{3}$");
            var regex2 = new Regex(@"^\d{2}\.\d{3}\.=>$");

            List<Page> settingPages = document.GetPages()
                .Where(p =>
                {
                    var words = p.GetWords().Select(w => w.Text.Trim()).ToList();

                    bool hasAdresse = words.Contains("Adresse:");
                    bool hasDisplayTekst = words.Contains("Display-tekst:");
                    bool hasReleinnstillinger = words.Contains("Releinnstillinger:");

                    int regexMatchCount = words.Count(word => regex1.IsMatch(word) || regex2.IsMatch(word));
                    bool regexMatch = regexMatchCount >= 2;

                    return regexMatch || (hasAdresse && hasDisplayTekst && hasReleinnstillinger);
                })
                .OrderBy(p => p.Number)
                .ToList();

            if (settingPages.Count == 0)
                return;

            var tocPageNumbers = deviceList.Select(dev => dev.TocStartPage).OrderBy(n => n).ToList();
            int deviceIdx = 0;
            int startIdx = 0;

            for (int i = 0; i < settingPages.Count; i++)
            {
                if (deviceIdx + 1 < deviceList.Count && settingPages[i].Number >= tocPageNumbers[deviceIdx + 1])
                {
                    var group = settingPages.GetRange(startIdx, i - startIdx)
                        .Select(p => new PdfSettingPage(p))
                        .Cast<IPdfSettingPage>()
                        .ToList();
                    deviceList[deviceIdx].SettingPages = group;
                    startIdx = i;
                    deviceIdx++;
                }
            }
            // Add last group
            if (startIdx < settingPages.Count && deviceIdx < deviceList.Count)
            {
                var group = settingPages.GetRange(startIdx, settingPages.Count - startIdx)
                    .Select(p => new PdfSettingPage(p))
                    .Cast<IPdfSettingPage>()
                    .ToList();
                deviceList[deviceIdx].SettingPages = group;
            }
        }



        public List<PdfDeviceModel> GetDevicesFromToc(Page firstPage)
        {
            var result = new List<PdfDeviceModel>();
            var words = firstPage.GetWords().ToList();

            Word? tocHeader = words.FirstOrDefault(w => w.Text.Contains("Innholdsfortegnelse"));
            Word? tocFooter = words.FirstOrDefault(w => w.Text.Contains("Informasjon"));

            if (tocHeader == null || tocFooter == null)
                return result;

            double left = firstPage.CropBox.Bounds.Left;
            double right = firstPage.CropBox.Bounds.Right;
            double top = tocHeader.BoundingBox.Bottom;
            double bottom = tocFooter.BoundingBox.Top;

            var tocRectangle1 = new PdfRectangle(left, bottom, left + ((right - left) / 2), top);
            var tocRectangle2 = new PdfRectangle(left + ((right - left) / 2), bottom, right, top);

            // Helper to get lines from words in a rectangle, in reverse order
            List<string> GetLinesFromRectangle(PdfRectangle rect)
            {
                var rectWords = words.Where(w =>
                    w.BoundingBox.Left >= rect.Left &&
                    w.BoundingBox.Right <= rect.Right &&
                    w.BoundingBox.Top <= rect.Top &&
                    w.BoundingBox.Bottom >= rect.Bottom
                ).OrderByDescending(w => w.BoundingBox.BottomLeft.Y) // reverse order
                 .ThenBy(w => w.BoundingBox.Left)
                 .ToList();

                var lines = new List<string>();
                var currentLine = new StringBuilder();
                double? lastY = null;
                foreach (var word in rectWords)
                {
                    if (lastY == null || Math.Abs(word.BoundingBox.BottomLeft.Y - lastY.Value) < 5)
                    {
                        currentLine.Append(word.Text + " ");
                    }
                    else
                    {
                        lines.Add(currentLine.ToString().Trim());
                        currentLine.Clear();
                        currentLine.Append(word.Text + " ");
                    }
                    lastY = word.BoundingBox.BottomLeft.Y;
                }
                if (currentLine.Length > 0)
                    lines.Add(currentLine.ToString().Trim());
                return lines;
            }

            // Read lines from each rectangle separately
            var lines1 = GetLinesFromRectangle(tocRectangle1);
            var lines2 = GetLinesFromRectangle(tocRectangle2);

            // Combine lines from both rectangles
            var allLines = lines1.Concat(lines2);

            foreach (var line in allLines)
            {
                var match = System.Text.RegularExpressions.Regex.Match(line, @"^(?<name>.+?)\s+(?<deviceType>\w+)\s+s\.\s*(?<page>\d+)");
                if (match.Success)
                {
                    var tocName = match.Groups["name"].Value.Trim();
                    var deviceType = match.Groups["deviceType"].Value.Trim();
                    if (int.TryParse(match.Groups["page"].Value, out int tocPage))
                    {
                        result.Add(new PdfDeviceModel(tocName, deviceType, tocPage));
                    }
                }
            }
            return result;
        }






    }
}
