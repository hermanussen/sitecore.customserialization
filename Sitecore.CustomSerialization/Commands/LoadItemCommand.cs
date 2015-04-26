namespace Sitecore.CustomSerialization.Commands
{
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.CustomSerialization.Managers;
    using Sitecore.CustomSerialization.Pipelines;
    using Sitecore.Pipelines;

    public class LoadItemCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            bool isRevert = "1".Equals(context.Parameters["revert"]);

            CorePipeline.Run(isRevert ? "serialization.revertitem" : "serialization.loaditem",
                new CustomSerializationPipelineArgs()
                    {
                        SerializationManager = new SerializationManager(),
                        Item = context.Items[0]
                    });
        }
    }
}
