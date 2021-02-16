# Publishing to nuget.org
If you want your plugin to be installable from `openmod install`, you will need to publish it to nuget.org.  
You will need a Microsoft account for [nuget.org](https://www.nuget.org/). 

## Preparing your plugin for NuGet
Step 1: Add the following to your plugins .csproj:
```xml
<PropertyGroup>
  <PackageId>Your PackageId</PackageId> <!-- must be unique, should be same as your plugin ID -->
  <PackageDescription>Your plugin description</PackageDescription>
  <PackageLicenseExpression>Your License</PackageLicenseExpression> <!-- see https://spdx.org/licenses/ -->
  <PackageAuthor>Your name</PackageAuthor>
  <PackageTags>openmod openmod-plugin XXX</PackageTags> <!-- XXX can be unturned, unityengine or universal depending on your plugin -->
  <Version>x.x.x</Version> <!-- Your plugins version. Must be semversion, see https://semver.org/ -->
  <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
  <GenerateNugetPackage>true</GenerateNugetPackage>  
</PropertyGroup>
```  
Step 2: Sign in to [nuget.org](https://nuget.org).  
Step 3: Click on your username, select API Keys.  
Step 4: Select "Create". Add a name, select the `Push` scope and add `*` as Glob pattern, then select create.  
Step 5: Copy your newly created key. Save your key securely as you cannot retrieve it again.  

## Uploading the plugin
Step 1: Navigate to your plugins folder. Execute the following command: `dotnet build --configuration Release`.  
Step 2: Go to `bin/Release/` and push to NuGet:  
`dotnet push <yourpackageid.x.x.x.nupkg> -n -k <your nuget.org key> -s https://api.nuget.org/v3/index.json`
    
You can now install your plugin with `openmod install <YourPackageId>`.

> [!NOTE]
> You may get the "PackageOrVersionNotFound" error after trying to install your plugin. This means your upload has not  been verified or indexed yet. It may take up to an hour until uploads to nuget.org are verified and indexed.

For more, read the [Publishing packages](https://docs.microsoft.com/en-us/nuget/nuget-org/publish-a-package) and [Create and publish a NuGet package (dotnet CLI)](https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli) pages on docs.microsoft.com.

## Plugin versioning
Like OpenMod, NuGet also uses Semantic Versioning 2.0.0.

Here is a short summary on how versioning works:

- Semantic Versioning follow the following template: `Major.Minor.Patch`.
- First Major is compared, then Minor and finally Patch. So `3.0.1` > `2.1.5` > `1.45.8`  > `0.999.0` > `0.99.0`.
- Your plugin starts with version `0.1.0` and increments the "Minor" version as it progresses towards release (e.g. `0.2.0`).
- When your plugin is ready for release, version `1.0.0` is used.
- For bug fixes, increase the "Patch" version (e.g. `1.0.1`).
- For new feature additions, increase the "Minor" version (e.g. `1.1.0`).
- For breaking changes (e.g. public API changes or breaking configs, etc.), increment the Major version (e.g. `2.0.0`).
- You can also use -rc1, -beta, -alpha, -beta2 etc. suffixes to indicate a pre release version (e.g. `2.0.0-rc1`)  
In this case version comparison is done alphabetically, e.g. `1.0.0-rc2` is newer than `1.0.0-rc1` and `1.0.0-beta`, but `1.0.0` is the newest because it is not a pre release version.
- You can attach any arbitary information, such as the commit hash, by using the + suffix (e.g. `1.0.0+525ffaa` or `1.0.0-rc1+525ffaa`). Everything after the + is ignored and not used for version comparison.

The following are valid semantic version examples:

* `0.0.1`
* `1.0.9`
* `1.45.6`
* `1.0.0-alpha`
* `1.0.0-beta1`
* `1.3.5-rc1`
* `1.3.5-rc2+somearbitaryinformation`

The following are **not valid**:

* `1.0.0.0`
* `v1.0.0`

For more information, check out the [semantic versioning website](https://semver.org).

## Publishing with GitHub workflows
You can use GitHub workflows to publish to NuGet with one click.  

The following assumes that your plugins source code is in the `YOURPLUGIN` folder under your repositories root directory.  
Create the following file in your repository's `.github/workflows` folder.

```yaml
name: Deployment

on:
  workflow_dispatch:
    inputs:
      version:
        description: 'Plugin Version (SemVer: https://semver.org)'
        required: true
jobs:
  deploy:
    name: "NuGet Deployment"
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v2
      name: Checkout Repository
      with:
        fetch-depth: 0
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 5.0.x
    - name: Install dependencies
      run: dotnet restore
    - name: Update version
      run: "sed -i \"s#<Version>0.0.0</Version>#<Version>${{ github.event.inputs.version }}</Version>#\" YOURPLUGIN/YOURPLUGIN.csproj"  
    - name: Update package version
      run: "sed -i \"s#<PackageVersion>0.0.0</PackageVersion>#<PackageVersion>${{ github.event.inputs.version }}</PackageVersion>#\" YOURPLUGIN/YOURPLUGIN.csproj"  
    - name: Update informational version
      run: "sed -i \"s#<InformationalVersion>0.0.0</InformationalVersion>#<InformationalVersion>${{ github.event.inputs.version }}</InformationalVersion>#\" YOURPLUGIN/YOURPLUGIN.csproj"              
    - name: Build
      run: dotnet build YOURPLUGIN/YOURPLUGIN.csproj --configuration Release --no-restore
    - name: Push to NuGet
      run: dotnet nuget push YOURPLUGIN/bin/Release/*.nupkg
             --api-key ${{ secrets.NUGET_DEPLOY_KEY }}
             --source https://api.nuget.org/v3/index.json
```

After that you must edit the `<Version>x.x.x</Version>` in your .csproj and set it to `<Version>0.0.0</Version>`. Also add `<PackageVersion>0.0.0</PackageVersion>` and `<InformationalVersion>0.0.0</InformationalVersion>`.

Finally, to finish setting up the workflow, you must add your NuGet secret:

- Go to https://github.com/YOURNAME/YOURPLUGIN/settings/secrets
- Create a new secret
- Name it "NUGET_DEPLOY_KEY" and add your NuGet API Key to it

To publish a new version of your plugin, all you have to do is to:

- Go to your repository on GitHub
- Click on "Actions"
- Select "NuGet Deployment" on the left
- Click on "Run workflow" on the right
- Enter plugin version and be sure to follow the versioning rules from above, then click "Run workflow".
- Check the workflow log if any error occured.
- Done, your plugin has been built and uploaded to nuget.org.

Here is an example from the [NewEssentials](https://github.com/Kr4ken-9/NewEssentials) plugin made by Kr4ken:
![GitHub workflow NuGet deployment example](https://i.imgur.com/MumDQS5.png)

It takes about 10-30 minutes for your plugin to be published to nuget.org. After waiting you can use `openmod install <YourPluginsPackageId>` to install your plugin.
