# Sitecore.CustomSerialization
Alternative serialization/deserialization format for Sitecore that aims to be more suitable for version control.

> Warning: this project is unfinished. Don't use this for actual projects (yet). Use at your own risk.

## How does it work?

After adding Sitecore.CustomSerialization to your project:
* The functions on the "Developer" ribbon tab related to serialization/deserialization are changed to work with a new serialization format. 
* Instead of the default "serialization" folder, Sitecore now uses "serialization_custom".
* More to come... (check the TODO section for more info)

## Advantages

The custom serialization format is designed to:
* **Be in a JSON format, so it can be easily parsed by other tools.**
* **Avoid duplicate/unnecessary information.** E.g.: the default serialization format lists the field name and length of the content, which is difficult to keep up to date and can't easily be merged in conflict situations.
* **Use item IDs as file names and basis for folder structure.** ID's don't change, whereas item names do. In the default serialization format, a small change like renaming a folder can cause an entire tree of items to be changed. This makes merging conflicting changes extremely difficult. As an added benefit, the file paths have a fixed length, so you won't need to shorten paths like the default format does.
* **Use custom ways of serializating different types of data** E.g. XML, HTML or a list of ID's is pretty-printed when serializing. This way, it will be easier to merge conflicts. You can even merge layout settings this way!
* **Enforce strict ordering of all list data** This also helps with merging. It doesn't mean that the order of items in Sitecore changes, because that is stored in a sortorder field.
* **Use pipelines for pretty much everything** So it's easy to extend.
* **Don't remove field values for fields that have been removed** This is to ensure that a change in Sitecore doesn't affect data that isn't relevant to it. E.g. when you remove a field from a template, not all field values are removed; but they shouldn't be removed when you change the value of a different field on an item either.

## Drawbacks

There are a few things that are significant drawbacks with this approach:
* **Maturity** The current serialization format has been used and perfected for quite some time. This new implementation is far from finished and likely contains bugs that need to be resolved.
* **Tooling** Many tools, such as TDS and Unicorn rely on the current format.
* **Sitecore support** The current format is supported by Sitecore. This new format is not (though it is completely open, so you can change it if you need to).
* **Readability** In some ways, readability is greatly improved in comparison with the current format. But the tree structure itself and the fact that there are no visible field names make it harder to read in other ways.

## Serialization format description

The file structure looks like this:
```
+-- serialization_custom
|   +-- [database_name]
|   |   +-- index.json
|   |   +-- [guid_path_5_characters]
|   |   |   +-- [guid].json
```

Here's an example:
```
+-- serialization_custom
|   +-- master
|   |   +-- index.json
|   |   +-- 0
|   |   |   +-- d
|   |   |   |   +-- e
|   |   |   |   |   +-- 9
|   |   |   |   |   |   +-- 5
|   |   |   |   |   |   |   +-- 0de95ae4-41ab-4d01-9eb0-67441b7c2450.json
|   |   +-- 1
|   |   |   +-- 1
|   |   |   |   +-- 1
|   |   |   |   |   +-- 1
|   |   |   |   |   |   +-- 1
|   |   |   |   |   |   |   +-- 11111111-1111-1111-1111-111111111111.json
```

The index.json file describes the tree structure of the items. Only the parent-child relationship between items is described. If an item is moved, only this item is changed. A simple index.json file looks like this:
```javascript
{
  "id": "11111111-1111-1111-1111-111111111111",
  "c": [
    {
      "id": "0de95ae4-41ab-4d01-9eb0-67441b7c2450"
    }
}
```

An individual item json file contains information like item name, template id, branch id, shared field values, language versions and versioned field values. For some field values, specific serialization formats are used to make conflict merging easier.

The following example is a little contrived (e.g. the /sitecore/content item doesn't usually have layout), but shows how it works:
```javascript
{
  "name": "content",
  "templateid": "e3e2d58c-df95-4230-adc9-279924cece84",
  "shared": [
    {
      "id": "06d5295c-ed2f-4a54-9bf2-26228d113318",
      "v": "People/16x16/cubes_blue.png"
    },
    {
      "id": "21f74f6e-42d4-42a2-a4b4-4cefbcfbd2bb",
      "v": "
63af4405-7f65-4079-8c62-5d09c705545b
154dc6da-89c4-4704-8cda-95994d794bea
0e85c8bf-6f59-4907-9e6d-fc603573a798
7146f1a4-45fb-4cec-9855-c95e9e595827
56ef9816-35ad-4160-b5dc-eca7fe7dcfc2
5b3e125b-0f02-4ea3-9c3c-8bdc9ce34a3b
a9649925-64c2-4ef9-aa66-cf20d6925929
bc06ed64-c4a1-4ee2-9835-541e1cc4ccc9
8355eb8a-44ec-47fc-8ffd-6412c400a397
",
      "serializer": "IdList"
    },
    {
      "id": "f1a1fe9e-a60c-4ddb-a3a0-bb5b29fe732e",
      "v": "
<r xmlns:p=\"p\" xmlns:s=\"s\" p:p=\"1\">
  <d id=\"{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}\">
    <r uid=\"{065A7240-E9A3-4629-9140-B5582A3A2D67}\" s:id=\"{493B3A83-0FA7-4484-8FC9-4680991CF743}\" s:ph=\"shared override\" />
  </d>
</r>
",
      "serializer": "Xml"
    }
  ],
  "languages": [
    {
      "lang": "da",
      "versions": [
        {
          "nr": 1,
          "fields": [
            {
              "id": "25bed78c-4957-4165-998a-ca1b52f67497",
              "v": "20150223T132637:635602947975977078Z"
            }
          ]
        }
      ]
    },
    {
      "lang": "en",
      "versions": [
        {
          "nr": 1,
          "fields": [
            {
              "id": "25bed78c-4957-4165-998a-ca1b52f67497",
              "v": "20080818T154900"
            },
            {
              "id": "a60acd61-a6db-4182-8329-c957982cec74",
              "v": "
<p style=\"line-height: 22px;\">
      From a single connected platform that also integrates with
      other customer-facing platforms, to a single view of the
      customer in a big data marketing repository, to a completely
      eliminating much of the complexity that has previously held
      marketers back, the latest version of Sitecore makes customer
      experience highly achievable. Learn how the latest version of
      Sitecore gives marketers the complete data, integrated tools,
      and automation capabilities to engage customers throughout an
      iterative lifecycle &ndash; the technology foundation
      absolutely necessary to win customers for life.
    </p>
    <p>
      For further information, please go to the <a href=
      \"https://doc.sitecore.net/\" target=\"_blank\" title=
      \"Sitecore Documentation site\">Sitecore Documentation site</a>
    </p>
",
              "serializer": "Html"
            }
          ]
        }
      ]
    }
  ]
}
```

## TODO list

1. Create a NuGet package and make it available
2. Add media library support
3. Add a way to exclude a part of the tree for serialization/deserialization
4. Add a way to exclude certain fields from serialization/deserialization
5. Add a data provider for use on developer machines (so that the filesystem always reflects the content in Sitecore)
6. Add a synchronization script to run at startup of Sitecore (so that Sitecore content can be updated from the filesystem automatically)
7. Add support for automatic serialization when items are saved, moved, deleted etc. (as an alternative to 4; similar to Unicorn)