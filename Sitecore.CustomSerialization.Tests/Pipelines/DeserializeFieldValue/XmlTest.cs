namespace Sitecore.CustomSerialization.Tests.Pipelines.DeserializeFieldValue
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using Sitecore.CustomSerialization.Managers;
    using Sitecore.CustomSerialization.Pipelines;
    using Sitecore.FakeDb;

    public class XmlTest
    {
        [Test(Description = "Check if a serialized, pretty printed xml is deserialized correctly")]
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
                    FieldSerializationType = FieldSerializationType.Xml,
                    Item = db.GetItem(dbItem.ID),
                    SerializationManager = new SerializationManager(),
                    ValueSerialized = @"
<r xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <d id=""{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}"" l=""{14030E9F-CE92-49C6-AD87-7D49B50E42EA}"">
    <r ds="""" id=""{885B8314-7D8C-4CBB-8000-01421EA8F406}"" par="""" ph=""main"" uid=""{43222D12-08C9-453B-AE96-D406EBB95126}"" />
  </d>
</r>
",
                    FieldId = Guid.NewGuid()
                };
                new Sitecore.CustomSerialization.Pipelines.DeserializeFieldValue.Xml().Process(args);
                args.ValueNormal.ShouldBeEquivalentTo(@"<r xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <d id=""{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}"" l=""{14030E9F-CE92-49C6-AD87-7D49B50E42EA}"">
    <r ds="""" id=""{885B8314-7D8C-4CBB-8000-01421EA8F406}"" par="""" ph=""main"" uid=""{43222D12-08C9-453B-AE96-D406EBB95126}"" />
  </d>
</r>");
            }
        }
    }
}