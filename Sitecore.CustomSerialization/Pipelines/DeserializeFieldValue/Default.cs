namespace Sitecore.CustomSerialization.Pipelines.DeserializeFieldValue
{
    using Sitecore.Diagnostics;

    public class Default : FieldSerializationPipelineProcessor
    {
        protected override void DoProcess(FieldSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.ValueNormal != null || FieldSerializationType.Default != args.FieldSerializationType)
            {
                return;
            }

            args.ValueNormal = args.ValueSerialized;
        }
    }
}
