namespace NicaStack.Cli.Services;

internal interface ITemplateRewriter
{
    void Rewrite(string rootPath, string oldName, string newName);
}