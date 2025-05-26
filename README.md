# Craftique_BE
Getting started With Project Apple Mart, My team used the first code to code this project, I have some guidelines
1.Code First Approach Guide
Code First is a method of developing databases using source code rather than starting from a direct database design.
Step 1 Install Entity Framework Core
You can add it using the following command in the Package Manager Console or .NET CLI:
Install with .NET CLI
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
Install with Package Manager Console
Click Tool -> Nuget Package Manager -> Package Manager Console
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Step 2 Create Model
Step 3 Create DbContext
Step 4 Configure ConnectionString Add connection string to appsettings.json (Change Server, Database, UserID and Password your SqlServer)
Step 5 Register DbContext in the Program.cs file, add the DbContext to the service
**Step 6 Add Migration and then update database **
To create a Migration, you use the following command in the Package Manager Console or .NET CLI:
Add with .NET CLI
dotnet ef migrations add InitialCreate -> dotnet ef database update
InitialCreate: This is the name of the migration. You can change this name as you wish (e.g. AddCourseTable, UpdateSchema, etc.)
Add with Package Manager Console
Click Tool -> Nuget Package Manager -> Package Manager Console -> Add-Migration InitialCreate -> Choose Project have Migration-> Update-Database
InitialCreate: This is the name of the migration. You can change this name as you wish (e.g. AddCourseTable, UpdateSchema, etc.)
