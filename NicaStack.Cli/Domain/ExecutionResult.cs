namespace NicaStack.Cli.Domain;

internal sealed record ExecutionResult(
    string FileName,
    string Arguments,
    int ExitCode,
    string StandardOutput,
    string StandardError)
{
    public bool IsSuccess => ExitCode == 0;

    public string CommandText => string.Join(' ', new[] { FileName, Arguments }.Where(part => !string.IsNullOrWhiteSpace(part)));
}