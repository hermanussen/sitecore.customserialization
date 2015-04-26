namespace Sitecore.CustomSerialization.Pipelines.DumpItem
{
    using Sitecore.Diagnostics;
    using System.IO;
    using Newtonsoft.Json;

    public class WriteIndexFile : CustomSerializationPipelineProcessor
    {
        protected override void DoProcess(CustomSerializationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.IndexFile, "no index file was passed");
            Assert.IsNotNull(args.Item, "no item was passed to the pipeline");

            FileInfo indexFile = GetIndexFileInfo(args.Item.Database.Name);
            indexFile.Directory.Create();

            File.WriteAllText(indexFile.FullName, JsonConvert.SerializeObject(args.IndexFile, Formatting.Indented));
        }
    }
}
