using NicaStack.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NicaStack.Cli.Commands;

internal sealed class ListCommand(ITemplateCatalog templateCatalog) : Command<ListCommand.Settings>
{
    internal sealed class Settings : CommandSettings
    {
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var table = new Table().RoundedBorder();
        table.AddColumn("Id");
        table.AddColumn("Template");
        table.AddColumn("Origen");
        table.AddColumn("Tipo");
        table.AddColumn("Descripcion");

        foreach (var template in templateCatalog.GetAll())
        {
            table.AddRow(
                $"[cyan]{template.Id}[/]",
                template.Name,
                template.Source,
                template.Kind.ToString(),
                template.Description);
        }

        AnsiConsole.Write(table);
        return 0;
    }
}