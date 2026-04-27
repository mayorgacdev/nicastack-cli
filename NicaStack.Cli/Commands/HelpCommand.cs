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
        AnsiConsole.Write(
            new FigletText("NicaStack")
                .LeftJustified()
                .Color(Color.DeepSkyBlue1));

        AnsiConsole.MarkupLine("[grey]Scaffold de arquitecturas base para proyectos .NET.[/]");
        AnsiConsole.MarkupLine("[grey]Si quieres una experiencia guiada, ejecuta[/] [cyan]nicastack[/] [grey]o[/] [cyan]nicastack wizard[/][grey].[/]");
        AnsiConsole.WriteLine();

        var commands = new Table().RoundedBorder();
        commands.AddColumn("Comando");
        commands.AddColumn("Descripcion");
        commands.AddRow("[cyan]help[/]", "Muestra esta ayuda.");
        commands.AddRow("[cyan]list[/]", "Lista los templates disponibles.");
        commands.AddRow("[cyan]create <name> --template <id>[/]", "Genera un proyecto en modo no interactivo.");
        commands.AddRow("[cyan]wizard[/]", "Abre el asistente interactivo paso a paso.");
        commands.AddRow("[cyan]doctor[/]", "Verifica dependencias y permisos basicos.");
        commands.AddRow("[cyan]version[/]", "Muestra la version del CLI y del runtime.");

        AnsiConsole.Write(commands);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Rutas recomendadas[/]");
        AnsiConsole.MarkupLine("[green]Nuevo en el CLI:[/] usa [cyan]nicastack[/] para entrar al modo guiado.");
        AnsiConsole.MarkupLine("[green]Ya sabes lo que quieres:[/] usa [cyan]nicastack create <name> --template <id>[/].");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold]Ejemplos[/]");
        AnsiConsole.MarkupLine("[cyan]nicastack[/]");
        AnsiConsole.MarkupLine("[cyan]nicastack list[/]");
        AnsiConsole.MarkupLine("[cyan]nicastack create Billing --template ardalis-full[/]");
        AnsiConsole.MarkupLine("[cyan]nicastack create Billing --template vertical-slice-github --git-init[/]");
        AnsiConsole.MarkupLine("[cyan]nicastack doctor[/]");
        AnsiConsole.MarkupLine("[cyan]nicastack wizard[/]");

        return 0;
    }
}