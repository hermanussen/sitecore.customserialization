namespace Sitecore.CustomSerialization.Commands
{
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.CustomSerialization.Pipelines;
    using Sitecore.Pipelines;
    using Sitecore.CustomSerialization.Managers;

    public class DumpTreeCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            CorePipeline.Run("serialization.dumptree", new CustomSerializationPipelineArgs()
                {
                    SerializationManager = new SerializationManager(),
                    Item = context.Items[0]
                });
        }
    }
}
