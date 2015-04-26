namespace Sitecore.CustomSerialization.Domain
{
    using System;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class Language : IComparable<Language>
    {
        public Language()
        {
            Versions = new SortedSet<Version>();
        }

        [JsonProperty(PropertyName = "lang")]
        public string LanguageName { get; set; }

        [JsonProperty(PropertyName = "versions")]
        public SortedSet<Version> Versions { get; private set; }

        public int CompareTo(Language other)
        {
            return string.Compare(LanguageName, other.LanguageName, StringComparison.Ordinal);
        }
    }
}
