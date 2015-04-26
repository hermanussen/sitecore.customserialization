namespace Sitecore.CustomSerialization.Pipelines.DumpItem
{
    using System.IO;
    using Newtonsoft.Json;
    using Sitecore.CustomSerialization.Domain;
    using Sitecore.CustomSerialization.Managers;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;

    public class UpdateItemFile : CustomSerializationPipelineProcessor
    {
        protected readonly bool recurseAllDescendants;

        public UpdateItemFile(string recurseAllDescendants)
        {
            bool.TryParse(recurseAllDescendants, out this.recurseAllDescendants);
        }

        protected override void DoProcess(CustomSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.Item, "no item was passed to the pipeline");

            DirectoryInfo indexFileDirectory = GetIndexFileInfo(args.Item.Database.Name).Directory;
            UpdateItem(args.SerializationManager, indexFileDirectory, args.Item, recurseAllDescendants);
        }

        private void UpdateItem(SerializationManager serializationManager, DirectoryInfo indexFileDirectory, Item item, bool recurseAllDescendants)
        {
            FileInfo itemFileInfo = GetItemFileInfo(indexFileDirectory, item.ID.ToGuid());

            // Read item file
            ItemFile itemFile = serializationManager.ReadItemFile(itemFileInfo);

            // Update item data
            serializationManager.UpdateItemFile(itemFile, item);

            // Write to file
            itemFileInfo.Directory.Create();

            string serialized = JsonConvert.SerializeObject(itemFile, Formatting.Indented);
            File.WriteAllText(itemFileInfo.FullName, serialized.Replace("\\r\\n", "\n"));

            if (!recurseAllDescendants || !item.HasChildren)
            {
                return;
            }

            // Update children
            foreach (Item child in item.GetChildren())
            {
                UpdateItem(serializationManager, indexFileDirectory, child, recurseAllDescendants);
            }
        }
    }
}
