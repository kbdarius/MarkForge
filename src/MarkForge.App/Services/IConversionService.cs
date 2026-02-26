using MarkForge.App.Models;

namespace MarkForge.App.Services;

public interface IConversionService
{
    Task RunAsync(
        ConversionRequest request,
        IProgress<string> progress,
        CancellationToken cancellationToken = default);
}
