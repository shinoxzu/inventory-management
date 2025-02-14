# Inventory Management API

An API project for Inventory Management system built with C# using ASP.NET Core and Entity Framework Core.

## Getting Started

This guide explains how to run the project in **development mode** using Docker Compose.

### Prerequisites

- Docker (+docker-compose)
- .NET SDK 9.0 or later

### Configuration Setup

1. Navigate to the `InventoryManagement.API` project directory
2. Create a new file named `appsettings.Development.json`
3. Copy the contents from `appsettings.Example.json` and update the values according to your environment

Note: If you modify the default database connection string, make sure to update it in the `docker-compose.dev.yaml` file as well.

### Running the Application

Execute the following command in the root directory:

```bash
docker-compose -f docker-compose.dev.yaml up
```

The API should now be running and accessible at `http://localhost:4001` (or the port specified in your configuration).

## Production Deployment

You can use the existing Dockerfile and docker-compose file for production deployment, but you may need to customize docker-compose file: change some variables (BUILD_CONFIGURATION, ASPNETCORE_ENVIRONMENT), disable automatic migrations, add nginx service and so on.
