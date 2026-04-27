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
                .WithDescription("Lista los templates disponibles.");

            config.AddCommand<HelpCommand>("help")
                .WithDescription("Muestra la ayuda general del CLI.");

            config.AddCommand<CreateCommand>("create")
                .WithDescription("Genera un proyecto a partir de un template.")
                .WithAlias("new");

            config.AddCommand<WizardCommand>("wizard")
                .WithDescription("Abre el asistente interactivo.");

            config.AddCommand<DoctorCommand>("doctor")
                .WithDescription("Verifica si tu entorno esta listo para generar proyectos.");

            config.AddCommand<VersionCommand>("version")
                .WithDescription("Muestra la version del CLI y del runtime.");

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