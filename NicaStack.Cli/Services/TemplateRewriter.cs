using System.Text;

namespace NicaStack.Cli.Services;

internal sealed class TemplateRewriter : ITemplateRewriter
{
    public void Rewrite(string rootPath, string oldName, string newName)
    {
        var allFiles = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories)
            .Where(file => !ShouldSkip(file))
            .ToArray();

        foreach (var file in allFiles)
        {
            TryReplaceFileContent(file, oldName, newName);
        }

        var allDirectories = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories)
            .OrderByDescending(path => path.Length)
            .ToArray();

        foreach (var directory in allDirectories)
        {
            var directoryName = Path.GetFileName(directory);
            if (!directoryName.Contains(oldName, StringComparison.Ordinal))
            {
                continue;
            }

            var parent = Directory.GetParent(directory)?.FullName;
            if (parent is null)
            {
                continue;
            }

            var renamedDirectory = Path.Combine(parent, directoryName.Replace(oldName, newName, StringComparison.Ordinal));
            Directory.Move(directory, renamedDirectory);
        }

        allFiles = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories)
            .Where(file => !ShouldSkip(file))
            .ToArray();

        foreach (var file in allFiles)
        {
            var fileName = Path.GetFileName(file);
            if (!fileName.Contains(oldName, StringComparison.Ordinal))
            {
                continue;
            }

            var parent = Directory.GetParent(file)?.FullName;
            if (parent is null)
            {
                continue;
            }

            var renamedFile = Path.Combine(parent, fileName.Replace(oldName, newName, StringComparison.Ordinal));
            File.Move(file, renamedFile);
        }
    }

    private static void TryReplaceFileContent(string filePath, string oldName, string newName)
    {
        try
        {
            var content = File.ReadAllText(filePath, Encoding.UTF8);
            if (content.Contains(oldName, StringComparison.Ordinal))
            {
                File.WriteAllText(filePath, content.Replace(oldName, newName, StringComparison.Ordinal), Encoding.UTF8);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (DecoderFallbackException)
        {
        }
    }

    private static bool ShouldSkip(string path)
    {
        var separator = Path.DirectorySeparatorChar;
        var normalized = path.Replace(Path.AltDirectorySeparatorChar, separator);

        return normalized.Contains($"{separator}bin{separator}", StringComparison.OrdinalIgnoreCase)
            || normalized.Contains($"{separator}obj{separator}", StringComparison.OrdinalIgnoreCase)
            || normalized.Contains($"{separator}.git{separator}", StringComparison.OrdinalIgnoreCase);
    }
}