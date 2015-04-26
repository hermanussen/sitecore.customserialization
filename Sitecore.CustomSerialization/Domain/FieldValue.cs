namespace Sitecore.CustomSerialization.Domain
{
    using System;
    using Newtonsoft.Json;
    
    public class FieldValue : IComparable<FieldValue>
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "v")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "serializer", NullValueHandling = NullValueHandling.Ignore)]
        public string Serializer { get; set; }

        public int CompareTo(FieldValue other)
        {
            return Id.CompareTo(other.Id);
        }
    }
}
