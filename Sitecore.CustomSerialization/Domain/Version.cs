namespace Sitecore.CustomSerialization.Domain
{
    using System;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class Version : IComparable<Version>
    {
        public Version()
        {
            FieldValues = new SortedSet<FieldValue>();
        }

        [JsonProperty(PropertyName = "nr")]
        public int VersionNumber { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public SortedSet<FieldValue> FieldValues { get; private set; }

        public int CompareTo(Version other)
        {
            // ensure versions are ordered from lowest version number to highest
            return VersionNumber.CompareTo(other.VersionNumber);
        }
    }
}
