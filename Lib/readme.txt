Copy the following files from your Sitecore bin folder here to make sure the Solution compiles:
- Sitecore.Kernel.dll
- sitecore.nexus.dll
- Newtonsoft.Json.dll (NuGet was not used here, to ensure the version from your Sitecore is used)
- license.xml (A valid Sitecore license, so Sitecore.FakeDb can work for running unit tests)