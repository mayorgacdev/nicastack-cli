namespace NicaStack.Cli.Domain;

internal sealed record ScaffoldResult(
    bool Success,
    string ProjectDirectory,
    IReadOnlyList<string> Steps,
    string Summary,
    string? ErrorMessage = null,
    IReadOnlyList<ExecutionResult>? Executions = null);