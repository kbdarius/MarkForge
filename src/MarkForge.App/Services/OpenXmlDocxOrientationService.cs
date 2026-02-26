using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Linq;

namespace MarkForge.App.Services;

public sealed class OpenXmlDocxOrientationService : IDocxOrientationService
{
    private const uint PortraitWidthTwips = 12240;
    private const uint PortraitHeightTwips = 15840;

    public void ApplyOrientation(string docxPath, bool useLandscapeOrientation)
    {
        if (!File.Exists(docxPath))
        {
            throw new FileNotFoundException("DOCX output file was not found for orientation post-processing.", docxPath);
        }

        using var document = WordprocessingDocument.Open(docxPath, true);
        var body = document.MainDocumentPart?.Document?.Body
            ?? throw new InvalidOperationException("DOCX output does not contain a valid document body.");

        var sections = body.Descendants<SectionProperties>().ToList();
        if (sections.Count == 0)
        {
            var newSection = new SectionProperties();
            body.AppendChild(newSection);
            sections.Add(newSection);
        }

        foreach (var section in sections)
        {
            ApplyOrientation(section, useLandscapeOrientation);
        }

        document.MainDocumentPart!.Document.Save();
    }

    private static void ApplyOrientation(SectionProperties section, bool useLandscapeOrientation)
    {
        var pageSize = section.GetFirstChild<PageSize>();
        if (pageSize is null)
        {
            pageSize = section.PrependChild(new PageSize
            {
                Width = PortraitWidthTwips,
                Height = PortraitHeightTwips
            });
        }

        var width = pageSize.Width?.Value ?? PortraitWidthTwips;
        var height = pageSize.Height?.Value ?? PortraitHeightTwips;

        if (useLandscapeOrientation)
        {
            var longer = Math.Max(width, height);
            var shorter = Math.Min(width, height);
            pageSize.Width = longer;
            pageSize.Height = shorter;
            pageSize.Orient = PageOrientationValues.Landscape;
            return;
        }

        var portraitWidth = Math.Min(width, height);
        var portraitHeight = Math.Max(width, height);
        pageSize.Width = portraitWidth;
        pageSize.Height = portraitHeight;
        pageSize.Orient = PageOrientationValues.Portrait;
    }
}
