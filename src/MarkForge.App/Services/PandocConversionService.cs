using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using MarkForge.App.Models;

namespace MarkForge.App.Services;

public sealed class PandocConversionService : IConversionService
{
    private readonly IDocxOrientationService _docxOrientationService;
    private readonly string _logsDirectoryPath;
    private readonly string _bundledLuaFiltersDirectoryPath;
    private readonly string _pandocExecutablePath;

    public PandocConversionService(
        IDocxOrientationService docxOrientationService,
        string? appDataRoot = null,
        string? bundledLuaFiltersDirectoryPath = null,
        string pandocExecutablePath = "pandoc")
    {
        _docxOrientationService = docxOrientationService;
        var basePath = appDataRoot
            ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _logsDirectoryPath = Path.Combine(basePath, "MarkForge", "logs");
        _bundledLuaFiltersDirectoryPath = bundledLuaFiltersDirectoryPath
            ?? Path.Combine(AppContext.BaseDirectory, "ConverterAssets", "filters");
        _pandocExecutablePath = pandocExecutablePath;
    }

    public async Task<ConversionResult> RunAsync(
        ConversionRequest request,
        IProgress<string> progress,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(progress);

        EnsureValidRequest(request);

        Directory.CreateDirectory(request.OutputFolderPath);
        Directory.CreateDirectory(_logsDirectoryPath);

        var outputDocxPath = Path.Combine(
            request.OutputFolderPath,
            $"{Path.GetFileNameWithoutExtension(request.InputFilePath)}.docx");
        var detailedLogPath = BuildLogPath();

        await using var logWriter = new StreamWriter(
            detailedLogPath,
            append: false,
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        void WriteLogLine(string message)
        {
            logWriter.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}");
            logWriter.Flush();
        }

        void ReportAndLog(string message)
        {
            progress.Report(message);
            WriteLogLine(message);
        }

        try
        {
            ReportAndLog("Preparing Pandoc conversion.");
            WriteLogLine($"Input markdown file: {request.InputFilePath}");
            WriteLogLine($"Output DOCX file: {outputDocxPath}");
            WriteLogLine($"Reference template: {request.ReferenceTemplatePath ?? "(none)"}");
            WriteLogLine($"TOC enabled: {request.IncludeTableOfContents}");
            WriteLogLine($"Requested orientation: {(request.UseLandscapeOrientation ? "Landscape" : "Portrait")}");
            WriteLogLine($"Highlight style: {request.HighlightStyle}");

            var luaFilterPaths = ResolveLuaFilterPaths(request);
            WriteLogLine(luaFilterPaths.Count == 0
                ? "Lua filters: none"
                : $"Lua filters: {string.Join(", ", luaFilterPaths)}");

            var arguments = BuildPandocArguments(request, outputDocxPath, luaFilterPaths);
            WriteLogLine($"Pandoc command: {FormatCommand(_pandocExecutablePath, arguments)}");

            ReportAndLog("Running Pandoc...");
            var processResult = await RunPandocProcessAsync(arguments, cancellationToken);

            if (!string.IsNullOrWhiteSpace(processResult.StandardOutput))
            {
                WriteLogLine("Pandoc stdout:");
                WriteLogLine(processResult.StandardOutput);
            }

            if (!string.IsNullOrWhiteSpace(processResult.StandardError))
            {
                WriteLogLine("Pandoc stderr:");
                WriteLogLine(processResult.StandardError);
            }

            if (processResult.ExitCode != 0)
            {
                throw new ConversionRunException(
                    $"Pandoc failed with exit code {processResult.ExitCode}. See detailed log.",
                    detailedLogPath);
            }

            ReportAndLog("Pandoc conversion completed.");
            ReportAndLog("Applying DOCX orientation post-processing...");
            _docxOrientationService.ApplyOrientation(outputDocxPath, request.UseLandscapeOrientation);
            ReportAndLog($"DOCX orientation applied: {(request.UseLandscapeOrientation ? "Landscape" : "Portrait")}.");

            ReportAndLog($"Conversion finished. Output: {outputDocxPath}");
            ReportAndLog($"Detailed log: {detailedLogPath}");

            return new ConversionResult
            {
                OutputDocxPath = outputDocxPath,
                DetailedLogPath = detailedLogPath
            };
        }
        catch (ConversionRunException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            WriteLogLine("Conversion canceled by request.");
            throw new ConversionRunException("Conversion canceled.", detailedLogPath);
        }
        catch (Exception ex)
        {
            WriteLogLine("Unexpected conversion failure:");
            WriteLogLine(ex.ToString());
            throw new ConversionRunException("Conversion failed unexpectedly. See detailed log.", detailedLogPath, ex);
        }
    }

    private static void EnsureValidRequest(ConversionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.InputFilePath))
        {
            throw new InvalidOperationException("Input markdown file is required.");
        }

        if (!File.Exists(request.InputFilePath))
        {
            throw new FileNotFoundException("Input markdown file does not exist.", request.InputFilePath);
        }

        if (string.IsNullOrWhiteSpace(request.OutputFolderPath))
        {
            throw new InvalidOperationException("Output folder is required.");
        }

        if (!string.IsNullOrWhiteSpace(request.ReferenceTemplatePath) && !File.Exists(request.ReferenceTemplatePath))
        {
            throw new FileNotFoundException(
                "Reference template file does not exist.",
                request.ReferenceTemplatePath);
        }
    }

    private List<string> ResolveLuaFilterPaths(ConversionRequest request)
    {
        var preferredPath = string.IsNullOrWhiteSpace(request.LuaFiltersDirectoryPath)
            ? _bundledLuaFiltersDirectoryPath
            : request.LuaFiltersDirectoryPath!;

        if (!Directory.Exists(preferredPath))
        {
            if (string.IsNullOrWhiteSpace(request.LuaFiltersDirectoryPath))
            {
                return new List<string>();
            }

            throw new DirectoryNotFoundException(
                $"Lua filter directory does not exist: {request.LuaFiltersDirectoryPath}");
        }

        return Directory
            .GetFiles(preferredPath, "*.lua", SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static List<string> BuildPandocArguments(
        ConversionRequest request,
        string outputDocxPath,
        IEnumerable<string> luaFilterPaths)
    {
        var arguments = new List<string>
        {
            request.InputFilePath,
            "--from",
            "markdown",
            "--to",
            "docx",
            "--output",
            outputDocxPath
        };

        if (request.IncludeTableOfContents)
        {
            arguments.Add("--toc");
        }

        if (!string.IsNullOrWhiteSpace(request.ReferenceTemplatePath))
        {
            arguments.Add("--reference-doc");
            arguments.Add(request.ReferenceTemplatePath);
        }

        if (string.Equals(request.HighlightStyle, "none", StringComparison.OrdinalIgnoreCase))
        {
            arguments.Add("--no-highlight");
        }
        else
        {
            arguments.Add("--highlight-style");
            arguments.Add(request.HighlightStyle);
        }

        foreach (var luaFilterPath in luaFilterPaths)
        {
            arguments.Add("--lua-filter");
            arguments.Add(luaFilterPath);
        }

        return arguments;
    }

    private async Task<ProcessResult> RunPandocProcessAsync(
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _pandocExecutablePath,
            WorkingDirectory = Environment.CurrentDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process { StartInfo = startInfo };
        try
        {
            process.Start();
        }
        catch (Win32Exception ex)
        {
            throw new ConversionRunException(
                "Pandoc executable was not found. Install Pandoc and ensure it is available on PATH.",
                innerException: ex);
        }

        var standardOutputTask = process.StandardOutput.ReadToEndAsync();
        var standardErrorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync(cancellationToken);
        var standardOutput = await standardOutputTask;
        var standardError = await standardErrorTask;

        return new ProcessResult(process.ExitCode, standardOutput, standardError);
    }

    private string BuildLogPath()
    {
        var stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff");
        return Path.Combine(_logsDirectoryPath, $"conversion-{stamp}.log");
    }

    private static string FormatCommand(string executable, IEnumerable<string> arguments)
    {
        return $"{executable} {string.Join(" ", arguments.Select(QuoteForDisplay))}";
    }

    private static string QuoteForDisplay(string value)
    {
        if (!value.Contains(' ') && !value.Contains('"'))
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\\\"")}\"";
    }

    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}
