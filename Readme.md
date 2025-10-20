# Azure Trial Function App

![CI/CD Pipeline](https://github.com/brijeshspatel/Azure-Trial-001-MyFunctionApp/actions/workflows/ci-cd.yml/badge.svg)

This repository contains a sample Azure Functions application built with C# 12 and .NET 8, demonstrating a simple HTTP-triggered function. The solution includes unit tests using xUnit, Moq, and AutoFixture.

## Features
- **Azure Function (HTTP Trigger):** Responds to GET and POST requests, returning a personalised greeting based on the `name` parameter in the query string or request body.
- **Logging:** Uses Serilog for structured, industry-grade logging. Logs are written to the console and can be sent to Azure Application Insights for centralised monitoring.
- **Unit Testing:** Comprehensive tests for function behaviour using xUnit, Moq, and AutoFixture.

## Tech Stack
- .NET 8
- C# 12
- Azure Functions
- Serilog (logging)
- xUnit (testing)
- Moq (mocking)
- AutoFixture (test data generation)

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Azure Functions Core Tools](https://learn.microsoft.com/en-gb/azure/azure-functions/functions-run-local)
- Visual Studio 2022 or later (recommended)

### Running the Function Locally
1. Clone the repository:
   ```sh
   git clone <repository-url>
   ```
2. Navigate to the solution directory:
   ```sh
   cd Azure-Trial-001-MyFuntionApp
   ```
3. Restore dependencies:
   ```sh
   dotnet restore
   ```
4. Start the Azure Function locally:
   ```sh
   dotnet build
   func start
   ```

### Testing
Run unit tests with:
```sh
dotnet test
```

## Logging

This project uses [Serilog](https://serilog.net/) for structured, industry-grade logging. Logs are written to the console and can be sent to Azure Application Insights for centralised monitoring.

### Configuration
Serilog is configured in `Startup.cs`. To enable Application Insights, provide your instrumentation key in the logger configuration.

### Usage
Structured logging is used throughout the function code, e.g.:
```
log.LogInformation("Processing request for {Name}", name);
```

## Project Structure
- `MyFunctionApp/` - Main Azure Functions project
- `MyFunctionApp.Tests/` - Unit test project
- `.gitignore` - Git ignore rules for the solution
- `local.settings.json` - Local development settings (excluded from source control)

## Usage
Send a GET or POST request to the function endpoint with a `name` parameter. Example:
```sh
curl -X GET "http://localhost:7126/api/Function1?name=Alice"
```
Or POST:
```sh
curl -X POST "http://localhost:7126/api/Function1" -H "Content-Type: application/json" -d '{"name":"Alice"}'
```

## Contributing
Contributions are welcome! Please raise an issue or submit a pull request for improvements.

## Licence
This project is licensed under the MIT Licence.

## Support
For help with Azure Functions, see the [official documentation](https://learn.microsoft.com/en-gb/azure/azure-functions/).
