using FormsDialogResult = System.Windows.Forms.DialogResult;
using FormsFolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using Win32OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace MarkForge.App.Services;

public sealed class FileDialogService : IFileDialogService
{
    public string? PickMarkdownFile(string? initialPath = null)
    {
        var dialog = new Win32OpenFileDialog
        {
            CheckFileExists = true,
            Filter = "Markdown files (*.md)|*.md|All files (*.*)|*.*",
            Title = "Select markdown file"
        };

        if (!string.IsNullOrWhiteSpace(initialPath))
        {
            dialog.FileName = initialPath;
        }

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? PickReferenceTemplate(string? initialPath = null)
    {
        var dialog = new Win32OpenFileDialog
        {
            CheckFileExists = true,
            Filter = "Word templates (*.docx)|*.docx|All files (*.*)|*.*",
            Title = "Select reference template"
        };

        if (!string.IsNullOrWhiteSpace(initialPath))
        {
            dialog.FileName = initialPath;
        }

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? PickFolder(string? initialPath = null)
    {
        using var dialog = new FormsFolderBrowserDialog
        {
            Description = "Select output folder",
            ShowNewFolderButton = true
        };

        if (!string.IsNullOrWhiteSpace(initialPath))
        {
            dialog.SelectedPath = initialPath;
        }
        else
        {
            dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        return dialog.ShowDialog() == FormsDialogResult.OK ? dialog.SelectedPath : null;
    }
}
