namespace Sitecore.CustomSerialization.Pipelines.LoadItem
{
    using System.IO;
    using Sitecore.CustomSerialization.Domain;
    using Sitecore.Pipelines;
    using Sitecore.Diagnostics;

    public class LoadItemFromFile : CustomSerializationPipelineProcessor
    {
        protected readonly bool revert;

        public LoadItemFromFile(string revert)
        {
            bool.TryParse(revert, out this.revert);
        }

        protected override void DoProcess(CustomSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.Item, "no item was passed to the pipeline");

            FileInfo itemFileInfo = GetItemFileInfo(GetIndexFileInfo(args.Item.Database.Name).Directory, args.Item.ID.ToGuid());

            if (! itemFileInfo.Exists)
            {
                args.AddMessage(
                    string.Format("File '{0}' could not be found; item loading aborted", itemFileInfo.FullName),
                    PipelineMessageType.Error);
                args.AbortPipeline();
                return;
            }

            ItemFile itemFile = args.SerializationManager.ReadItemFile(itemFileInfo);

            if (this.revert)
            {
                args.Item = args.SerializationManager.DeleteAndRecreate(itemFile, args.Item);
            }

            args.SerializationManager.LoadItemIntoSitecore(itemFile, args.Item, revert);
        }
    }
}
