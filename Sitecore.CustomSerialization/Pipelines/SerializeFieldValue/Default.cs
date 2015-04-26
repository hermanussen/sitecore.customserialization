namespace Sitecore.CustomSerialization.Pipelines.SerializeFieldValue
{
    using Sitecore.Diagnostics;

    public class Default : FieldSerializationPipelineProcessor
    {
        protected override void DoProcess(FieldSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.ValueSerialized != null)
            {
                return;
            }

            args.ValueSerialized = args.ValueNormal;
        }
    }
}