using Microsoft.Extensions.DependencyInjection;
using NicaStack.Cli.Commands;
using NicaStack.Cli.Services;
using Spectre.Console.Cli;

namespace NicaStack.Cli.Bootstrap;

internal static class CommandAppFactory
{
    public static CommandApp Create()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IProcessRunner, ProcessRunner>();
        services.AddSingleton<ITemplateCatalog, TemplateCatalog>();
        services.AddSingleton<ITemplateRewriter, TemplateRewriter>();
        services.AddSingleton<IEnvironmentInspector, EnvironmentInspector>();
        services.AddSingleton<IScaffoldingService, ScaffoldingService>();

        var registrar = new TypeRegistrar(services);
        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            config.SetApplicationName("nicastack");
            config.ValidateExamples();

            config.AddCommand<ListCommand>("list")
                .WithDescription("List available templates.");

            config.AddCommand<HelpCommand>("help")
                .WithDescription("Show general CLI help.");

            config.AddCommand<CreateCommand>("create")
                .WithDescription("Generate a project from a template.")
                .WithAlias("new");

            config.AddCommand<WizardCommand>("wizard")
                .WithDescription("Open the interactive wizard.");

            config.AddCommand<DoctorCommand>("doctor")
                .WithDescription("Check whether your environment is ready.");

            config.AddCommand<VersionCommand>("version")
                .WithDescription("Show the CLI and runtime version.");

            config.SetExceptionHandler((exception, _) =>
            {
                Spectre.Console.AnsiConsole.MarkupLine($"[red]Error:[/] {exception.Message}");
            });

            config.AddExample("list");
            config.AddExample("help");
            config.AddExample("create", "Billing", "--template", "ardalis-full");
            config.AddExample("create", "Billing", "--template", "ardalis-minimal", "--output", "./sandbox", "--git-init");
            config.AddExample("wizard");
            config.AddExample("doctor");
            config.AddExample("version");
        });

        return app;
    }
}