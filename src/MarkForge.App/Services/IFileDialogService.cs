namespace MarkForge.App.Services;

public interface IFileDialogService
{
    string? PickMarkdownFile(string? initialPath = null);

    string? PickReferenceTemplate(string? initialPath = null);

    string? PickFolder(string? initialPath = null);
}
