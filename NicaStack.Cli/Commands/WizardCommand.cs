using NicaStack.Cli.Domain;
using NicaStack.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NicaStack.Cli.Commands;

internal sealed class WizardCommand(ITemplateCatalog templateCatalog, IScaffoldingService scaffoldingService) : Command<WizardCommand.Settings>
{
    internal sealed class Settings : CommandSettings
    {
        [CommandOption("-o|--output <PATH>")]
        public string OutputPath { get; init; } = ".";
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(
            new FigletText("NicaStack")
                .LeftJustified()
                .Color(Color.DeepSkyBlue1));

        AnsiConsole.Write(
            new Panel(new Markup("[bold white]Architectural CLI[/] - wizard interactivo"))
                .Border(BoxBorder.Heavy)
                .BorderColor(Color.Yellow)
                .Padding(2, 0, 2, 0));

        AnsiConsole.WriteLine();

        var templates = templateCatalog.GetAll();
        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<TemplateDefinition>()
                .Title("[bold green]Select the template you want to generate[/]")
                .UseConverter(template => $"{template.Name} [grey]({template.Id})[/]")
                .AddChoices(templates));

        var projectName = AnsiConsole.Ask<string>("[bold white]Project/module name:[/]");
        var gitInit = AnsiConsole.Confirm("Initialize git when finished?", defaultValue: true);

        var request = new ScaffoldRequest(
            projectName,
            selection.Id,
            settings.OutputPath,
            Force: false,
            DryRun: false,
            SkipInstall: false,
            GitInit: gitInit,
            Verbose: false);

        var result = scaffoldingService.Scaffold(request);
        if (!result.Success)
        {
            AnsiConsole.MarkupLine($"[red]Error:[/] {result.ErrorMessage}");
            return 1;
        }

        AnsiConsole.MarkupLine($"[green]OK:[/] {result.Summary}");
        AnsiConsole.MarkupLine($"[yellow]Next step:[/] cd {result.ProjectDirectory}");
        return 0;
    }
}