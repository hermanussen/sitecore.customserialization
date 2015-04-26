namespace Sitecore.CustomSerialization.Domain
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ItemFile : IComparable<ItemFile>
    {
        public ItemFile()
        {
            SharedFieldValues = new SortedSet<FieldValue>();
            Languages = new SortedSet<Language>();
        }

        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "templateid")]
        public Guid TemplateId { get; set; }

        [JsonProperty(PropertyName = "branchid", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? BranchId { get; set; }

        [JsonProperty(PropertyName = "shared")]
        public SortedSet<FieldValue> SharedFieldValues { get; private set; }

        [JsonProperty(PropertyName = "languages")]
        public SortedSet<Language> Languages { get; private set; }

        public int CompareTo(ItemFile other)
        {
            return Id.CompareTo(other.Id);
        }
    }
}
