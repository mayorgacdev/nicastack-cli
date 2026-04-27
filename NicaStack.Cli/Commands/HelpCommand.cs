using Spectre.Console;
using Spectre.Console.Cli;

namespace NicaStack.Cli.Commands;

internal sealed class HelpCommand : Command<HelpCommand.Settings>
{
    internal sealed class Settings : CommandSettings
    {
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.MarkupLine("[white]▲[/] [bold]NicaStack[/]");
        AnsiConsole.MarkupLine("[grey]Scaffold base architectures for .NET projects.[/]");
        AnsiConsole.MarkupLine("[grey]If you want a guided experience, run[/] [cyan]nicastack[/] [grey]or[/] [cyan]nicastack wizard[/][grey].[/]");
        AnsiConsole.Write(new Rule().RuleStyle("grey").LeftJustified());
        AnsiConsole.WriteLine();

        var commands = new Table().MinimalBorder().Expand();
        commands.AddColumn("Command");
        commands.AddColumn("Description");
        commands.AddRow("[cyan]help[/]", "Show this help.");
        commands.AddRow("[cyan]list[/]", "List available templates.");
        commands.AddRow("[cyan]create <name> --template <id>[/]", "Generate a project in non-interactive mode.");
        commands.AddRow("[cyan]wizard[/]", "Open the interactive step-by-step wizard.");
        commands.AddRow("[cyan]doctor[/]", "Check dependencies and basic permissions.");
        commands.AddRow("[cyan]version[/]", "Show the CLI and runtime version.");

        AnsiConsole.Write(commands);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Recommended paths[/]");
        AnsiConsole.MarkupLine("[green]New to the CLI:[/] use [cyan]nicastack[/] to enter guided mode.");
        AnsiConsole.MarkupLine("[green]Already know what you want:[/] use [cyan]nicastack create <name> --template <id>[/].");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Examples[/]");
        AnsiConsole.MarkupLine("[cyan]nicastack[/]");
        AnsiConsole.MarkupLine("[cyan]nicastack list[/]");
        AnsiConsole.MarkupLine("[cyan]nicastack create Billing --template ardalis-full[/]");
        AnsiConsole.MarkupLine("[cyan]nicastack create Billing --template vertical-slice-github --git-init[/]");
        AnsiConsole.MarkupLine("[cyan]nicastack doctor[/]");
        AnsiConsole.MarkupLine("[cyan]nicastack wizard[/]");

        return 0;
    }
}