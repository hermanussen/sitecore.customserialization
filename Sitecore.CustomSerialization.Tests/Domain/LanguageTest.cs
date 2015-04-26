namespace Sitecore.CustomSerialization.Tests.Domain
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using NUnit.Framework;
    using Sitecore.CustomSerialization.Domain;

    public class LanguageTest
    {
        [Test(Description = "To ensure consistent serialization, versions should always be ordered according to their number (ascending)")]
        public void ShouldVersionsBeSorted()
        {
            Language language = new Language();
            language.Versions.Add(new CustomSerialization.Domain.Version() { VersionNumber = 3 });
            language.Versions.Add(new CustomSerialization.Domain.Version() { VersionNumber = 1 });
            language.Versions.Add(new CustomSerialization.Domain.Version() { VersionNumber = 2 });

            language.Versions.Select(v => v.VersionNumber).ShouldBeEquivalentTo(new[]
                {
                    1, 2, 3
                }, options => options.WithStrictOrdering());
        }
    }
}
