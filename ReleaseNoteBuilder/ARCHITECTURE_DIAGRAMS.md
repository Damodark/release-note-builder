# Clean Architecture Diagram

## Layer Architecture
```
┌─────────────────────────────────────────────────────────────┐
│                      PRESENTATION LAYER                      │
│                     (Blazor Server Pages)                    │
│                                                               │
│  Pages/ListReleases.razor    Pages/Release.razor            │
│  Pages/ReleaseDetails.razor  Components/...                 │
│                                                               │
│  Dependencies: Application layer interfaces only             │
└───────────────────────────┬─────────────────────────────────┘
                            │
                            │ Injects IReleaseService
                            │ Injects IReleaseNoteGenerator
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                     APPLICATION LAYER                        │
│                  (Business Logic / Use Cases)                │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Interfaces/                                          │   │
│  │  - IReleaseService                                  │   │
│  │  - IReleaseNoteGenerator                            │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Services/                                            │   │
│  │  - ReleaseService (implements IReleaseService)      │   │
│  │  - ReleaseNoteGenerator (implements IRelease...)    │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ DTOs/                                                │   │
│  │  - ReleaseDto                                        │   │
│  │  - CreateReleaseDto                                  │   │
│  │  - UpdateReleaseDto                                  │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                               │
│  Dependencies: Core layer only                               │
└───────────────────────────┬─────────────────────────────────┘
                            │
                            │ Uses IReleaseRepository
                            │ Uses Release entity
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                        CORE LAYER                            │
│                   (Domain / Enterprise)                      │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Entities/                                            │   │
│  │  - Release                                           │   │
│  │    + IsProduction()                                  │   │
│  │    + UpdateDetails()                                 │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Interfaces/                                          │   │
│  │  - IReleaseRepository                                │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                               │
│  Dependencies: NONE (Pure domain)                            │
└───────────────────────────▲─────────────────────────────────┘
                            │
                            │ Implements IReleaseRepository
                            │ Uses Release entity
                            │
┌─────────────────────────────────────────────────────────────┐
│                   INFRASTRUCTURE LAYER                       │
│              (External Concerns / Framework)                 │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ Persistence/                                         │   │
│  │  - ApplicationDbContext (EF Core)                   │   │
│  │  - Repositories/                                     │   │
│  │    + ReleaseRepository (implements IRelease...)     │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                               │
│  ┌─────────────────────────────────────────────────────┐   │
│  │ ExternalServices/                                    │   │
│  │  - AzureDevOpsService                                │   │
│  │  - ApprovalService                                   │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                               │
│  Dependencies: Core, Application                             │
└─────────────────────────────────────────────────────────────┘
```

## Dependency Flow
```
┌──────────────┐
│ Presentation │ ──depends on──┐
└──────────────┘                │
                                ▼
                        ┌──────────────┐
                        │ Application  │
                        └──────┬───────┘
                               │
                               │ depends on
                               ▼
                        ┌──────────────┐
                        │     Core     │ ◄──── implements
                        └──────────────┘        │
                                                │
                        ┌──────────────────────┐│
                        │  Infrastructure      ││
                        └──────────────────────┘
```

## Data Flow Example: Get All Releases

```
User clicks "View Releases"
        │
        ▼
┌─────────────────────────────────────┐
│ ListReleases.razor                  │
│ @inject IReleaseService             │
└─────────────────┬───────────────────┘
                  │
                  │ await GetAllReleasesAsync()
                  ▼
┌─────────────────────────────────────┐
│ ReleaseService                      │
│ (Application Layer)                 │
└─────────────────┬───────────────────┘
                  │
                  │ await _repository.GetAllAsync()
                  ▼
┌─────────────────────────────────────┐
│ ReleaseRepository                   │
│ (Infrastructure Layer)              │
└─────────────────┬───────────────────┘
                  │
                  │ _context.Releases.ToListAsync()
                  ▼
┌─────────────────────────────────────┐
│ ApplicationDbContext                │
│ (EF Core)                           │
└─────────────────┬───────────────────┘
                  │
                  │ SQL Query
                  ▼
        ┌─────────────────┐
        │    Database     │
        └─────────────────┘

Data flows back up:
Database → DbContext → Repository → Service → Page
  Entity   → Entity   → Entity    → DTO     → Display
```

## Create Release Flow

```
User submits form
        │
        ▼
┌─────────────────────────────────────┐
│ Release.razor                       │
│ Creates CreateReleaseDto            │
└─────────────────┬───────────────────┘
                  │
                  │ await CreateReleaseAsync(dto)
                  ▼
┌─────────────────────────────────────┐
│ ReleaseService                      │
│ - Converts DTO to Entity            │
│ - Validates business rules          │
└─────────────────┬───────────────────┘
                  │
                  │ await _repository.AddAsync(entity)
                  ▼
┌─────────────────────────────────────┐
│ ReleaseRepository                   │
│ - Adds to DbContext                 │
│ - Saves changes                     │
└─────────────────┬───────────────────┘
                  │
                  │ INSERT INTO Releases
                  ▼
        ┌─────────────────┐
        │    Database     │
        └─────────────────┘

Result flows back:
Database → Repository → Service → Page
  Entity  → Entity    → DTO     → Display
```

## Folder Structure Tree

```
ReleaseNoteBuilder/
│
├── 📁 Core/                        [Domain Layer - No Dependencies]
│   ├── 📁 Entities/
│   │   └── 📄 Release.cs          ← Domain entity with business logic
│   └── 📁 Interfaces/
│       └── 📄 IReleaseRepository.cs  ← Repository contract
│
├── 📁 Application/                 [Application Layer - Depends on Core]
│   ├── 📁 DTOs/
│   │   ├── 📄 ReleaseDto.cs       ← Data transfer objects
│   │   ├── 📄 CreateReleaseDto.cs
│   │   └── 📄 UpdateReleaseDto.cs
│   ├── 📁 Interfaces/
│   │   ├── 📄 IReleaseService.cs  ← Service contracts
│   │   └── 📄 IReleaseNoteGenerator.cs
│   └── 📁 Services/
│       ├── 📄 ReleaseService.cs   ← Business logic implementation
│       └── 📄 ReleaseNoteGenerator.cs
│
├── 📁 Infrastructure/              [Infrastructure - Depends on Core & Application]
│   ├── 📁 Persistence/
│   │   ├── 📄 ApplicationDbContext.cs  ← EF Core DbContext
│   │   └── 📁 Repositories/
│   │       └── 📄 ReleaseRepository.cs  ← Data access implementation
│   └── 📁 ExternalServices/
│       ├── 📄 AzureDevOpsService.cs    ← External API integration
│       └── 📄 ApprovalService.cs       ← External service
│
├── 📁 Pages/                       [Presentation - Depends on Application]
│   ├── 📄 ListReleases.razor      ← UI components
│   ├── 📄 Release.razor
│   └── 📄 ReleaseDetails.razor
│
└── 📄 Program.cs                   ← Dependency injection setup
```

## Interface Dependencies

```
IReleaseService (Application)
    ↓ uses
IReleaseRepository (Core)
    ↑ implemented by
ReleaseRepository (Infrastructure)
    ↓ uses
ApplicationDbContext (Infrastructure)
```

## Object Mapping Flow

```
┌──────────────┐
│   Release    │  ← Domain Entity (Core)
│  (Entity)    │
└──────┬───────┘
       │
       │ Repository returns
       │
       ▼
┌──────────────┐
│   Release    │  ← Entity in Application
│  (Entity)    │
└──────┬───────┘
       │
       │ Service maps to
       │
       ▼
┌──────────────┐
│  ReleaseDto  │  ← DTO for UI
│    (DTO)     │
└──────────────┘
```

## Typical Request Lifecycle

```
1. User Action (UI Event)
   ↓
2. Page Component (Presentation)
   ↓
3. Service Interface Call (Application)
   ↓
4. Service Implementation (Application)
   ↓
5. Repository Interface Call (Core)
   ↓
6. Repository Implementation (Infrastructure)
   ↓
7. DbContext (Infrastructure)
   ↓
8. Database
   ↓
9. Return Entity
   ↓
10. Map to DTO
   ↓
11. Return to UI
   ↓
12. Display Result
```

## Testing Structure

```
Unit Tests
├── Core.Tests
│   └── Entities/
│       └── ReleaseTests.cs          [Pure domain logic]
│
├── Application.Tests
│   └── Services/
│       ├── ReleaseServiceTests.cs   [Mock repository]
│       └── ReleaseNoteGeneratorTests.cs
│
└── Infrastructure.Tests
    └── Repositories/
        └── ReleaseRepositoryTests.cs [Integration with DB]

Integration Tests
└── E2E.Tests
    └── Pages/
        └── ListReleasesTests.cs      [Full stack test]
```

## Benefits Visualization

```
┌─────────────────────────────────────────────────────────┐
│                   CLEAN ARCHITECTURE                     │
├─────────────────────────────────────────────────────────┤
│                                                           │
│  ✅ Testability         Each layer tested independently │
│  ✅ Maintainability     Clear boundaries and contracts  │
│  ✅ Flexibility         Swap implementations easily     │
│  ✅ Scalability         Add features without rewrites   │
│  ✅ Independence        Framework/DB/UI agnostic        │
│  ✅ Clarity             Easy to navigate and understand │
│                                                           │
└─────────────────────────────────────────────────────────┘
```
