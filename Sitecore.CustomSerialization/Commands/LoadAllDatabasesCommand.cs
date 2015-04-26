namespace Sitecore.CustomSerialization.Commands
{
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Framework.Commands;

    public class LoadAllDatabasesCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
        }
    }
}
