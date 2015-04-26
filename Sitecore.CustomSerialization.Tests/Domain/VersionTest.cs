namespace Sitecore.CustomSerialization.Tests.Domain
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;
    using Sitecore.CustomSerialization.Domain;

    public class VersionTest
    {
        [Test(Description = "To ensure consistent serialization, field values should always be ordered alphabetically according to their id")]
        public void ShouldFieldValuesBeSorted()
        {
            var version = new CustomSerialization.Domain.Version();
            Guid guid1 = new Guid("f11cd74b-099b-48c6-aaed-44927164e9e7");
            version.FieldValues.Add(new FieldValue() { Id = guid1 });
            Guid guid2 = new Guid("62e71efe-bcc8-48dc-9868-4dd2fdbfc2dc");
            version.FieldValues.Add(new FieldValue() { Id = guid2 });
            Guid guid3 = new Guid("6aa71741-aaf7-4fef-97ad-6148e3a5c076");
            version.FieldValues.Add(new FieldValue() { Id = guid3 });

            version.FieldValues.Select(c => c.Id).ShouldBeEquivalentTo(new[]
                {
                    guid2, guid3, guid1
                }, options => options.WithStrictOrdering());
        }
    }
}
