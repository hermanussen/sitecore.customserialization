namespace Sitecore.CustomSerialization.Pipelines
{
    using System;
    using Sitecore.Pipelines;
    using Sitecore.CustomSerialization.Managers;
    using Sitecore.Data.Items;

    public class FieldSerializationPipelineArgs : PipelineArgs
    {
        public Item Item { get; set; }
        public SerializationManager SerializationManager { get; set; }
        public Guid FieldId { get; set; }
        public string FieldTypeKey { get; set; }
        public string ValueNormal { get; set; }
        public string ValueSerialized { get; set; }
        public FieldSerializationType FieldSerializationType { get; set; }
    }
}
