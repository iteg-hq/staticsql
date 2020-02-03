# StaticSQL

StaticSQL lets you write metadata-driven templates in Visual Studio.

Rather than write e.g. a loader for each dimension in your data warehouse or for each history table in your staging layer, you can write a T4 template that generates all the loader code based on entity metadata stored in json files.

StaticSQL has a simple user interface and a minimum of dependencies.

## Installation

StaticSQL is a library that is can be installed into the Global Assembly Cache on your development machine using [gacutil](https://docs.microsoft.com/en-us/dotnet/framework/app-domains/install-assembly-into-gac#global-assembly-cache-tool):

- Make sure you have Visual Studio installed.

- Download StaticSQL.dll (and dependencies) from https://github.com/iteg-hq/staticsql/releases.

- Start the *Developer Command Prompt for Visual Studio* as an administrator.

- Navigate to the directory containing the dll's and run:

  ~~~
  gacutil -i Newtonsoft.Json.dll
  gacutil -i SystemIO.Abstractions.dll
  gacutil -i StaticSQL.dll
  ~~~

## Creating your first T4 template with StaticSQL

This section shows how to create a database project in Visual Studio, add metadata definitions and generate a simple change-tracking mechanism for a single entity representing a person:

- A table storing the full history for each person.
- A stored procedure to update the data for a person.
- A view to expose the most recent data for each person.

### Requirements

You will need the following installed to complete the tutorial:

- StaticSQL (see previous section) .
- Visual Studio with the "Data storage an processing" workload.

### Create a new project

First, create a new database project to hold the generated SQL:

- Open Visual Studio.
- Create a solution containing a new SQL Server Database Project.

### Create an entity folder

Next, create some entity metadata to drive the templates:

- Create a new folder in the project called `StaticSQL`. This is where the entity metadata will live.

- Create a text file in the new folder, rename it `Person.json` and set the content to:

  ~~~json
  {
      "schema": "dbo",
      "name": "Person",
      "description": "A human individual.",
      "attributes": [
          {
              "name": "First Name",
              "data_type": "NVARCHAR(50)",
              "is_nullable": false,
              "description": "The first name of the person (e.g Bruce)."
          },
          {
              "name": "Last Name",
              "data_type": "NVARCHAR(50)",
              "is_nullable": false,
              "description": "The last name of the person (e.g Wayne)."
          },
          {
              "name": "Age",
              "data_type": "INT",
              "is_nullable": true,
              "description": "The age of the person, in years."
        }
      ]
  }
  ~~~
  
  This is entity metadata that StaticSQL will make available to the templates.

### Create an StaticSQL project file

Now, create a project file, that points to the entity metadata:

- Create a text file in the root of the project (right click the project, "Add" > "New Item..." > "Text File").

- Rename the file `sample.staticsql`.

- Set the contents to:

  ~~~json
  {
  	"entity_folder": "StaticSQL"
  }
  ~~~

  This tells StaticSQL where the entity metadata lives. The path of the entity folder is relative to the directory of the project file.
  
  When processing the template, StaticSQL will search the directory of the template, then the parent directory and so on until it finds the project file. Placing the StaticSQL project file in the root of the Visual Studio project makes the metadata available to all templates in the project.

### Create T4 templates

Next, add some templates to the entity metadata folder:

-  Add a new folder to the project called `T4`.

-  Add a template to the new folder ("Add" > "New Item..." > "Text Template").

- Rename it `Table.tt` and set the content to:

    ~~~c#
    <#@ template debug="false" hostspecific="true" language="C#" #>
    <#@ output extension=".sql" #>
    <#@ assembly name="System.Core" #>
    <#@ assembly name="StaticSQL.dll" #>
    <#@ import namespace="StaticSQL" #>
    <# Project project = Project.Load(Host.ResolvePath(".")); #>
    <# project.Formatter = FormatterFactory.PascalCaseQuoteIfNeeded(); #>
    
    <# foreach(var entity in project.Entities) { #>
    CREATE TABLE dbo.<#= entity.Name #>Table (
        <#= entity.Name #>ID INT NOT NULL
    <# foreach(var attribute in entity.Attributes) { #>
      , <#= attribute.Name #> <#= attribute.DataType #> <#= attribute.NullabilityString #>
    <# } #>
      , InsertedOn DATETIME2(7) DEFAULT SYSUTCDATETIME()
      , CONSTRAINT PK_<#= entity.Name #> PRIMARY KEY ( <#= entity.Name #>ID, InsertedOn )
      )
    
    GO
<# } #>
    ~~~

- Save it - Visual Studio should generate a file beneath the template file. The table definition in that file corresponds closely to the entity, only we've added an ID and an insert timestamp.

- Next, create a stored procedure to insert rows into the table. Add a new template called `InsertProcedure.tt` and set the content to:

    ~~~c#
    <#@ template debug="false" hostspecific="true" language="C#" #>
    <#@ output extension=".sql" #>
    <#@ assembly name="System.Core" #>
    <#@ assembly name="StaticSQL.dll" #>
    <#@ import namespace="StaticSQL" #>
    <# Project project = Project.Load(Host.ResolvePath(".")); #>
    <# project.Formatter = FormatterFactory.PascalCaseQuoteIfNeeded(); #>
    
    <# foreach(var entity in project.Entities) { #>
    CREATE PROCEDURE dbo.Add<#= entity.Name #>
        @<#= entity.Name #>ID INT
    <# foreach(var attribute in entity.Attributes) { #>
      , @<#= attribute.Name #> <#= attribute.DataType #>
    <# } #>
    AS
    
    INSERT INTO dbo.<#= entity.Name #>Table (
        <#= entity.Name #>ID
    <# foreach(var attribute in entity.Attributes) { #>
      , <#= attribute.Name #>
    <# } #>
    )
    VALUES (
        @<#= entity.Name #>ID
    <# foreach(var attribute in entity.Attributes) { #>
      , @<#= attribute.Name #>    
    <# } #>
    )
    
    GO
    <# } #>
    ~~~

-  Finally, create a view to show only the last row inserted for each ID: Add a new template called `View.tt` and set the content to:

    ~~~c#
    <#@ template debug="false" hostspecific="true" language="C#" #>
    <#@ output extension=".sql" #>
    <#@ assembly name="System.Core" #>
    <#@ assembly name="StaticSQL.dll" #>
    <#@ import namespace="StaticSQL" #>
    <# Project project = Project.Load(Host.ResolvePath(".")); #>
    <# project.Formatter = FormatterFactory.PascalCaseQuoteIfNeeded(); #>
    
    <# foreach(var entity in project.Entities) { #>
    CREATE VIEW dbo.<#= entity.Name #>
    AS
    SELECT
    <# foreach(var attribute in entity.Attributes) { #>
      <#= attribute.CommaBefore #> <#= attribute.Name #>    
    <# } #>
    FROM dbo.<#= entity.Name #>Table AS outer_table
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.<#= entity.Name #>Table AS inner_table
        WHERE inner_table.<#= entity.Name #>ID = outer_table.<#= entity.Name #>ID
          AND inner_table.InsertedOn > outer_table.InsertedOn
      )
    
    <# } #>
    ~~~

At this point you may want to deploy the generated code to a server and do something like:

~~~mssql
EXEC dbo.AddPerson 1, 'Clark', 'Kent', 39;
SELECT * FROM dbo.Person;

EXEC dbo.AddPerson 2, 'Bruce', 'Wayne', 42;
SELECT * FROM dbo.Person;

EXEC dbo.AddPerson 2, 'Bruce', 'Wayne', 43;
SELECT * FROM dbo.Person;
SELECT * FROM dbo.PersonTable;
~~~

### Add template processing to the build

Templates are not processed automatically when you change the content of the entity files, so you'll probably want to trigger template processing as part of your build.

This involves hand-editing your project file (if you do this to an existing project file, make sure you have some way to revert the change if it doesn't go as planned):

- Right-click the project, click "Unload Project"

- Right-click again, click "Edit Sample.sqlproj"

- Find the last `<Import>` xml tag in the document.

- Right after that tag, insert the following tags:

  ~~~
    <Import Project="$(MSBuildExtensionsPath)\Microsoft\VisualStudio\v$(VisualStudioVersion)\TextTemplating\Microsoft.TextTemplating.targets" />
    <PropertyGroup>
      <TransformOnBuild>true</TransformOnBuild>
      <TransformOutOfDateOnly>false</TransformOutOfDateOnly>
    </PropertyGroup>
  ~~~

  This makes the build process all templates at the start of every build.

- Close the file.

- Right-click the project and choose "Reload Project".

- Check that templates are processed on build to pressing `Ctrl`+`Shift`+`B`.

### Add another entity

Next, try adding a new entity:

- Copy `Person.json`, and rename the copy `Dog.json`.

- Edit the entity name and attributes as needed: the entity name should be "Dog", and dogs probably don't have last names.

- Build the project again (or opening the templates and saving them, if you skipped the last step).

  A table, a procedure and a view are generated for the Dog entity now, as well. :dog:

### Modify the templates

Next, try modifying the templates:

- Add delta detection to the procedure so that a row is not added to the table if the data has not changed.
- Add support for [soft deletion](https://en.wiktionary.org/wiki/soft_deletion). You'll need to create:
  - a new stored procedure to soft-delete an instance given and id.
  - a deletion flag on the table.
  - a filter in the view so that it hides the soft-deleted rows.

