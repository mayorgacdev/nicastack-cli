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
                return ValidationResult.Error("You must provide --template. Use 'nicastack list' to see the available options.");
            }

            return ValidationResult.Success();
        }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        WriteHeader();

        var request = new ScaffoldRequest(
            settings.Name,
            settings.TemplateId,
            settings.OutputPath,
            settings.Force,
            settings.DryRun,
            settings.SkipInstall,
            settings.GitInit,
            settings.Verbose);

        WriteTelemetry(settings);

        ScaffoldResult result;
        if (settings.DryRun)
        {
            result = scaffoldingService.Scaffold(request);
        }
        else
        {
            result = default!;
            AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .SpinnerStyle(Style.Parse("grey"))
                .Start("Preparing project...", statusContext =>
                {
                    result = scaffoldingService.Scaffold(request, message =>
                    {
                        statusContext.Status($"[white]{Markup.Escape(message)}[/]");
                    });
                });
        }

        AnsiConsole.Write(new Rule("[grey]Steps[/]").RuleStyle("grey").LeftJustified());

        foreach (var step in result.Steps)
        {
            AnsiConsole.MarkupLine($"[grey]-[/] {Markup.Escape(step)}");
        }

        AnsiConsole.WriteLine();

        if (!result.Success)
        {
            AnsiConsole.Write(new Rule("[red]Error[/]").RuleStyle("red").LeftJustified());
            AnsiConsole.MarkupLine($"[red]x[/] {Markup.Escape(result.ErrorMessage ?? "Unknown error.")}");
            return 1;
        }

        AnsiConsole.Write(new Rule("[green]Done[/]").RuleStyle("green").LeftJustified());
        AnsiConsole.MarkupLine($"[green]✓[/] {Markup.Escape(result.Summary)}");

        if (settings.Verbose && result.Executions is not null)
        {
            var executionTable = new Table().MinimalBorder().Expand();
            executionTable.AddColumn("Command");
            executionTable.AddColumn("Code");
            executionTable.AddColumn("Output");

            foreach (var execution in result.Executions)
            {
                var output = string.IsNullOrWhiteSpace(execution.StandardError) ? execution.StandardOutput : execution.StandardError;
                executionTable.AddRow(
                    execution.CommandText,
                    execution.ExitCode == 0 ? "[green]0[/]" : $"[red]{execution.ExitCode}[/]",
                    string.IsNullOrWhiteSpace(output) ? "(sin salida)" : Markup.Escape(output));
            }

            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Rule("[grey]Verbose[/]").RuleStyle("grey").LeftJustified());
            AnsiConsole.Write(executionTable);
        }

        if (!settings.DryRun)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[grey]Next:[/] [yellow]cd {Markup.Escape(result.ProjectDirectory)}[/]");
        }

        return 0;
    }

    private static void WriteHeader()
    {
        AnsiConsole.MarkupLine("[white]▲[/] [bold]NicaStack[/]");
        AnsiConsole.MarkupLine("[grey]Project scaffolding for .NET[/]");
        AnsiConsole.Write(new Rule().RuleStyle("grey").LeftJustified());
        AnsiConsole.WriteLine();
    }

    private static void WriteTelemetry(Settings settings)
    {
        AnsiConsole.MarkupLine($"[grey]Project[/]: [bold]{Markup.Escape(settings.Name)}[/]");
        AnsiConsole.MarkupLine($"[grey]Template[/]: [bold]{Markup.Escape(settings.TemplateId)}[/]");
        AnsiConsole.MarkupLine($"[grey]Output[/]: {Markup.Escape(Path.GetFullPath(settings.OutputPath))}");
        AnsiConsole.MarkupLine($"[grey]Mode[/]: {(settings.DryRun ? "[blue]dry-run[/]" : "[green]live[/]")}");
        if (settings.GitInit)
        {
            AnsiConsole.MarkupLine("[grey]Git[/]: [green]enabled[/]");
        }

        AnsiConsole.WriteLine();
    }
}