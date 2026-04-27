using NicaStack.Cli.Domain;

namespace NicaStack.Cli.Services;

internal sealed class EnvironmentInspector(IProcessRunner processRunner) : IEnvironmentInspector
{
    public IReadOnlyList<DoctorCheckResult> RunChecks()
    {
        var checks = new List<DoctorCheckResult>
        {
            CheckCommand("dotnet SDK", "dotnet", ["--version"]),
            CheckCommand("git", "git", ["--version"]),
            CheckWritableDirectory(),
        };

        return checks;
    }

    private DoctorCheckResult CheckCommand(string name, string fileName, string[] arguments)
    {
        try
        {
            var result = processRunner.Run(fileName, arguments);
            return result.IsSuccess
                ? new DoctorCheckResult(name, true, result.StandardOutput)
                : new DoctorCheckResult(name, false, string.IsNullOrWhiteSpace(result.StandardError) ? "Comando ejecuto con error." : result.StandardError);
        }
        catch (Exception exception)
        {
            return new DoctorCheckResult(name, false, exception.Message);
        }
    }

    private static DoctorCheckResult CheckWritableDirectory()
    {
        try
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var testFile = Path.Combine(currentDirectory, $".nicastack-write-test-{Guid.NewGuid():N}.tmp");
            File.WriteAllText(testFile, "ok");
            File.Delete(testFile);

            return new DoctorCheckResult("workspace", true, $"Escritura habilitada en {currentDirectory}");
        }
        catch (Exception exception)
        {
            return new DoctorCheckResult("workspace", false, exception.Message);
        }
    }
}