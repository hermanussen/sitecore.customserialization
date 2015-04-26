namespace Sitecore.CustomSerialization.Tests.Domain
{
    using NUnit.Framework;
    using System;
    using FluentAssertions;
    using Sitecore.CustomSerialization.Domain;
    using System.Linq;

    public class IndexFileItemTest
    {
        [Test(Description = "To ensure consistent serialization, children in the index file should always be ordered alphabetically according to their id")]
        public void ShouldChildrenBeSorted()
        {
            IndexFileItem indexFileItem = new IndexFileItem();
            Guid guid1 = new Guid("f11cd74b-099b-48c6-aaed-44927164e9e7");
            indexFileItem.Children.Add(new IndexFileItem() { Id = guid1 });
            Guid guid2 = new Guid("62e71efe-bcc8-48dc-9868-4dd2fdbfc2dc");
            indexFileItem.Children.Add(new IndexFileItem() { Id = guid2 });
            Guid guid3 = new Guid("6aa71741-aaf7-4fef-97ad-6148e3a5c076");
            indexFileItem.Children.Add(new IndexFileItem() { Id = guid3 });

            indexFileItem.Children.Select(c => c.Id).ShouldBeEquivalentTo(new []
                {
                    guid2, guid3, guid1
                }, options => options.WithStrictOrdering());
        }

        [Test(Description = "To ensure that templates and then branches are deserialized first, we need to make an exception to the guid ordering")]
        public void ShouldSortTemplatesFirst()
        {
            IndexFileItem indexFileItem = new IndexFileItem()
                {
                    Id = ItemIDs.RootID.ToGuid()
                };

            // Set children for tree root
            indexFileItem.Children.Add(new IndexFileItem()
                {
                    Id = ItemIDs.ContentRoot.ToGuid() //0DE95AE4
                });
            indexFileItem.Children.Add(new IndexFileItem()
                {
                    Id = ItemIDs.MediaLibraryRoot.ToGuid() //3D6658D8
                });
            indexFileItem.Children.Add(new IndexFileItem()
                {
                    Id = ItemIDs.LayoutRoot.ToGuid() //EB2E4FFD
                });
            indexFileItem.Children.Add(new IndexFileItem()
                {
                    Id = ItemIDs.SystemRoot.ToGuid() //13D6D6C6
                });
            IndexFileItem templatesIndexFileItem = new IndexFileItem()
                {
                    Id = ItemIDs.TemplateRoot.ToGuid() //3C1715FE
                };
            indexFileItem.Children.Add(templatesIndexFileItem);

            // Set children below templates root
            Guid guid1 = new Guid("f11cd74b-099b-48c6-aaed-44927164e9e7");
            templatesIndexFileItem.Children.Add(new IndexFileItem()
                {
                    Id = guid1
                });
            templatesIndexFileItem.Children.Add(new IndexFileItem()
                {
                    Id = ItemIDs.BranchesRoot.ToGuid()
                });
            Guid guid2 = new Guid("62e71efe-bcc8-48dc-9868-4dd2fdbfc2dc");
            templatesIndexFileItem.Children.Add(new IndexFileItem()
                {
                    Id = guid2
                });

            // Check if template root is first
            indexFileItem.Children.Select(c => c.Id).ShouldBeEquivalentTo(new[]
                {
                    ItemIDs.TemplateRoot.ToGuid(),
                    ItemIDs.ContentRoot.ToGuid(),
                    ItemIDs.SystemRoot.ToGuid(),
                    ItemIDs.MediaLibraryRoot.ToGuid(),
                    ItemIDs.LayoutRoot.ToGuid()
                }, options => options.WithStrictOrdering());

            // Check if branches root is last
            templatesIndexFileItem.Children.Select(c => c.Id).ShouldBeEquivalentTo(new[]
                {
                    guid2, guid1, ItemIDs.BranchesRoot.ToGuid()
                }, options => options.WithStrictOrdering());
        }

        [Test(Description = "Ensure that the GetDescendantOrSelf method returns itself when the id matches")]
        public void ShouldGetSelf()
        {
            IndexFileItem indexFileItem = new IndexFileItem()
                {
                    Id = Guid.NewGuid()
                };
            indexFileItem.GetDescendantOrSelf(indexFileItem.Id).Should().BeSameAs(indexFileItem);
        }

        [Test(Description = "Ensure that the GetDescendantOrSelf method returns the grandchild when the id matches")]
        public void ShouldGetDescendant()
        {
            IndexFileItem indexFileItem = new IndexFileItem()
                {
                    Id = Guid.NewGuid()
                };
            IndexFileItem child = new IndexFileItem()
                {
                    Id = Guid.NewGuid()
                };
            indexFileItem.Children.Add(child);
            IndexFileItem grandChild = new IndexFileItem()
                {
                    Id = Guid.NewGuid()
                };
            child.Children.Add(grandChild);

            indexFileItem.GetDescendantOrSelf(grandChild.Id).Should().BeSameAs(grandChild);
        }

        [Test(Description = "Ensure that the GetDescendantOrSelf method returns null when the id does not match anything")]
        public void ShouldGetNullIfUnavailable()
        {
            IndexFileItem indexFileItem = new IndexFileItem()
                {
                    Id = Guid.NewGuid()
                };
            indexFileItem.GetDescendantOrSelf(Guid.NewGuid()).Should().BeNull();
        }
    }
}
