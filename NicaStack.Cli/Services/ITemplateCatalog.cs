using NicaStack.Cli.Domain;

namespace NicaStack.Cli.Services;

internal interface ITemplateCatalog
{
    IReadOnlyList<TemplateDefinition> GetAll();
    TemplateDefinition? FindById(string id);
}