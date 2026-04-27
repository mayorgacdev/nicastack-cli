namespace NicaStack.Cli.Domain;

internal sealed record ScaffoldRequest(
    string ProjectName,
    string TemplateId,
    string OutputPath,
    bool Force,
    bool DryRun,
    bool SkipInstall,
    bool GitInit,
    bool Verbose);