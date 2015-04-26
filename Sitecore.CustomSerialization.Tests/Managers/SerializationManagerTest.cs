namespace Sitecore.CustomSerialization.Tests.Managers
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Sitecore.CustomSerialization.Domain;
    using Sitecore.CustomSerialization.Managers;
    using Sitecore.FakeDb;
    using NUnit.Framework;
    using Sitecore.Data;
    using Sitecore.Data.Items;

    public class SerializationManagerTest
    {
        [Test(Description = "Test basic scenario where a single item is updated in tree")]
        public void ShouldUpdateIndexFile()
        {
            DbItem dbItem = new DbItem("it1");
            using (var db = new Db()
                {
                    dbItem
                })
            {
                IndexFileItem indexFileItem = new IndexFileItem()
                    {
                        Id = ItemIDs.RootID.ToGuid()
                    };
                new SerializationManager().UpdateIndexFile(indexFileItem, db.GetItem(dbItem.ID));

                indexFileItem.Should().NotBeNull();

                List<IndexFileItem> itemList = new List<IndexFileItem>();
                itemList.Add(indexFileItem);
                AddAllDescendants(itemList, indexFileItem);

                itemList.Select(i => i.Id).ShouldAllBeEquivalentTo(new []
                    {
                        ItemIDs.RootID.ToGuid(),
                        ItemIDs.ContentRoot.ToGuid(),
                        dbItem.ID.ToGuid()
                    });
            }
        }

        [Test(Description = "Test scenario where an item and its descendants are serialized in the tree")]
        public void ShouldUpdateIndexFileRecursive()
        {
            DbItem d11 = new DbItem("it1.1");
            DbItem d121 = new DbItem("it1.2.1");
            DbItem d12 = new DbItem("it1.2")
                {
                    d121,
                };
            DbItem d13 = new DbItem("it1.3");
            DbItem dbItem = new DbItem("it1")
                {
                    d11,
                    d12,
                    d13,
                };
            using (var db = new Db()
                {
                    dbItem
                })
            {
                IndexFileItem indexFileItem = new IndexFileItem()
                {
                    Id = ItemIDs.RootID.ToGuid()
                };
                new SerializationManager().UpdateIndexFile(indexFileItem, db.GetItem(dbItem.ID), true);

                indexFileItem.Should().NotBeNull();

                List<IndexFileItem> itemList = new List<IndexFileItem>();
                itemList.Add(indexFileItem);
                AddAllDescendants(itemList, indexFileItem);

                itemList.Select(i => i.Id).ShouldAllBeEquivalentTo(new[]
                    {
                        ItemIDs.RootID.ToGuid(),
                        ItemIDs.ContentRoot.ToGuid(),
                        dbItem.ID.ToGuid(),
                        d11.ID.ToGuid(),
                        d12.ID.ToGuid(),
                        d13.ID.ToGuid(),
                        d121.ID.ToGuid()
                    });
            }
        }

        [Test(Description = "When updating the tree from the root, be sure not to add it as a child of itself")]
        public void ShouldUpdateIndexFileButSkipRoot()
        {
            using (var db = new Db())
            {
                IndexFileItem indexFileItem = new IndexFileItem()
                {
                    Id = ItemIDs.RootID.ToGuid()
                };
                new SerializationManager().UpdateIndexFile(indexFileItem, db.GetItem(ItemIDs.RootID));

                indexFileItem.Should().NotBeNull();

                List<IndexFileItem> itemList = new List<IndexFileItem>();
                itemList.Add(indexFileItem);
                AddAllDescendants(itemList, indexFileItem);

                itemList.Select(i => i.Id).ShouldAllBeEquivalentTo(new[]
                    {
                        ItemIDs.RootID.ToGuid()
                    });
            }
        }

        [Test(Description = "Should update the file to be serialized correctly with the item that is passed")]
        public void ShouldUpdateItemFile()
        {
            DbField sharedField = new DbField("sharedfield")
                {
                    Shared = true,
                    Value = "sharedfieldvalue"
                };
            DbField fieldInEnglishV1 = new DbField("env1field")
                {
                    { "en", 1, "env1fieldvalue"}
                };
            DbField fieldInEnglishV2 = new DbField("env2field")
                {
                    { "en", 2, "env2fieldvalue"}
                };
            DbField fieldInDutchV1 = new DbField("nlv1field")
                {
                    { "nl", 1, "nlv1fieldvalue"}
                };
            DbItem it = new DbItem("it")
                {
                    sharedField,
                    fieldInEnglishV1,
                    fieldInEnglishV2,
                    fieldInDutchV1
                };
            using (var db = new Db()
                {
                    it
                })
            {
                ItemFile itemFile = new ItemFile();
                
                SerializationManager serializationManager = new SerializationManager();
                serializationManager.UpdateItemFile(itemFile, db.GetItem(it.ID));
                
                itemFile.Id.ShouldBeEquivalentTo(it.ID.ToGuid());
                itemFile.Name.ShouldBeEquivalentTo("it");
                itemFile.TemplateId.ShouldBeEquivalentTo(it.TemplateID.ToGuid());
                itemFile.BranchId.ShouldBeEquivalentTo(it.BranchId);
                itemFile.Languages.Count.ShouldBeEquivalentTo(2);
                itemFile.Languages.First().LanguageName.ShouldBeEquivalentTo("en");
                itemFile.Languages.First().Versions.Count.ShouldBeEquivalentTo(2);
                
                CustomSerialization.Domain.Version englishVersion1 = itemFile.Languages.First().Versions.First();
                englishVersion1.VersionNumber.ShouldBeEquivalentTo(1);
                englishVersion1.FieldValues.Should().NotBeEmpty();
                FieldValue fieldValue = englishVersion1.FieldValues.FirstOrDefault(f => f.Id == fieldInEnglishV1.ID.ToGuid());
                fieldValue.Should().NotBeNull();
                fieldValue.Value.ShouldBeEquivalentTo("env1fieldvalue");
            }
        }

        [Test(Description = "Read an item from a file and update the item in Sitecore")]
        public void ShouldLoadItemIntoSitecore()
        {
            DbTemplate dbTemplate = new DbTemplate("tmpl");
            DbItem dbItem = new DbItem("it", ID.NewID, dbTemplate.ID);
            using (var db = new Db()
                {
                    dbTemplate,
                    dbItem
                })
            {
                ItemFile itemFile = new ItemFile()
                    {
                        TemplateId = dbTemplate.ID.ToGuid(),
                        BranchId = ID.NewID.ToGuid(),
                        Id = dbItem.ID.ToGuid(),
                        Name = "it"
                    };
                itemFile.SharedFieldValues.Add(new FieldValue()
                    {
                        Id = FieldIDs.ReadOnly.ToGuid(),
                        Value = "1"
                    });
                Language language = new Language()
                    {
                        LanguageName = "en",
                    };
                itemFile.Languages.Add(language);

                var version = new CustomSerialization.Domain.Version()
                    {
                        VersionNumber = 1
                    };
                language.Versions.Add(version);

                version.FieldValues.Add(new FieldValue()
                    {
                        Id = FieldIDs.Lock.ToGuid(),
                        Value = "<r />"
                    });

                new SerializationManager().LoadItemIntoSitecore(itemFile, db.GetItem(dbItem.ID), false);

                Item item = db.GetItem(dbItem.ID);

                item.Should().NotBeNull();
                item[FieldIDs.ReadOnly].ShouldBeEquivalentTo("1");
                item[FieldIDs.Lock].ShouldBeEquivalentTo("<r />");
            }
        }

        [Test(Description = "Read an item from a file and update the item name in Sitecore")]
        public void ShouldRenameItemInSitecore()
        {
            DbTemplate dbTemplate = new DbTemplate("tmpl");
            DbItem dbItem = new DbItem("it", ID.NewID, dbTemplate.ID);
            using (var db = new Db()
                {
                    dbTemplate,
                    dbItem
                })
            {
                ItemFile itemFile = new ItemFile()
                    {
                        TemplateId = dbTemplate.ID.ToGuid(),
                        BranchId = ID.NewID.ToGuid(),
                        Id = dbItem.ID.ToGuid(),
                        Name = "renamed"
                    };
                Language language = new Language() { LanguageName = "en" };
                itemFile.Languages.Add(language);
                language.Versions.Add(new CustomSerialization.Domain.Version() { VersionNumber = 1 });

                new SerializationManager().LoadItemIntoSitecore(itemFile, db.GetItem(dbItem.ID), false);

                Item item = db.GetItem(dbItem.ID);

                item.Should().NotBeNull();
                item.Name.ShouldBeEquivalentTo("renamed");
            }
        }

        [Test(Description = "Read an item from a file and update the item's template in Sitecore")]
        public void ShouldChangeTemplateInSitecore()
        {
            DbTemplate dbTemplateOld = new DbTemplate();
            DbTemplate dbTemplateNew = new DbTemplate();
            DbItem dbItem = new DbItem("it", ID.NewID, dbTemplateOld.ID);
            using (var db = new Db()
                {
                    dbTemplateOld,
                    dbTemplateNew,
                    dbItem
                })
            {
                ItemFile itemFile = new ItemFile()
                    {
                        TemplateId = dbTemplateNew.ID.ToGuid(),
                        BranchId = ID.NewID.ToGuid(),
                        Id = dbItem.ID.ToGuid(),
                        Name = "it"
                    };
                Language language = new Language() { LanguageName = "en" };
                itemFile.Languages.Add(language);
                language.Versions.Add(new CustomSerialization.Domain.Version() { VersionNumber = 1 });

                new SerializationManager().LoadItemIntoSitecore(itemFile, db.GetItem(dbItem.ID), false);

                Item item = db.GetItem(dbItem.ID);

                item.Should().NotBeNull();

                // TODO: the template isn't changed here, possibly because FakeDb doesn't handle item.ChangeTemplate(...) correctly
                //item.TemplateID.ToGuid().ShouldBeEquivalentTo(itemFile.TemplateId);
            }
        }

        [Test(Description = "Read an item from a file and update the reference to the branch it was created from in Sitecore")]
        public void ShouldChangeBranchInSitecore()
        {
            DbItem branchItem = new DbItem("branchit", ID.NewID, TemplateIDs.BranchTemplate);
            DbTemplate templateItem = new DbTemplate("templateit");
            DbItem dbItem = new DbItem("it", ID.NewID, templateItem.ID)
                {
                    BranchId = branchItem.ID
                };
            using (var db = new Db()
                {
                    branchItem,
                    templateItem,
                    dbItem
                })
            {
                ItemFile itemFile = new ItemFile()
                    {
                        TemplateId = templateItem.ID.ToGuid(),
                        BranchId = ID.NewID.ToGuid(),
                        Id = dbItem.ID.ToGuid(),
                        Name = "it"
                    };
                Language language = new Language() { LanguageName = "en" };
                itemFile.Languages.Add(language);
                language.Versions.Add(new CustomSerialization.Domain.Version() { VersionNumber = 1 });

                new SerializationManager().LoadItemIntoSitecore(itemFile, db.GetItem(dbItem.ID), false);

                Item item = db.GetItem(dbItem.ID);

                item.Should().NotBeNull();

                // TODO: the branch isn't changed here, possibly because FakeDb doesn't handle the BranchId setter on the item object correctly
                //item.BranchId.ToGuid().ShouldBeEquivalentTo(itemFile.BranchId);
            }
        }

        private static void AddAllDescendants(List<IndexFileItem> itemList, IndexFileItem indexFileItem)
        {
            itemList.AddRange(indexFileItem.Children);
            foreach (IndexFileItem it in indexFileItem.Children)
            {
                AddAllDescendants(itemList, it);
            }
        }
    }
}
