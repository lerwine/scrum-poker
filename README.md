# ScrumPoker

Scrum poker web application

## Methods of execution

- **Deployment to web host**: The [WebApp](#deployable-web-application) project can be deployed as a .NET web application.
- **Stand-alone web server**: The [StandAloneServer](#stand-alone-web-service) project is a console application that provides a stand-alone web server.
- **Via PowerShell**: The [WebServer](#powershell-based-web-service) script acts as a stand-alone web server which does not require any pre-compiled binaries.

## Shared Web Root Folder

The `wwwRoot` folder contains the static content for the Scrum Poker web application.

Transpiled TypeScript sources located in the `Webapp/ClientApp` folder are emitted to the `wwwRoot/app` folder.

## ScrumPoker Project

This is a shared library which helps to enforce common data contract schemas and routing paths.

The c# language version of this project is explicitly set to version 4.0 so the individual source files can be imported into PowerShell using the `Add-Type` command.

## Deployable Web Application

## Stand-Alone Web Service

## PowerShell-based Web Service
