using NicaStack.Cli.Domain;

namespace NicaStack.Cli.Services;

internal sealed class ScaffoldingService(
    ITemplateCatalog templateCatalog,
    IProcessRunner processRunner,
    ITemplateRewriter templateRewriter) : IScaffoldingService
{
    public ScaffoldResult Scaffold(ScaffoldRequest request, Action<string>? reportProgress = null)
    {
        var template = templateCatalog.FindById(request.TemplateId);
        if (template is null)
        {
            return new ScaffoldResult(false, string.Empty, [], "Project generation failed.", $"Template '{request.TemplateId}' was not found.");
        }

        if (!IsValidProjectName(request.ProjectName))
        {
            return new ScaffoldResult(false, string.Empty, [], "Project generation failed.", "Project name can only contain letters, numbers, dots, and hyphens.");
        }

        var outputRoot = Path.GetFullPath(request.OutputPath);
        var projectDirectory = Path.Combine(outputRoot, request.ProjectName);
        if (Directory.Exists(projectDirectory))
        {
            if (!request.Force)
            {
                return new ScaffoldResult(false, projectDirectory, [], "Project generation failed.", $"Target directory already exists: {projectDirectory}");
            }

            Directory.Delete(projectDirectory, recursive: true);
        }

        var steps = BuildPlan(template, request, projectDirectory);
        if (request.DryRun)
        {
            return new ScaffoldResult(true, projectDirectory, steps, "Dry run completed. No changes were made.");
        }

        Directory.CreateDirectory(outputRoot);
        var executions = new List<ExecutionResult>();

        try
        {
            switch (template.Kind)
            {
                case TemplateKind.DotnetTemplate:
                    return ExecuteDotnetTemplate(template, request, projectDirectory, steps, executions, reportProgress);
                case TemplateKind.GitClone:
                    return ExecuteGitClone(template, request, projectDirectory, steps, executions, reportProgress);
                default:
                    return new ScaffoldResult(false, projectDirectory, steps, "Project generation failed.", "Unsupported template type.");
            }
        }
        catch (Exception exception)
        {
            return new ScaffoldResult(false, projectDirectory, steps, "Project generation failed.", exception.Message, executions);
        }
    }

    private ScaffoldResult ExecuteDotnetTemplate(
        TemplateDefinition template,
        ScaffoldRequest request,
        string projectDirectory,
        IReadOnlyList<string> steps,
        List<ExecutionResult> executions,
        Action<string>? reportProgress)
    {
        if (!request.SkipInstall && !string.IsNullOrWhiteSpace(template.PackageName))
        {
            reportProgress?.Invoke($"Installing NuGet template {template.PackageName}");
            var installResult = processRunner.Run("dotnet", ["new", "install", template.PackageName]);
            executions.Add(installResult);
            if (!installResult.IsSuccess)
            {
                return Failure(projectDirectory, steps, executions, "Could not install or update the NuGet template.");
            }
        }

        reportProgress?.Invoke($"Generating solution with dotnet new {template.ShortName}");
        var createResult = processRunner.Run(
            "dotnet",
            ["new", template.ShortName!, "-n", request.ProjectName, "-o", projectDirectory]);
        executions.Add(createResult);

        if (!createResult.IsSuccess)
        {
            return Failure(projectDirectory, steps, executions, "dotnet new returned an error.");
        }

        if (request.GitInit)
        {
            reportProgress?.Invoke("Initializing git repository");
            var gitInitResult = processRunner.Run("git", ["init"], projectDirectory);
            executions.Add(gitInitResult);
            if (!gitInitResult.IsSuccess)
            {
                return Failure(projectDirectory, steps, executions, "Could not initialize git in the generated project.");
            }
        }

        return Success(projectDirectory, steps, executions);
    }

    private ScaffoldResult ExecuteGitClone(
        TemplateDefinition template,
        ScaffoldRequest request,
        string projectDirectory,
        IReadOnlyList<string> steps,
        List<ExecutionResult> executions,
        Action<string>? reportProgress)
    {
        reportProgress?.Invoke("Cloning base repository from GitHub");
        var cloneResult = processRunner.Run("git", ["clone", template.RepositoryUrl!, projectDirectory]);
        executions.Add(cloneResult);

        if (!cloneResult.IsSuccess)
        {
            return Failure(projectDirectory, steps, executions, "Could not clone the base repository.");
        }

        var gitDirectory = Path.Combine(projectDirectory, ".git");
        if (Directory.Exists(gitDirectory))
        {
            Directory.Delete(gitDirectory, recursive: true);
        }

        reportProgress?.Invoke("Rewriting namespaces and internal names");
        templateRewriter.Rewrite(projectDirectory, template.NamespaceSeed!, request.ProjectName);

        if (request.GitInit)
        {
            reportProgress?.Invoke("Initializing git repository");
            var gitInitResult = processRunner.Run("git", ["init"], projectDirectory);
            executions.Add(gitInitResult);
            if (!gitInitResult.IsSuccess)
            {
                return Failure(projectDirectory, steps, executions, "Could not reinitialize git in the cloned project.");
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
            $"Selected template: {template.Name}",
            $"Target directory: {projectDirectory}",
        };

        if (template.Kind == TemplateKind.DotnetTemplate && !request.SkipInstall && !string.IsNullOrWhiteSpace(template.PackageName))
        {
            steps.Add($"Install or update NuGet template: {template.PackageName}");
        }

        if (template.Kind == TemplateKind.DotnetTemplate)
        {
            steps.Add($"Run dotnet new {template.ShortName}");
        }

        if (template.Kind == TemplateKind.GitClone)
        {
            steps.Add($"Clone base repository: {template.RepositoryUrl}");
            steps.Add("Rewrite namespaces and file names");
        }

        if (request.GitInit)
        {
            steps.Add("Initialize local git repository");
        }

        return steps;
    }

    private static ScaffoldResult Success(string projectDirectory, IReadOnlyList<string> steps, IReadOnlyList<ExecutionResult> executions)
    {
        return new ScaffoldResult(
            true,
            projectDirectory,
            steps,
            $"Project generated successfully at {projectDirectory}",
            Executions: executions);
    }

    private static ScaffoldResult Failure(string projectDirectory, IReadOnlyList<string> steps, IReadOnlyList<ExecutionResult> executions, string message)
    {
        var lastFailure = executions.LastOrDefault(result => !result.IsSuccess);
        var details = lastFailure is null
            ? message
            : $"{message}{Environment.NewLine}{lastFailure.CommandText}{Environment.NewLine}{(string.IsNullOrWhiteSpace(lastFailure.StandardError) ? lastFailure.StandardOutput : lastFailure.StandardError)}";

        return new ScaffoldResult(false, projectDirectory, steps, "Project generation failed.", details, executions);
    }
}