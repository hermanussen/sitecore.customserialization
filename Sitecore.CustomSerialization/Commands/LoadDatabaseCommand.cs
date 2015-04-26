namespace Sitecore.CustomSerialization.Commands
{
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.CustomSerialization.Managers;
    using Sitecore.CustomSerialization.Pipelines;
    using Sitecore.Data.Items;
    using Sitecore.Pipelines;

    public class LoadDatabaseCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            bool isRevert = "1".Equals(context.Parameters["revert"]);

            Item rootItem = context.Items[0].Database.GetItem(ItemIDs.RootID);
            CorePipeline.Run(isRevert ? "serialization.reverttree" : "serialization.loadtree",
                new CustomSerializationPipelineArgs()
                {
                    SerializationManager = new SerializationManager(),
                    Item = rootItem
                });
        }
    }
}
