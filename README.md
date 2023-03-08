# ScrumPoker

Scrum Planning Poker web application

## Methods of execution

- **Deployment to web host**: The [WebApp](#deployable-web-application) project can be deployed as a .NET web application.
- **Stand-alone web server**: The [StandAloneServer](#stand-alone-web-service) project is a console application that provides a stand-alone web server.
- **Via PowerShell**: The [WebServer](#powershell-based-web-service) script acts as a stand-alone web server which does not require any pre-compiled binaries.

### Shared Web Root Folder

The [wwwRoot](wwwRoot) folder contains the static content for the Scrum Poker web application.

Transpiled TypeScript sources located in the [Webapp/ClientApp](WebApp/ClientApp) folder are emitted to the [wwwRoot/app](wwwRoot/app) folder.

### ScrumPoker Project

The [ScrumPoker](ScrumPoker/ScrumPoker.csproj) project is a shared library which helps to enforce common data contract schemas and routing paths.

The c# language version of this project is explicitly set to version 4.0 so the individual source files can be imported into PowerShell using the `Add-Type` command.

### Deployable Web Application

The [ScrumPoker.WebApp](WebApp/ScrumPoker.WebApp.csproj) project can be deployed as a web application.

### Stand-Alone Web Service

The [ScrumPoker.StandaloneServer](StandaloneServer/ScrumPoker.StandaloneServer.csproj) project produces a console application that serves as a stand-alone web service for the Scrum Poker web application.

### PowerShell-based Web Service

The [WebServer](WebServer.ps1) script can be used to run a web server for the Scrum Poker application using PowerShell.

This can be used for circumstances the web application is not deployed to a web server and security restrictions do not allow the stand-alone application to be executed.

## Common API

### Data Contracts

### API Routes

- [User Controller](./API%20Routes.md#user-controller)

## References

- [SQLite Data Types](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/types)
- [EF Core](https://learn.microsoft.com/en-us/ef/core/)
- [Configuration in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration)
- [Make HTTP requests with the HttpClient class](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient)
  - [JsonContent Class](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json.jsoncontent?view=net-7.0)
  - [StringContent Class](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.stringcontent?view=net-7.0)
