namespace Sitecore.CustomSerialization.Pipelines
{
    using System.IO;
    using System.Linq;
    using Sitecore.Data.Events;
    using Sitecore.SecurityModel;

    public abstract class CustomSerializationPipelineProcessor
    {
        public void Process(CustomSerializationPipelineArgs args)
        {
            using (new SecurityDisabler())
            {
                DoProcess(args);
            }
        }

        protected abstract void DoProcess(CustomSerializationPipelineArgs args);

        protected FileInfo GetIndexFileInfo(string databaseName)
        {
            return new FileInfo(Path.Combine(
                Sitecore.Configuration.Settings.GetSetting("SerializationFolder"),
                databaseName,
                "index.json"));
        }

        public FileInfo GetItemFileInfo(DirectoryInfo parentDirectory, System.Guid id)
        {
            string[] folders = id.ToString().Substring(0, 5).ToLowerInvariant()
                .ToCharArray().Select(c => c.ToString()).ToArray();
            string foldersRelativePath = Path.Combine(folders);
            string itemFileName = string.Format("{0}.json", id.ToString().ToLowerInvariant());
            return new FileInfo(Path.Combine(parentDirectory.FullName, foldersRelativePath, itemFileName));
        }
    }
}
