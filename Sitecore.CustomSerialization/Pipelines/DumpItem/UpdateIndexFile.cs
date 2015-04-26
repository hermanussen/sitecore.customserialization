namespace Sitecore.CustomSerialization.Pipelines.DumpItem
{
    using Sitecore.Diagnostics;

    public class UpdateIndexFile : CustomSerializationPipelineProcessor
    {
        protected readonly bool recurseAllDescendants;

        public UpdateIndexFile(string recurseAllDescendants)
        {
            bool.TryParse(recurseAllDescendants, out this.recurseAllDescendants);
        }

        protected override void DoProcess(CustomSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.IndexFile, "no index file was passed");
            Assert.IsNotNull(args.Item, "no item was passed to the pipeline");

            args.SerializationManager.UpdateIndexFile(args.IndexFile, args.Item, this.recurseAllDescendants);
        }
    }
}
