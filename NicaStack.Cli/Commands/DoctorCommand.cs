using NicaStack.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NicaStack.Cli.Commands;

internal sealed class DoctorCommand(IEnvironmentInspector environmentInspector) : Command<DoctorCommand.Settings>
{
    internal sealed class Settings : CommandSettings
    {
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var checks = environmentInspector.RunChecks();
        var table = new Table().RoundedBorder();
        table.AddColumn("Check");
        table.AddColumn("Estado");
        table.AddColumn("Detalle");

        foreach (var check in checks)
        {
            var state = check.Success ? "[green]OK[/]" : "[red]FAIL[/]";
            table.AddRow(check.Name, state, check.Message);
        }

        AnsiConsole.Write(table);
        return checks.All(check => check.Success) ? 0 : 1;
    }
}