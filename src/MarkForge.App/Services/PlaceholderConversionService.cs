using MarkForge.App.Models;
using System.IO;

namespace MarkForge.App.Services;

public sealed class PlaceholderConversionService : IConversionService
{
    public async Task RunAsync(
        ConversionRequest request,
        IProgress<string> progress,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(progress);

        if (string.IsNullOrWhiteSpace(request.InputFilePath))
        {
            throw new InvalidOperationException("Input markdown file is required.");
        }

        if (string.IsNullOrWhiteSpace(request.OutputFolderPath))
        {
            throw new InvalidOperationException("Output folder is required.");
        }

        Directory.CreateDirectory(request.OutputFolderPath);

        progress.Report("Preparing conversion pipeline (placeholder).");
        await Task.Delay(250, cancellationToken);

        progress.Report($"Input: {request.InputFilePath}");
        await Task.Delay(150, cancellationToken);

        progress.Report($"Output folder: {request.OutputFolderPath}");
        await Task.Delay(150, cancellationToken);

        progress.Report($"TOC enabled: {request.IncludeTableOfContents}");
        progress.Report($"Landscape orientation: {request.UseLandscapeOrientation}");
        progress.Report($"Highlight style: {request.HighlightStyle}");
        await Task.Delay(150, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.ReferenceTemplatePath))
        {
            progress.Report($"Reference template: {request.ReferenceTemplatePath}");
        }
        else
        {
            progress.Report("Reference template not set.");
        }

        var simulatedOutputPath = Path.Combine(
            request.OutputFolderPath,
            $"{Path.GetFileNameWithoutExtension(request.InputFilePath)}.docx");
        progress.Report($"Simulated output target: {simulatedOutputPath}");

        await Task.Delay(300, cancellationToken);
        progress.Report("Phase 0 placeholder conversion finished (no DOCX generated yet).");
    }
}
