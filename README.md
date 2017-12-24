Asp.Net MVC Core Angular 2-5 template app To run the app You'll need:

1) clone this repository into Your local PC folder
2) open the project's solution file with VisualStudio 2017
3) restore Node (npm) and NuGet packages
4) set the valid connection string for using SqlServer permanent storage in 2 places: in file: S7App\S7Test.Infrastructure.Data.DesignTimeDbContextFactory.cs and also in file: S7App\S7Test.Web.appsettings.json ("DefaultConnection": "Server=MYPCNAME\MYSQLSERVER;Database=S7Test;User ID=sa;Password=pass;...)
5) set "S7Test.Web" as StartUp project, select "Default project"-> Infrastructure\S7Test.Infrastructure.Data in "Package Manager Console" and run command: Add-Migration "InitialCreate" -Verbose
(it will parse S7App\S7Test.Infrastructure.Data.players.json file and insert as initial data into database using EFCore DbContext)
6) run the project with IISExpress in VS2017, navigate to "Register"- page and register a user with prefilled data (may register other users by filling required data fields, they will be added with a role "User", which is required to access the WebApi)
7) navigate to "Login"- page and login with registered user's credentials (for signed in users app will navigate to "Players' list"- page)
8) in Players' List- page there is a table with sorting, paging and filtering (by keyword for starting player-name or team-name), add/update and delete functionality is also implemented.