namespace Sitecore.CustomSerialization.Pipelines.SerializeFieldValue
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sitecore.Diagnostics;
    using System.IO;
    using System.Text;
    using TidyNet;

    public class Html : FieldSerializationPipelineProcessor
    {
        protected readonly List<string> supportedFieldTypeKeys = new List<string>()
            {
                "Rich Text"
            };

        protected override void DoProcess(FieldSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.ValueSerialized != null
                || string.IsNullOrWhiteSpace(args.ValueNormal)
                || !supportedFieldTypeKeys.Any(k => k.Equals(args.FieldTypeKey, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            Tidy tidy = new Tidy();

            tidy.Options.DocType = DocType.Omit;
            tidy.Options.TidyMark = true;
            tidy.Options.IndentContent = true;

            TidyMessageCollection tmc = new TidyMessageCollection();
            
            using(MemoryStream input = new MemoryStream())
            using (MemoryStream output = new MemoryStream())
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(args.ValueNormal);
                input.Write(byteArray, 0, byteArray.Length);
                input.Position = 0;
                tidy.Parse(input, output, tmc);

                string html = Encoding.UTF8.GetString(output.ToArray());
                string bodyTag = "<body>";
                string bodyCloseTag = "</body>";
                if (html.IndexOf(bodyTag) > 0)
                {
                    html = html.Substring(html.IndexOf(bodyTag) + bodyTag.Length);
                }
                if (html.IndexOf(bodyCloseTag) > 0)
                {
                    html = html.Substring(0, html.IndexOf(bodyCloseTag));
                }
                html = html.Trim();

                args.ValueSerialized = string.Concat(
                    Environment.NewLine,
                    html,
                    Environment.NewLine);
                args.FieldSerializationType = FieldSerializationType.Html;
            }
        }
    }
}
