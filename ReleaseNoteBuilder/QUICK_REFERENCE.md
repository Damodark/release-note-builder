# Clean Architecture Quick Reference

## 📋 Quick Rules

### The Dependency Rule
**Inner layers NEVER depend on outer layers**

```
Core ← Application ← Infrastructure
  ↑                      
  └─────────────────── Presentation
```

## 🎯 Where to Put Things

| What | Where | Example |
|------|-------|---------|
| Business entities | `Core/Entities/` | `Release.cs` |
| Repository interfaces | `Core/Interfaces/` | `IReleaseRepository.cs` |
| DTOs | `Application/DTOs/` | `ReleaseDto.cs` |
| Service interfaces | `Application/Interfaces/` | `IReleaseService.cs` |
| Business logic | `Application/Services/` | `ReleaseService.cs` |
| Database context | `Infrastructure/Persistence/` | `ApplicationDbContext.cs` |
| Repository implementation | `Infrastructure/Persistence/Repositories/` | `ReleaseRepository.cs` |
| External APIs | `Infrastructure/ExternalServices/` | `AzureDevOpsService.cs` |
| Blazor pages | `Pages/` | `ListReleases.razor` |

## 🔨 Adding a New Feature

### Example: Adding WorkItem Entity

#### Step 1: Core Layer
```csharp
// Core/Entities/WorkItem.cs
public class WorkItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}

// Core/Interfaces/IWorkItemRepository.cs
public interface IWorkItemRepository
{
    Task<List<WorkItem>> GetAllAsync();
    Task<WorkItem?> GetByIdAsync(int id);
    Task<WorkItem> AddAsync(WorkItem workItem);
}
```

#### Step 2: Application Layer
```csharp
// Application/DTOs/WorkItemDto.cs
public class WorkItemDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}

// Application/Interfaces/IWorkItemService.cs
public interface IWorkItemService
{
    Task<List<WorkItemDto>> GetAllWorkItemsAsync();
}

// Application/Services/WorkItemService.cs
public class WorkItemService : IWorkItemService
{
    private readonly IWorkItemRepository _repository;

    public WorkItemService(IWorkItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<WorkItemDto>> GetAllWorkItemsAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(MapToDto).ToList();
    }

    private WorkItemDto MapToDto(WorkItem item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Description = item.Description
    };
}
```

#### Step 3: Infrastructure Layer
```csharp
// Infrastructure/Persistence/Repositories/WorkItemRepository.cs
public class WorkItemRepository : IWorkItemRepository
{
    private readonly ApplicationDbContext _context;

    public WorkItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkItem>> GetAllAsync()
    {
        return await _context.WorkItems.ToListAsync();
    }
    // ... other methods
}

// Update ApplicationDbContext.cs
public DbSet<WorkItem> WorkItems => Set<WorkItem>();
```

#### Step 4: Register in Program.cs
```csharp
builder.Services.AddScoped<IWorkItemRepository, WorkItemRepository>();
builder.Services.AddScoped<IWorkItemService, WorkItemService>();
```

#### Step 5: Use in Blazor Page
```razor
@page "/workitems"
@inject IWorkItemService WorkItemService

<h3>Work Items</h3>

@foreach (var item in workItems)
{
    <p>@item.Title</p>
}

@code {
    List<WorkItemDto> workItems;

    protected override async Task OnInitializedAsync()
    {
        workItems = await WorkItemService.GetAllWorkItemsAsync();
    }
}
```

## 🧪 Testing Patterns

### Unit Test Example
```csharp
public class ReleaseServiceTests
{
    [Fact]
    public async Task GetAllReleasesAsync_ReturnsAllReleases()
    {
        // Arrange
        var mockRepo = new Mock<IReleaseRepository>();
        mockRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Release> { /* test data */ });

        var service = new ReleaseService(mockRepo.Object);

        // Act
        var result = await service.GetAllReleasesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }
}
```

## 🚫 Common Mistakes

### ❌ DON'T
```csharp
// DON'T inject DbContext in Blazor pages
@inject ApplicationDbContext _context

// DON'T use entities in UI
List<Release> releases; // ❌

// DON'T skip interfaces
public class ReleaseService // ❌ missing interface
```

### ✅ DO
```csharp
// DO inject services through interfaces
@inject IReleaseService ReleaseService

// DO use DTOs in UI
List<ReleaseDto> releases; // ✅

// DO implement interfaces
public class ReleaseService : IReleaseService // ✅
```

## 📊 Layer Communication

### UI → Service
```razor
@inject IReleaseService ReleaseService

@code {
    var releases = await ReleaseService.GetAllReleasesAsync();
}
```

### Service → Repository
```csharp
public class ReleaseService : IReleaseService
{
    private readonly IReleaseRepository _repository;

    public async Task<List<ReleaseDto>> GetAllReleasesAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MapToDto).ToList();
    }
}
```

### Repository → Database
```csharp
public class ReleaseRepository : IReleaseRepository
{
    private readonly ApplicationDbContext _context;

    public async Task<List<Release>> GetAllAsync()
    {
        return await _context.Releases.ToListAsync();
    }
}
```

## 🎨 Naming Conventions

| Type | Convention | Example |
|------|-----------|---------|
| Interface | I{Name} | `IReleaseService` |
| Implementation | {Name} | `ReleaseService` |
| Entity | Singular noun | `Release` |
| DTO | {Entity}Dto | `ReleaseDto` |
| Create DTO | Create{Entity}Dto | `CreateReleaseDto` |
| Update DTO | Update{Entity}Dto | `UpdateReleaseDto` |
| Repository Interface | I{Entity}Repository | `IReleaseRepository` |
| Repository | {Entity}Repository | `ReleaseRepository` |

## 🔍 Troubleshooting

### Build Error: "Type not found"
**Solution**: Add missing using statements
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
```

### Build Error: "Missing interface implementation"
**Solution**: Implement all interface members
```csharp
public class ReleaseService : IReleaseService
{
    // Implement ALL methods from IReleaseService
}
```

### Runtime Error: "Service not registered"
**Solution**: Register in Program.cs
```csharp
builder.Services.AddScoped<IYourService, YourService>();
```

## 📚 Useful Commands

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

### Add Package
```bash
dotnet add package PackageName
```

### Create Test Project
```bash
dotnet new xunit -n ProjectName.Tests
```

### Add Project Reference
```bash
dotnet add reference ../OtherProject/OtherProject.csproj
```

## 🎓 Key Concepts

### DTO (Data Transfer Object)
- Used to transfer data between layers
- Prevents exposing domain entities
- Can have different properties than entities

### Repository Pattern
- Abstracts data access
- Provides collection-like interface
- Enables easy testing and swapping

### Dependency Inversion
- Depend on abstractions (interfaces)
- Not on concrete implementations
- Enables flexibility and testing

### Separation of Concerns
- Each layer has single responsibility
- Changes isolated to specific layer
- Easier to maintain and test

## 💡 Best Practices

1. **Always use interfaces** for services and repositories
2. **Use DTOs** between layers, never expose entities
3. **Keep entities simple** - just data and domain logic
4. **Business logic** goes in Application services
5. **Data access** goes in Infrastructure repositories
6. **UI logic** stays in Blazor pages/components
7. **Test each layer** independently
8. **Document** complex business rules

## 🔗 Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Microsoft Architecture Guides](https://docs.microsoft.com/en-us/dotnet/architecture/)
- See `CLEAN_ARCHITECTURE.md` for detailed documentation
