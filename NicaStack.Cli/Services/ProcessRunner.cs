using System.Diagnostics;
using NicaStack.Cli.Domain;

namespace NicaStack.Cli.Services;

internal sealed class ProcessRunner : IProcessRunner
{
    public ExecutionResult Run(string fileName, IEnumerable<string> arguments, string? workingDirectory = null)
    {
        var argumentList = arguments.ToArray();
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        if (!string.IsNullOrWhiteSpace(workingDirectory))
        {
            startInfo.WorkingDirectory = workingDirectory;
        }

        foreach (var argument in argumentList)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException($"No se pudo iniciar el proceso '{fileName}'.");

        var standardOutput = process.StandardOutput.ReadToEnd();
        var standardError = process.StandardError.ReadToEnd();

        process.WaitForExit();

        return new ExecutionResult(
            fileName,
            string.Join(' ', argumentList.Select(QuoteIfNeeded)),
            process.ExitCode,
            standardOutput.Trim(),
            standardError.Trim());
    }

    private static string QuoteIfNeeded(string value) =>
        value.Contains(' ', StringComparison.Ordinal) ? $"\"{value}\"" : value;
}