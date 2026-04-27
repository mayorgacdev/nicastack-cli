using NicaStack.Cli.Domain;

namespace NicaStack.Cli.Services;

internal interface IEnvironmentInspector
{
    IReadOnlyList<DoctorCheckResult> RunChecks();
}