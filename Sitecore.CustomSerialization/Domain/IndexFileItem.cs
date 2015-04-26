namespace Sitecore.CustomSerialization.Domain
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using System.Linq;

    public class IndexFileItem : IComparable<IndexFileItem>
    {
        public IndexFileItem()
        {
            Children = new SortedSet<IndexFileItem>();
        }

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "c")]
        public SortedSet<IndexFileItem> Children { get; private set; }

        /// <summary>
        /// Don't serialize the Children property if there are no items.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeChildren()
        {
            return Children.Any();
        }

        /// <summary>
        /// Order index file items based on guid value.
        /// With one exception; deserialization order should be templates, then branches and then the rest.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IndexFileItem other)
        {
            if (Id == other.Id)
            {
                return 0;
            }
            if (Id == ItemIDs.TemplateRoot.ToGuid())
            {
                return int.MinValue;
            }
            if (other.Id == ItemIDs.TemplateRoot.ToGuid())
            {
                return int.MaxValue;
            }
            if (Id == ItemIDs.BranchesRoot.ToGuid())
            {
                return int.MaxValue;
            }
            if (other.Id == ItemIDs.BranchesRoot.ToGuid())
            {
                return int.MinValue;
            }
            return Id.CompareTo(other.Id);
        }

        /// <summary>
        /// Finds the item that corresponds with an id in all the descendants (or returns itself if it matches).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IndexFileItem GetDescendantOrSelf(Guid id)
        {
            if (Id == id)
            {
                return this;
            }
            return Children
                .Select(indexFileItem => indexFileItem.GetDescendantOrSelf(id))
                .FirstOrDefault(result => result != null);
        }
    }
}
