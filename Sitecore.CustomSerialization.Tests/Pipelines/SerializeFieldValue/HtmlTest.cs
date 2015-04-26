namespace Sitecore.CustomSerialization.Tests.Pipelines.SerializeFieldValue
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using Sitecore.CustomSerialization.Managers;
    using Sitecore.CustomSerialization.Pipelines;
    using Sitecore.CustomSerialization.Pipelines.SerializeFieldValue;
    using Sitecore.FakeDb;

    public class HtmlTest
    {
        [Test(Description = "Check if html is pretty printed for serialization")]
        public void ShouldSerialize()
        {
            DbItem dbItem = new DbItem("it");
            using (var db = new Db()
                {
                    dbItem
                })
            {
                var args = new FieldSerializationPipelineArgs()
                {
                    Item = db.GetItem(dbItem.ID),
                    SerializationManager = new SerializationManager(),
                    ValueNormal = @"<p style=""line-height: 22px;"">From a single connected platform that also integrates with other customer-facing platforms, to a single view of the customer in a big data marketing repository, to a completely eliminating much of the complexity that has previously held marketers back, the latest version of Sitecore makes customer experience highly achievable. Learn how the latest version of Sitecore gives marketers the complete data, integrated tools, and automation capabilities to engage customers throughout an iterative lifecycle &ndash; the technology foundation absolutely necessary to win customers for life.</p>
<p>For further information, please go to the <a href=""https://doc.sitecore.net/"" target=""_blank"" title=""Sitecore Documentation site"">Sitecore Documentation site</a></p>",
                    FieldId = Guid.NewGuid(),
                    FieldTypeKey = "Rich Text"
                };
                new Html().Process(args);
                args.ValueSerialized.ShouldBeEquivalentTo(@"
<p style=""line-height: 22px;"">
      From a single connected platform that also integrates with
      other customer-facing platforms, to a single view of the
      customer in a big data marketing repository, to a completely
      eliminating much of the complexity that has previously held
      marketers back, the latest version of Sitecore makes customer
      experience highly achievable. Learn how the latest version of
      Sitecore gives marketers the complete data, integrated tools,
      and automation capabilities to engage customers throughout an
      iterative lifecycle &ndash; the technology foundation
      absolutely necessary to win customers for life.
    </p>
    <p>
      For further information, please go to the <a href=
      ""https://doc.sitecore.net/"" target=""_blank"" title=
      ""Sitecore Documentation site"">Sitecore Documentation site</a>
    </p>
");
                args.FieldSerializationType.ShouldBeEquivalentTo(FieldSerializationType.Html);
            }
        }
    }
}
