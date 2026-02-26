namespace MarkForge.App.Models;

public sealed class ConversionRunException : Exception
{
    public ConversionRunException(string message, string? detailedLogPath = null, Exception? innerException = null)
        : base(message, innerException)
    {
        DetailedLogPath = detailedLogPath;
    }

    public string? DetailedLogPath { get; }
}
