# .NET Core Entity Framework and functions

C# Function that uses dependency injection and entity framework core with Azure SQL.  A few things of note:

I want to use design-first migration, so I created a `IDesignTimeDbContextFactory` for the tooling to generate the right `DbContext`.

I also had to add a post-build step to the `.csproj` file to make sure the `.dll` for the project is where the entity framework tooling expected:

```xml
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
  <Exec Command="copy /Y &quot;$(TargetDir)bin\$(ProjectName).dll&quot; &quot;$(TargetDir)$(ProjectName).dll&quot;" />
</Target>
```
