# Release Note Builder - Clean Architecture

This project has been refactored to follow **Clean Architecture** principles, ensuring separation of concerns, testability, and maintainability.

## Architecture Overview

The solution is organized into four main layers:

```
ReleaseNoteBuilder/
├── Core/                          # Domain Layer (Enterprise Business Rules)
│   ├── Entities/                  # Domain entities
│   │   └── Release.cs            # Core business entity
│   └── Interfaces/                # Repository interfaces
│       └── IReleaseRepository.cs
│
├── Application/                   # Application Layer (Use Cases)
│   ├── DTOs/                     # Data Transfer Objects
│   │   ├── ReleaseDto.cs
│   │   ├── CreateReleaseDto.cs
│   │   └── UpdateReleaseDto.cs
│   ├── Interfaces/               # Service interfaces
│   │   ├── IReleaseService.cs
│   │   └── IReleaseNoteGenerator.cs
│   └── Services/                 # Business logic implementation
│       ├── ReleaseService.cs
│       └── ReleaseNoteGenerator.cs
│
├── Infrastructure/                # Infrastructure Layer (External Concerns)
│   ├── Persistence/              # Database implementation
│   │   ├── ApplicationDbContext.cs
│   │   └── Repositories/
│   │       └── ReleaseRepository.cs
│   └── ExternalServices/         # External service integrations
│       ├── AzureDevOpsService.cs
│       └── ApprovalService.cs
│
└── Pages/                        # Presentation Layer (UI)
    ├── ListReleases.razor
    ├── Release.razor
    └── ReleaseDetails.razor
```

## Layer Responsibilities

### 1. Core (Domain Layer)
- **Contains**: Business entities and repository interfaces
- **Dependencies**: None (completely independent)
- **Purpose**: Encapsulates enterprise-wide business rules

**Files:**
- `Core/Entities/Release.cs` - Core domain entity with business methods
- `Core/Interfaces/IReleaseRepository.cs` - Repository contract

### 2. Application (Application Business Rules)
- **Contains**: Use cases, DTOs, and service interfaces
- **Dependencies**: Core only
- **Purpose**: Contains application-specific business logic

**Files:**
- `Application/DTOs/*` - Data transfer objects for API boundaries
- `Application/Interfaces/IReleaseService.cs` - Service contract
- `Application/Services/ReleaseService.cs` - Business logic implementation

### 3. Infrastructure (Frameworks & Drivers)
- **Contains**: Database, external services, file system access
- **Dependencies**: Core, Application
- **Purpose**: Implements interfaces defined in Core and Application

**Files:**
- `Infrastructure/Persistence/ApplicationDbContext.cs` - EF Core DbContext
- `Infrastructure/Persistence/Repositories/ReleaseRepository.cs` - Repository implementation
- `Infrastructure/ExternalServices/*` - External integrations (ADO, Approvals)

### 4. Presentation (UI)
- **Contains**: Blazor pages and components
- **Dependencies**: Application (through interfaces)
- **Purpose**: User interface and presentation logic

**Files:**
- `Pages/ListReleases.razor` - Main releases list page
- Uses `IReleaseService` instead of direct repository access

## Dependency Flow

```
Presentation → Application → Core
Infrastructure → Application → Core
```

The dependency rule: **Source code dependencies must point inward**. Inner layers know nothing about outer layers.

## Benefits of This Architecture

### 1. **Testability**
- Each layer can be tested independently
- Mock implementations can be easily created for interfaces
- Business logic is isolated from infrastructure concerns

### 2. **Maintainability**
- Clear separation of concerns
- Changes in one layer don't affect others
- Easy to locate and modify functionality

### 3. **Flexibility**
- Can swap database implementations (e.g., SQL Server instead of In-Memory)
- Can change UI framework without affecting business logic
- Easy to add new features following established patterns

### 4. **Independence**
- Business logic doesn't depend on frameworks
- Domain entities are plain C# objects (POCO)
- Infrastructure details are isolated

## Key Patterns Used

### Dependency Injection
All dependencies are injected through constructors and registered in `Program.cs`:

```csharp
// Infrastructure
builder.Services.AddDbContext<ApplicationDbContext>(...);
builder.Services.AddScoped<IReleaseRepository, ReleaseRepository>();

// Application
builder.Services.AddScoped<IReleaseService, ReleaseService>();
builder.Services.AddScoped<IReleaseNoteGenerator, ReleaseNoteGenerator>();
```

### Repository Pattern
Data access is abstracted through the `IReleaseRepository` interface:

```csharp
public interface IReleaseRepository
{
    Task<List<Release>> GetAllAsync();
    Task<Release?> GetByIdAsync(int id);
    Task<Release> AddAsync(Release release);
    Task UpdateAsync(Release release);
    Task DeleteAsync(int id);
}
```

### DTO Pattern
Data transfer between layers uses dedicated DTOs:
- `ReleaseDto` - For reading data
- `CreateReleaseDto` - For creating new releases
- `UpdateReleaseDto` - For updating existing releases

This prevents exposing domain entities to the UI layer.

## How to Add New Features

### Adding a New Entity

1. Create entity in `Core/Entities/`
2. Create repository interface in `Core/Interfaces/`
3. Create DTOs in `Application/DTOs/`
4. Create service interface in `Application/Interfaces/`
5. Implement service in `Application/Services/`
6. Implement repository in `Infrastructure/Persistence/Repositories/`
7. Register in `Program.cs`
8. Use in Blazor pages through service interface

### Example: Adding Work Items

```csharp
// 1. Core/Entities/WorkItem.cs
public class WorkItem { ... }

// 2. Core/Interfaces/IWorkItemRepository.cs
public interface IWorkItemRepository { ... }

// 3. Application/Services/WorkItemService.cs
public class WorkItemService : IWorkItemService { ... }

// 4. Infrastructure/Persistence/Repositories/WorkItemRepository.cs
public class WorkItemRepository : IWorkItemRepository { ... }

// 5. Program.cs
builder.Services.AddScoped<IWorkItemRepository, WorkItemRepository>();
builder.Services.AddScoped<IWorkItemService, WorkItemService>();
```

## Migration from Old Structure

| Old Location | New Location | Layer |
|-------------|-------------|-------|
| `Models/ReleaseRecord.cs` | `Core/Entities/Release.cs` | Core |
| `Data/AppDbContext.cs` | `Infrastructure/Persistence/ApplicationDbContext.cs` | Infrastructure |
| `Services/ReleaseRepository.cs` | `Infrastructure/Persistence/Repositories/ReleaseRepository.cs` | Infrastructure |
| `Services/ReleaseService.cs` | `Application/Services/ReleaseService.cs` | Application |
| `Services/AdoService.cs` | `Infrastructure/ExternalServices/AzureDevOpsService.cs` | Infrastructure |
| `Services/ApprovalService.cs` | `Infrastructure/ExternalServices/ApprovalService.cs` | Infrastructure |

## Next Steps

1. **Add Unit Tests**: Create test projects for each layer
2. **Add Validation**: Implement FluentValidation in Application layer
3. **Add Logging**: Implement structured logging with Serilog
4. **Add API Layer**: Create Web API controllers if needed
5. **Implement ADO Integration**: Complete AzureDevOpsService implementation
6. **Add Authentication**: Implement user authentication and authorization

## References

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Microsoft: Common web application architectures](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
