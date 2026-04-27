using NicaStack.Cli.Domain;

namespace NicaStack.Cli.Services;

internal sealed class TemplateCatalog : ITemplateCatalog
{
    private static readonly IReadOnlyList<TemplateDefinition> Templates =
    [
        new(
            "ardalis-full",
            "Ardalis Full Clean Architecture",
            "Solucion multi-proyecto con capas Domain, Application, Infrastructure y Web.",
            TemplateKind.DotnetTemplate,
            "NuGet",
            PackageName: "Ardalis.CleanArchitecture.Template",
            ShortName: "clean-arch"),
        new(
            "ardalis-minimal",
            "Ardalis Minimal Clean",
            "Plantilla ligera orientada a API y vertical slices.",
            TemplateKind.DotnetTemplate,
            "NuGet",
            PackageName: "Ardalis.MinimalClean.Template",
            ShortName: "min-clean"),
        new(
            "vertical-slice-github",
            "Nadirbad Vertical Slice",
            "Clona un repositorio base y reemplaza el namespace raiz.",
            TemplateKind.GitClone,
            "GitHub",
            RepositoryUrl: "https://github.com/nadirbad/VerticalSliceArchitecture.git",
            NamespaceSeed: "VerticalSliceArchitecture"),
    ];

    public IReadOnlyList<TemplateDefinition> GetAll() => Templates;

    public TemplateDefinition? FindById(string id) =>
        Templates.FirstOrDefault(template => string.Equals(template.Id, id, StringComparison.OrdinalIgnoreCase));
}