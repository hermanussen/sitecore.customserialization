namespace Sitecore.CustomSerialization.Pipelines.DeserializeFieldValue
{
    using System;
    using System.Text.RegularExpressions;
    using Sitecore.Diagnostics;
    using System.Text;
    using Sitecore.Data;

    public class IdList : FieldSerializationPipelineProcessor
    {
        private static readonly int guidLength = Guid.NewGuid().ToString().Length;

        protected override void DoProcess(FieldSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.ValueNormal != null || FieldSerializationType.IdList != args.FieldSerializationType)
            {
                return;
            }

            StringBuilder valueNormal = new StringBuilder();
            string processValue = Regex.Replace(args.ValueSerialized, @"\s+", string.Empty);
            while (! string.IsNullOrWhiteSpace(processValue))
            {
                if (valueNormal.Length > 0)
                {
                    valueNormal.Append('|');
                }
                valueNormal.Append(ID.Parse(Guid.Parse(processValue.Substring(0, guidLength))));
                processValue = processValue.Substring(guidLength);
            }

            args.ValueNormal = valueNormal.ToString();
        }
    }
}
