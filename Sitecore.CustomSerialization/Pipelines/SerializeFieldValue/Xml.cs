namespace Sitecore.CustomSerialization.Pipelines.SerializeFieldValue
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using Sitecore.Diagnostics;

    public class Xml : FieldSerializationPipelineProcessor
    {
        protected readonly List<string> supportedFieldTypeKeys = new List<string>()
            {
                "Layout"
            };

        protected override void DoProcess(FieldSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.ValueSerialized != null
                || string.IsNullOrWhiteSpace(args.ValueNormal)
                || ! supportedFieldTypeKeys.Any(k => k.Equals(args.FieldTypeKey, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            XmlDocument parsed = new XmlDocument();
            parsed.LoadXml(args.ValueNormal);

            using(MemoryStream memoryStream = new MemoryStream())
            using (XmlTextWriter writer = new XmlTextWriter(memoryStream, Encoding.Unicode)
                {
                    Formatting = Formatting.Indented
                })
            {
                parsed.WriteContentTo(writer);

                writer.Flush();
                memoryStream.Flush();
                memoryStream.Position = 0;

                StreamReader sr = new StreamReader(memoryStream);
                args.ValueSerialized = string.Concat(
                    Environment.NewLine,
                    sr.ReadToEnd(),
                    Environment.NewLine);
            }
            args.FieldSerializationType = FieldSerializationType.Xml;
        }
    }
}
