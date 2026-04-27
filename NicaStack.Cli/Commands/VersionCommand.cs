using System.Reflection;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NicaStack.Cli.Commands;

internal sealed class VersionCommand : Command<VersionCommand.Settings>
{
    internal sealed class Settings : CommandSettings
    {
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "dev";

        var table = new Table().RoundedBorder();
        table.AddColumn("Campo");
        table.AddColumn("Valor");
        table.AddRow("CLI", version);
        table.AddRow("Runtime", Environment.Version.ToString());
        table.AddRow("OS", System.Runtime.InteropServices.RuntimeInformation.OSDescription);

        AnsiConsole.Write(table);
        return 0;
    }
}