using NicaStack.Cli.Bootstrap;

if (args.Length == 0)
{
	return CommandAppFactory.Create().Run(["wizard"]);
}

return CommandAppFactory.Create().Run(args);