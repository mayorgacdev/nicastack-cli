using NicaStack.Cli.Domain;
using NicaStack.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NicaStack.Cli.Commands;

internal sealed class CreateCommand(IScaffoldingService scaffoldingService) : Command<CreateCommand.Settings>
{
    internal sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<name>")]
        public string Name { get; init; } = string.Empty;

        [CommandOption("-t|--template <TEMPLATE>")]
        public string TemplateId { get; init; } = string.Empty;

        [CommandOption("-o|--output <PATH>")]
        public string OutputPath { get; init; } = ".";

        [CommandOption("--git-init")]
        public bool GitInit { get; init; }

        [CommandOption("--force")]
        public bool Force { get; init; }

        [CommandOption("--dry-run")]
        public bool DryRun { get; init; }

        [CommandOption("--skip-install")]
        public bool SkipInstall { get; init; }

        [CommandOption("--verbose")]
        public bool Verbose { get; init; }

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(TemplateId))
            {
                return ValidationResult.Error("Debes indicar --template. Usa 'nicastack list' para ver las opciones.");
            }

            return ValidationResult.Success();
        }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var request = new ScaffoldRequest(
            settings.Name,
            settings.TemplateId,
            settings.OutputPath,
            settings.Force,
            settings.DryRun,
            settings.SkipInstall,
            settings.GitInit,
            settings.Verbose);

        var result = scaffoldingService.Scaffold(request);

        var plan = new Table().Border(TableBorder.Rounded).HideHeaders();
        plan.AddColumn("Plan");

        foreach (var step in result.Steps)
        {
            plan.AddRow($"[grey]-[/] {step}");
        }

        AnsiConsole.Write(plan);

        if (!result.Success)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {result.ErrorMessage}");
            return 1;
        }

        AnsiConsole.MarkupLine($"[green]OK:[/] {result.Summary}");

        if (settings.Verbose && result.Executions is not null)
        {
            var executionTable = new Table().RoundedBorder();
            executionTable.AddColumn("Comando");
            executionTable.AddColumn("ExitCode");
            executionTable.AddColumn("Salida");

            foreach (var execution in result.Executions)
            {
                var output = string.IsNullOrWhiteSpace(execution.StandardError) ? execution.StandardOutput : execution.StandardError;
                executionTable.AddRow(execution.CommandText, execution.ExitCode.ToString(), string.IsNullOrWhiteSpace(output) ? "(sin salida)" : output);
            }

            AnsiConsole.Write(executionTable);
        }

        if (!settings.DryRun)
        {
            AnsiConsole.MarkupLine($"[yellow]Siguiente paso:[/] cd {result.ProjectDirectory}");
        }

        return 0;
    }
}