namespace Sitecore.CustomSerialization.Commands
{
    using Sitecore.Diagnostics;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Pipelines;
    using Sitecore.CustomSerialization.Managers;
    using Sitecore.CustomSerialization.Pipelines;

    public class DumpItemCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            CorePipeline.Run("serialization.dumpitem", new CustomSerializationPipelineArgs()
                {
                    SerializationManager = new SerializationManager(),
                    Item = context.Items[0]
                });
        }
    }
}
