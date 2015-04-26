namespace Sitecore.CustomSerialization.Pipelines.SerializeFieldValue
{
    using Sitecore.Diagnostics;
    using System;
    using System.Linq;
    using Sitecore.Data;

    public class IdList : FieldSerializationPipelineProcessor
    {
        protected override void DoProcess(FieldSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.ValueSerialized != null
                || string.IsNullOrWhiteSpace(args.ValueNormal)
                || ! MainUtil.Split(args.ValueNormal, '|').All(ID.IsID))
            {
                return;
            }
            
            ID[] parsed = ID.ParseArray(args.ValueNormal);
            args.ValueSerialized = string.Concat(
                Environment.NewLine,
                string.Join(Environment.NewLine, parsed.Select(id => id.ToGuid().ToString())),
                Environment.NewLine);
            args.FieldSerializationType = FieldSerializationType.IdList;
        }
    }
}
