namespace MarkForge.App.Services;

public interface IDocxOrientationService
{
    void ApplyOrientation(string docxPath, bool useLandscapeOrientation);
}
