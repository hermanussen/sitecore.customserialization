namespace Sitecore.CustomSerialization.Pipelines.DeserializeFieldValue
{
    using Sitecore.Diagnostics;

    public class Html : FieldSerializationPipelineProcessor
    {
        protected override void DoProcess(FieldSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.ValueNormal != null
                || FieldSerializationType.Html != args.FieldSerializationType
                || args.ValueSerialized == null)
            {
                return;
            }

            args.ValueNormal = args.ValueSerialized.Trim();
        }
    }
}
