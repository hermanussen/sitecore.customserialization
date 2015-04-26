namespace Sitecore.CustomSerialization.Tests.Pipelines.DeserializeFieldValue
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using Sitecore.CustomSerialization.Managers;
    using Sitecore.CustomSerialization.Pipelines;
    using Sitecore.FakeDb;

    public class HtmlTest
    {
        [Test(Description = "Check if serialized, pretty printed html is deserialized correctly")]
        public void ShouldDeserialize()
        {
            DbItem dbItem = new DbItem("it");
            using (var db = new Db()
                {
                    dbItem
                })
            {
                var args = new FieldSerializationPipelineArgs()
                {
                    FieldSerializationType = FieldSerializationType.Html,
                    Item = db.GetItem(dbItem.ID),
                    SerializationManager = new SerializationManager(),
                    ValueSerialized = @"
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
",
                    FieldId = Guid.NewGuid()
                };
                new Sitecore.CustomSerialization.Pipelines.DeserializeFieldValue.Html().Process(args);
                args.ValueNormal.ShouldBeEquivalentTo(@"<p style=""line-height: 22px;"">
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
    </p>");
            }
        }
    }
}