# BookStore Solution

This is a .NET solution for a bookstore application with API, web interface, and various services.

## Prerequisites

- .NET SDK 10.0.104 or later (check global.json for exact version)

## How to Run

1. Navigate to the project directory:

   ```
   cd /Users/sakshamchauhan/Downloads/BookStoreSolution
   ```

2. Build the entire solution:

   ```
   dotnet build BookStoreSolution.sln
   ```

3. Run the API (starts on port specified in launchSettings.json, typically 5000/5001):

   ```
   dotnet run --project BookStore.API/BookStore.API.csproj
   ```

4. Run the Web application (starts on port specified in launchSettings.json):

   ```
   dotnet run --project BookStore.Web/BookStore.Web.csproj
   ```

5. Run all tests:
   ```
   dotnet test BookStoreSolution.sln
   ```

## Project Structure

- **BookStore.API**: ASP.NET Core Web API with controllers for books, orders, auth, etc.
- **BookStore.Web**: ASP.NET Core MVC web application
- **BookStore.Application**: Application services, DTOs, and business logic
- **BookStore.Domain**: Domain entities and models
- **BookStore.Infrastructure**: Data access, repositories, and external services
- **BookStore.Shared**: Shared utilities and common classes
- **BookStore.Tests.NUnit**: NUnit integration tests
- **BookStore.Tests.XUnit**: xUnit unit tests

## Available Tasks in VS Code

You can also use the predefined tasks in VS Code:

- `build`: Builds the solution
- `run-api`: Runs the API project
- `run-web`: Runs the web project
- `test`: Runs all tests
