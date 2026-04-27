using NicaStack.Cli.Domain;

namespace NicaStack.Cli.Services;

internal interface IProcessRunner
{
    ExecutionResult Run(string fileName, IEnumerable<string> arguments, string? workingDirectory = null);
}