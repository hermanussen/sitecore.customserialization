namespace Sitecore.CustomSerialization.Pipelines
{
    using Sitecore.CustomSerialization.Domain;
    using Sitecore.CustomSerialization.Managers;
    using Sitecore.Data.Items;
    using Sitecore.Pipelines;

    public class CustomSerializationPipelineArgs : PipelineArgs
    {
        public Item Item { get; set; }
        public SerializationManager SerializationManager { get; set; }
        public IndexFileItem IndexFile { get; set; }
    }
}
