namespace NicaStack.Cli.Domain;

internal sealed record DoctorCheckResult(string Name, bool Success, string Message);