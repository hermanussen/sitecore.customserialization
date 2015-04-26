namespace Sitecore.CustomSerialization.Pipelines.DumpItem
{
    using Sitecore.Diagnostics;
    using System.IO;
    using Newtonsoft.Json;
    using Sitecore.CustomSerialization.Domain;

    public class ReadIndexFile : CustomSerializationPipelineProcessor
    {
        protected override void DoProcess(CustomSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.Item, "no item was passed to the pipeline");

            FileInfo indexFile = GetIndexFileInfo(args.Item.Database.Name);
            if (! indexFile.Exists)
            {
                args.IndexFile = new IndexFileItem()
                    {
                        Id = ItemIDs.RootID.ToGuid()
                    };
            }
            else
            {
                using (StreamReader indexFileStream = File.OpenText(indexFile.FullName))
                {
                    args.IndexFile = new JsonSerializer().Deserialize<IndexFileItem>(new JsonTextReader(indexFileStream));
                }
            }
        }
    }
}
