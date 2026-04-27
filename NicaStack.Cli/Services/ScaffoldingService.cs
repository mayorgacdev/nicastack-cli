using NicaStack.Cli.Domain;

namespace NicaStack.Cli.Services;

internal sealed class ScaffoldingService(
    ITemplateCatalog templateCatalog,
    IProcessRunner processRunner,
    ITemplateRewriter templateRewriter) : IScaffoldingService
{
    public ScaffoldResult Scaffold(ScaffoldRequest request)
    {
        var template = templateCatalog.FindById(request.TemplateId);
        if (template is null)
        {
            return new ScaffoldResult(false, string.Empty, [], "No se pudo generar el proyecto.", $"Template '{request.TemplateId}' no encontrado.");
        }

        if (!IsValidProjectName(request.ProjectName))
        {
            return new ScaffoldResult(false, string.Empty, [], "No se pudo generar el proyecto.", "El nombre del proyecto solo puede contener letras, numeros, puntos y guiones.");
        }

        var outputRoot = Path.GetFullPath(request.OutputPath);
        var projectDirectory = Path.Combine(outputRoot, request.ProjectName);
        if (Directory.Exists(projectDirectory))
        {
            if (!request.Force)
            {
                return new ScaffoldResult(false, projectDirectory, [], "No se pudo generar el proyecto.", $"La carpeta destino ya existe: {projectDirectory}");
            }

            Directory.Delete(projectDirectory, recursive: true);
        }

        var steps = BuildPlan(template, request, projectDirectory);
        if (request.DryRun)
        {
            return new ScaffoldResult(true, projectDirectory, steps, "Dry run completado. No se realizaron cambios.");
        }

        Directory.CreateDirectory(outputRoot);
        var executions = new List<ExecutionResult>();

        try
        {
            switch (template.Kind)
            {
                case TemplateKind.DotnetTemplate:
                    return ExecuteDotnetTemplate(template, request, projectDirectory, steps, executions);
                case TemplateKind.GitClone:
                    return ExecuteGitClone(template, request, projectDirectory, steps, executions);
                default:
                    return new ScaffoldResult(false, projectDirectory, steps, "No se pudo generar el proyecto.", "Tipo de template no soportado.");
            }
        }
        catch (Exception exception)
        {
            return new ScaffoldResult(false, projectDirectory, steps, "No se pudo generar el proyecto.", exception.Message, executions);
        }
    }

    private ScaffoldResult ExecuteDotnetTemplate(
        TemplateDefinition template,
        ScaffoldRequest request,
        string projectDirectory,
        IReadOnlyList<string> steps,
        List<ExecutionResult> executions)
    {
        if (!request.SkipInstall && !string.IsNullOrWhiteSpace(template.PackageName))
        {
            var installResult = processRunner.Run("dotnet", ["new", "install", template.PackageName]);
            executions.Add(installResult);
            if (!installResult.IsSuccess)
            {
                return Failure(projectDirectory, steps, executions, "No se pudo instalar o actualizar el template de NuGet.");
            }
        }

        var createResult = processRunner.Run(
            "dotnet",
            ["new", template.ShortName!, "-n", request.ProjectName, "-o", projectDirectory]);
        executions.Add(createResult);

        if (!createResult.IsSuccess)
        {
            return Failure(projectDirectory, steps, executions, "dotnet new devolvio un error.");
        }

        if (request.GitInit)
        {
            var gitInitResult = processRunner.Run("git", ["init"], projectDirectory);
            executions.Add(gitInitResult);
            if (!gitInitResult.IsSuccess)
            {
                return Failure(projectDirectory, steps, executions, "No se pudo inicializar git en el proyecto generado.");
            }
        }

        return Success(projectDirectory, steps, executions);
    }

    private ScaffoldResult ExecuteGitClone(
        TemplateDefinition template,
        ScaffoldRequest request,
        string projectDirectory,
        IReadOnlyList<string> steps,
        List<ExecutionResult> executions)
    {
        var cloneResult = processRunner.Run("git", ["clone", template.RepositoryUrl!, projectDirectory]);
        executions.Add(cloneResult);

        if (!cloneResult.IsSuccess)
        {
            return Failure(projectDirectory, steps, executions, "No se pudo clonar el repositorio base.");
        }

        var gitDirectory = Path.Combine(projectDirectory, ".git");
        if (Directory.Exists(gitDirectory))
        {
            Directory.Delete(gitDirectory, recursive: true);
        }

        templateRewriter.Rewrite(projectDirectory, template.NamespaceSeed!, request.ProjectName);

        if (request.GitInit)
        {
            var gitInitResult = processRunner.Run("git", ["init"], projectDirectory);
            executions.Add(gitInitResult);
            if (!gitInitResult.IsSuccess)
            {
                return Failure(projectDirectory, steps, executions, "No se pudo reinicializar git en el proyecto clonado.");
            }
        }

        return Success(projectDirectory, steps, executions);
    }

    private static bool IsValidProjectName(string projectName)
    {
        return !string.IsNullOrWhiteSpace(projectName)
            && projectName.All(character => char.IsLetterOrDigit(character) || character is '.' or '-');
    }

    private static IReadOnlyList<string> BuildPlan(TemplateDefinition template, ScaffoldRequest request, string projectDirectory)
    {
        var steps = new List<string>
        {
            $"Template seleccionado: {template.Name}",
            $"Directorio destino: {projectDirectory}",
        };

        if (template.Kind == TemplateKind.DotnetTemplate && !request.SkipInstall && !string.IsNullOrWhiteSpace(template.PackageName))
        {
            steps.Add($"Instalar o actualizar template NuGet: {template.PackageName}");
        }

        if (template.Kind == TemplateKind.DotnetTemplate)
        {
            steps.Add($"Ejecutar dotnet new {template.ShortName}");
        }

        if (template.Kind == TemplateKind.GitClone)
        {
            steps.Add($"Clonar repositorio base: {template.RepositoryUrl}");
            steps.Add("Reescribir namespaces y nombres de archivos");
        }

        if (request.GitInit)
        {
            steps.Add("Inicializar repositorio git local");
        }

        return steps;
    }

    private static ScaffoldResult Success(string projectDirectory, IReadOnlyList<string> steps, IReadOnlyList<ExecutionResult> executions)
    {
        return new ScaffoldResult(
            true,
            projectDirectory,
            steps,
            $"Proyecto generado correctamente en {projectDirectory}",
            Executions: executions);
    }

    private static ScaffoldResult Failure(string projectDirectory, IReadOnlyList<string> steps, IReadOnlyList<ExecutionResult> executions, string message)
    {
        var lastFailure = executions.LastOrDefault(result => !result.IsSuccess);
        var details = lastFailure is null
            ? message
            : $"{message}{Environment.NewLine}{lastFailure.CommandText}{Environment.NewLine}{(string.IsNullOrWhiteSpace(lastFailure.StandardError) ? lastFailure.StandardOutput : lastFailure.StandardError)}";

        return new ScaffoldResult(false, projectDirectory, steps, "No se pudo generar el proyecto.", details, executions);
    }
}