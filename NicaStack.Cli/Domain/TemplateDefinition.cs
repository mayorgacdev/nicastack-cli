namespace NicaStack.Cli.Domain;

internal sealed record TemplateDefinition(
    string Id,
    string Name,
    string Description,
    TemplateKind Kind,
    string Source,
    string? PackageName = null,
    string? ShortName = null,
    string? RepositoryUrl = null,
    string? NamespaceSeed = null);