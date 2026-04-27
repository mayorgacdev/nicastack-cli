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
        AnsiConsole.MarkupLine("[white]▲[/] [bold]NicaStack[/]");
        AnsiConsole.MarkupLine("[grey]Available templates[/]");
        AnsiConsole.Write(new Rule().RuleStyle("grey").LeftJustified());

        AnsiConsole.WriteLine();

        var table = new Table().MinimalBorder().Expand();
        table.AddColumn("[bold]Id[/]");
        table.AddColumn("[bold]Template[/]");
        table.AddColumn("[bold]Source[/]");
        table.AddColumn("[bold]Type[/]");
        table.AddColumn("[bold]Description[/]");

        foreach (var template in templateCatalog.GetAll())
        {
            table.AddRow(
                $"[cyan]{template.Id}[/]",
                $"[bold]{template.Name}[/]",
                $"[yellow]{template.Source}[/]",
                template.Kind.ToString(),
                template.Description);
        }

        AnsiConsole.Write(table);

        AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[grey]Guided[/]: [cyan]nicastack[/]");
            AnsiConsole.MarkupLine("[grey]Direct[/]: [cyan]nicastack create Billing --template ardalis-full[/]");
        return 0;
    }
}