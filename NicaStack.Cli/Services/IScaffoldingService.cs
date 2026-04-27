using NicaStack.Cli.Domain;

namespace NicaStack.Cli.Services;

internal interface IScaffoldingService
{
    ScaffoldResult Scaffold(ScaffoldRequest request);
}