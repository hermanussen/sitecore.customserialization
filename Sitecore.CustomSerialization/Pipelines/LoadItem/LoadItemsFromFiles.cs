namespace Sitecore.CustomSerialization.Pipelines.LoadItem
{
    using Sitecore.Diagnostics;
    using Sitecore.CustomSerialization.Domain;
    using Sitecore.Pipelines;
    using Sitecore.CustomSerialization.Managers;
    using System.IO;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.SecurityModel;

    public class LoadItemsFromFiles : CustomSerializationPipelineProcessor
    {
        protected readonly bool revert;

        public LoadItemsFromFiles(string revert)
        {
            bool.TryParse(revert, out this.revert);
        }

        protected override void DoProcess(CustomSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.IndexFile, "No IndexFile found; needed for determining item tree structure");

            IndexFileItem indexFileItem = args.IndexFile.GetDescendantOrSelf(args.Item.ID.ToGuid());
            Assert.IsNotNull(indexFileItem, string.Format("Could not find item with ID {0} in serialized data", args.Item.ID));

            DirectoryInfo parentDirectory = GetIndexFileInfo(args.Item.Database.Name).Directory;

            SerializationManager serializationManager = new SerializationManager();
            CorePipeline.Run(revert ? "serialization.revertitem" : "serialization.loaditem",
                new CustomSerializationPipelineArgs()
                {
                    SerializationManager = serializationManager,
                    Item = args.Item
                });
            LoadDescendants(indexFileItem, args.Item, serializationManager, parentDirectory);
        }

        private void LoadDescendants(
            IndexFileItem parentFileItem,
            Item parent,
            SerializationManager serializationManager,
            DirectoryInfo parentDirectory)
        {
            foreach (var child in parentFileItem.Children)
            {
                Item item = parent.Database.GetItem(ID.Parse(child.Id));
                if (item == null)
                {
                    ItemFile itemFile = serializationManager
                        .ReadItemFile(GetItemFileInfo(parentDirectory, child.Id));
                    item = ItemManager.CreateItem(
                        itemFile.Name,
                        parent,
                        ID.Parse(itemFile.TemplateId),
                        ID.Parse(itemFile.Id),
                        SecurityCheck.Disable);
                }

                CorePipeline.Run(revert ? "serialization.revertitem" : "serialization.loaditem",
                    new CustomSerializationPipelineArgs()
                    {
                        SerializationManager = serializationManager,
                        Item = item
                    });

                LoadDescendants(child, item, serializationManager, parentDirectory);
            }
        }
    }
}
