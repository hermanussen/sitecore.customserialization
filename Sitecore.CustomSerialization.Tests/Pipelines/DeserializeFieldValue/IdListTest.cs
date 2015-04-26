namespace Sitecore.CustomSerialization.Tests.Pipelines.DeserializeFieldValue
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using Sitecore.CustomSerialization.Managers;
    using Sitecore.CustomSerialization.Pipelines;
    using Sitecore.FakeDb;
    using Sitecore.CustomSerialization.Pipelines.DeserializeFieldValue;

    public class IdListTest
    {
        [Test(Description = "Check if a list of ids is correctly deserialized")]
        public void ShouldDeserialize()
        {
            DbItem dbItem = new DbItem("it");
            using(var db = new Db()
                {
                    dbItem
                })
            {
                var args = new FieldSerializationPipelineArgs()
                    {
                        FieldSerializationType = FieldSerializationType.IdList,
                        Item = db.GetItem(dbItem.ID),
                        SerializationManager = new SerializationManager(),
                        ValueSerialized = @"
f11cd74b-099b-48c6-aaed-44927164e9e7
62e71efe-bcc8-48dc-9868-4dd2fdbfc2dc
",
                        FieldId = Guid.NewGuid()
                    };
                new IdList().Process(args);
                args.ValueNormal.ShouldBeEquivalentTo("{F11CD74B-099B-48C6-AAED-44927164E9E7}|{62E71EFE-BCC8-48DC-9868-4DD2FDBFC2DC}");
            }
        }
    }
}
