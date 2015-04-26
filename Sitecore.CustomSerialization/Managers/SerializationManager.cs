namespace Sitecore.CustomSerialization.Managers
{
    using Sitecore.CustomSerialization.Domain;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using System.Collections.Generic;
    using System.IO;
    using Sitecore.Data.Fields;
    using System.Linq;
    using Newtonsoft.Json;
    using System;
    using Sitecore.Data;
    using Sitecore.Data.Managers;
    using Sitecore.Globalization;
    using Sitecore.SecurityModel;
    using Sitecore.CustomSerialization.Pipelines;
    using Sitecore.Pipelines;

    public class SerializationManager
    {
        public void UpdateIndexFile(IndexFileItem indexFileItem, Item item, bool recurseAllDescendants = false)
        {
            Assert.ArgumentNotNull(indexFileItem, "indexFileItem");
            Assert.ArgumentNotNull(item, "item");

            IndexFileItem postitionInTree = indexFileItem;

            // Iterate ancestors to ensure they are present in the tree
            var ancestorsAndSelf = item.Axes.GetAncestors().Skip(1);
            if (item.ID != ItemIDs.RootID)
            {
                ancestorsAndSelf = ancestorsAndSelf.Concat(new[] {item});
            }

            foreach (var it in ancestorsAndSelf)
            {
                IndexFileItem current = postitionInTree.Children.FirstOrDefault(i => i.Id == it.ID.ToGuid());
                if (current == null)
                {
                    current = new IndexFileItem()
                        {
                            Id = it.ID.ToGuid()
                        };
                    postitionInTree.Children.Add(current);
                }
                postitionInTree = current;
            }

            // If required, iterate all descendants and add them to the tree as well
            if (recurseAllDescendants)
            {
                AddDescendantsToTree(postitionInTree, item);
            }
        }

        public void UpdateItemFile(ItemFile itemFile, Item item)
        {
            itemFile.Id = item.ID.ToGuid();
            itemFile.Name = item.Name;
            itemFile.TemplateId = item.TemplateID.ToGuid();
            itemFile.BranchId = item.BranchId.IsNull ? null as System.Guid? : item.BranchId.ToGuid();

            foreach (Item version in item.Versions.GetVersions(true))
            {
                UpdateValuesInLanguageVersion(itemFile, version);
            }
        }

        private void UpdateValuesInLanguageVersion(ItemFile itemFile, Item item)
        {
            foreach (Field field in item.Fields)
            {
                if (field.Shared)
                {
                    SortedSet<FieldValue> fields = itemFile.SharedFieldValues;
                    UpdateField(item, fields, field);
                }
                else
                {
                    Domain.Language language = itemFile.Languages.FirstOrDefault(l => l.LanguageName == field.Language.Name);
                    if (language == null)
                    {
                        language = new Domain.Language()
                            {
                                LanguageName = field.Language.Name
                            };
                        itemFile.Languages.Add(language);
                    }
                    Domain.Version version = language.Versions.FirstOrDefault(v => v.VersionNumber == item.Version.Number);
                    if (version == null)
                    {
                        version = new Domain.Version()
                            {
                                VersionNumber = item.Version.Number
                            };
                        language.Versions.Add(version);
                    }
                    UpdateField(item, version.FieldValues, field);
                }
            }
        }

        private void UpdateField(Item item, SortedSet<FieldValue> fields, Field field)
        {
            FieldValue fieldValue = fields.FirstOrDefault(f => f.Id == field.ID.ToGuid());
            if (fieldValue == null)
            {
                fieldValue = new FieldValue()
                    {
                        Id = field.ID.ToGuid()
                    };
                fields.Add(fieldValue);
            }
            FieldSerializationType fieldValueType;
            fieldValue.Value = SerializeFieldValue(field.Value, item, field.ID.ToGuid(), out fieldValueType);
            if (fieldValueType != FieldSerializationType.Default)
            {
                fieldValue.Serializer = Enum.GetName(typeof (FieldSerializationType), fieldValueType);
            }
        }

        private void AddDescendantsToTree(IndexFileItem indexFileItem, Item item)
        {
            if (! item.HasChildren)
            {
                return;
            }
            foreach (Item child in item.GetChildren().InnerChildren)
            {
                IndexFileItem fileItem = indexFileItem.Children.FirstOrDefault(c => c.Id == child.ID.ToGuid());
                    
                if (fileItem == null)
                {
                    fileItem = new IndexFileItem()
                        {
                            Id = child.ID.ToGuid()
                        };
                    indexFileItem.Children.Add(fileItem);
                }
                    
                AddDescendantsToTree(fileItem, child);
            }
        }

        public ItemFile ReadItemFile(FileInfo itemFileInfo)
        {
            if (!itemFileInfo.Exists)
            {
                return new ItemFile();
            }
            else
            {
                using (StreamReader indexFileStream = File.OpenText(itemFileInfo.FullName))
                {
                    return new JsonSerializer().Deserialize<ItemFile>(new JsonTextReader(indexFileStream));
                }
            }
        }

        public void LoadItemIntoSitecore(ItemFile itemFile, Item item, bool revert)
        {
            Assert.ArgumentNotNull(itemFile, "itemFile");
            Assert.ArgumentNotNull(item, "item");
            Assert.AreEqual(itemFile.Id.ToString(), item.ID.ToGuid().ToString(),
                "you cannot load an item if id's are differents");

            bool sharedOperationsHandled = false;

            foreach (Domain.Language language in itemFile.Languages)
            {
                using (new LanguageSwitcher(language.LanguageName))
                {
                    foreach (Domain.Version version in language.Versions)
                    {
                        var itemInLanguage = item.Database.GetItem(
                            item.ID,
                            LanguageManager.GetLanguage(language.LanguageName),
                            new Data.Version(version.VersionNumber));

                        if (itemInLanguage == null)
                        {
                            itemInLanguage = item.Versions.AddVersion();
                        }

                        itemInLanguage.Editing.BeginEdit();
                        try
                        {
                            if (!sharedOperationsHandled)
                            {
                                // Ensure that the correct template is used
                                if (! itemInLanguage.TemplateID.ToGuid().Equals(itemFile.TemplateId))
                                {
                                    Item templateItem = itemInLanguage.Database.GetItem(new ID(itemFile.TemplateId));
                                    if (templateItem == null)
                                    {
                                        throw new CustomSerializationException(
                                            string.Format("Unable to deserialize item {0}, because template {1} is not available",
                                                            itemInLanguage.ID,
                                                            itemInLanguage.TemplateID));
                                    }
                                    itemInLanguage.ChangeTemplate(new TemplateItem(templateItem));
                                }

                                // Set the correct branch that the item originates from
                                itemInLanguage.BranchId = itemFile.BranchId.HasValue ? new ID(itemFile.BranchId.Value) : ID.Null;

                                // Set the correct name of the item (rename if needed)
                                if (itemInLanguage.Name != itemFile.Name)
                                {
                                    itemInLanguage.Name = itemFile.Name;
                                }

                                // Set the values of shared fields
                                foreach (FieldValue sharedFieldValue in itemFile.SharedFieldValues)
                                {
                                    FieldSerializationType serializationType = FieldSerializationType.Default;
                                    if (sharedFieldValue.Serializer != null)
                                    {
                                        Enum.TryParse(sharedFieldValue.Serializer, out serializationType);
                                    }
                                    itemInLanguage[ID.Parse(sharedFieldValue.Id)] = DeserializeFieldValue(sharedFieldValue.Value, itemInLanguage, sharedFieldValue.Id, serializationType);    
                                }

                                sharedOperationsHandled = true;
                            }

                            foreach (FieldValue fieldValue in version.FieldValues)
                            {
                                FieldSerializationType serializationType = FieldSerializationType.Default;
                                if (fieldValue.Serializer != null)
                                {
                                    Enum.TryParse(fieldValue.Serializer, out serializationType);
                                }
                                itemInLanguage[ID.Parse(fieldValue.Id)] = DeserializeFieldValue(fieldValue.Value, itemInLanguage, fieldValue.Id, serializationType);
                            }

                            itemInLanguage.Editing.AcceptChanges(false, false);
                            itemInLanguage.Editing.EndEdit(false, false);
                        }
                        catch (Exception)
                        {
                            itemInLanguage.Editing.RejectChanges();
                            itemInLanguage.Editing.EndEdit();
                            throw;
                        }
                    }
                }
            }
        }

        private string SerializeFieldValue(string value, Item item, Guid fieldId, out FieldSerializationType serializationType)
        {
            serializationType = FieldSerializationType.Default;

            var args = new FieldSerializationPipelineArgs()
                {
                    SerializationManager = this,
                    Item = item,
                    FieldId = fieldId,
                    ValueNormal = value,
                };

            Field field = item.Fields[ID.Parse(fieldId)];
            if (field != null)
            {
                args.FieldTypeKey = field.TypeKey;
            }

            const string pipelineName = "serialization.serializefieldvalue";
            if (CorePipelineFactory.GetPipeline(pipelineName, string.Empty) != null)
            {
                CorePipeline.Run(pipelineName, args);
                serializationType = args.FieldSerializationType;
            }
            return args.ValueSerialized ?? value;
        }

        private string DeserializeFieldValue(string value, Item item, Guid fieldId, FieldSerializationType serializationType)
        {
            var args = new FieldSerializationPipelineArgs()
                {
                    SerializationManager = this,
                    Item = item,
                    FieldId = fieldId,
                    ValueSerialized = value,
                    FieldSerializationType = serializationType
                };
            const string pipelineName = "serialization.deserializefieldvalue";
            if (CorePipelineFactory.GetPipeline(pipelineName, string.Empty) != null)
            {
                CorePipeline.Run(pipelineName, args);
            }
            return args.ValueNormal ?? value;
        }

        public Item DeleteAndRecreate(ItemFile itemFile, Item item)
        {
            // Delete and recreate the item
            var parent = item.Parent;
            item.Delete();
            return ItemManager.CreateItem(
                itemFile.Name,
                parent,
                ID.Parse(itemFile.TemplateId),
                ID.Parse(itemFile.Id),
                SecurityCheck.Disable);
        }
    }
}