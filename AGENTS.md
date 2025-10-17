# Development Guidelines

## Architectural Requirements
- Use Clean Architecture.
- Define separate layers: Domain, Application, Infrastructure, and API.
- Use Entity Framework Core for data access.
- Store configuration in `appsettings.json`.
- Implement RESTful controllers with validation and DTOs.
- Configure dependency injection in `Program.cs`.
- Provide structured logging with Serilog.
- Write unit tests with xUnit and FluentAssertions.
- Document APIs with Swagger/OpenAPI.

## Code Style Guidelines
- Always use explicit type definitions; `var` is not allowed.
- Choose clear, descriptive variable names.
- Use camelCase for local variables, PascalCase for methods and class members, and UPPER_SNAKE_CASE for TA properties.
- API endpoints must not return TA elements.
- LINQ and Entity Framework transformations are allowed with simple, reasonable usage.
- Use four spaces instead of tabs.
- Use CRLF for new lines.
- Place braces `{}` on new lines.
- Insert a blank line between methods.
- Maintain sensible whitespace inside methods; keep related code together.
- Include a space before and after any operator (except for the `++` operator).
- Use the `++` operator only as a postfix.
- Prefer `double` over `float`.
- Prefer `List<T>` over arrays.
- Avoid `out` parameters and optional parameters.
