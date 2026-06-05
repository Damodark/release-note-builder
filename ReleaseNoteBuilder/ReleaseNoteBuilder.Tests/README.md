# ReleaseNoteBuilder.Tests

Comprehensive test suite for the ReleaseNoteBuilder application using xUnit, Moq, and FluentAssertions.

## Test Structure

```
ReleaseNoteBuilder.Tests/
├── Core/
│   └── Entities/
│       └── ReleaseTests.cs                    # Domain entity unit tests
├── Application/
│   └── Services/
│       ├── ReleaseServiceTests.cs            # Business logic unit tests
│       └── ReleaseNoteGeneratorTests.cs      # Note generation tests
└── Infrastructure/
    └── Repositories/
        └── ReleaseRepositoryTests.cs         # Repository integration tests
```

## Test Categories

### Unit Tests
- **Core/Entities/ReleaseTests.cs**: Tests domain entity behavior and business methods
- **Application/Services/ReleaseServiceTests.cs**: Tests business logic with mocked dependencies
- **Application/Services/ReleaseNoteGeneratorTests.cs**: Tests release note generation logic

### Integration Tests
- **Infrastructure/Repositories/ReleaseRepositoryTests.cs**: Tests data access with InMemory database

## Dependencies

- **xUnit**: Testing framework
- **Moq**: Mocking library for creating test doubles
- **FluentAssertions**: Fluent assertion library for readable test assertions
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database for integration tests

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~ReleaseServiceTests"
```

### Run Tests by Name Pattern
```bash
dotnet test --filter "Name~GetAllReleasesAsync"
```

### Run with Detailed Output
```bash
dotnet test --verbosity detailed
```

### Run with Code Coverage
```bash
dotnet test /p:CollectCoverage=true
```

## Test Coverage

### Core Layer
✅ **Release Entity** (ReleaseTests.cs)
- Default initialization
- Property setting
- `IsProduction()` method with various inputs
- `UpdateDetails()` method
- Immutability of Id and CreatedAt

### Application Layer
✅ **ReleaseService** (ReleaseServiceTests.cs)
- Get all releases
- Get release by ID
- Create release
- Update release
- Delete release
- Get releases by environment
- Search releases with filters

✅ **ReleaseNoteGenerator** (ReleaseNoteGeneratorTests.cs)
- Generate single release note
- Generate multiple release notes
- Filter by environment and build ID
- Export to Word format
- Handle empty collections

### Infrastructure Layer
✅ **ReleaseRepository** (ReleaseRepositoryTests.cs)
- CRUD operations
- Ordering by CreatedAt
- Seed data functionality
- Error handling

## Test Examples

### Unit Test with Moq
```csharp
[Fact]
public async Task GetAllReleasesAsync_ShouldReturnAllReleases()
{
    // Arrange
    var mockRepository = new Mock<IReleaseRepository>();
    mockRepository.Setup(r => r.GetAllAsync())
        .ReturnsAsync(new List<Release> { /* test data */ });

    var service = new ReleaseService(mockRepository.Object);

    // Act
    var result = await service.GetAllReleasesAsync();

    // Assert
    result.Should().HaveCount(2);
    mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
}
```

### Integration Test with InMemory Database
```csharp
[Fact]
public async Task AddAsync_ShouldAddReleaseToDatabase()
{
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    using var context = new ApplicationDbContext(options);
    var repository = new ReleaseRepository(context);
    var release = new Release { /* properties */ };

    // Act
    var result = await repository.AddAsync(release);

    // Assert
    result.Id.Should().BeGreaterThan(0);
}
```

### Theory Test with InlineData
```csharp
[Theory]
[InlineData("PROD", true)]
[InlineData("UAT", false)]
[InlineData("TEST", false)]
public void IsProduction_ShouldReturnCorrectValue(string env, bool expected)
{
    var release = new Release { Environment = env };
    release.IsProduction().Should().Be(expected);
}
```

## Writing New Tests

### 1. Create Test Class
```csharp
public class YourServiceTests
{
    private readonly Mock<IDependency> _mockDependency;
    private readonly YourService _service;

    public YourServiceTests()
    {
        _mockDependency = new Mock<IDependency>();
        _service = new YourService(_mockDependency.Object);
    }

    [Fact]
    public async Task YourMethod_ShouldDoSomething()
    {
        // Arrange

        // Act

        // Assert
    }
}
```

### 2. Follow AAA Pattern
- **Arrange**: Set up test data and mocks
- **Act**: Execute the method under test
- **Assert**: Verify the expected outcome

### 3. Use Descriptive Test Names
Follow the pattern: `MethodName_Scenario_ExpectedResult`
- `GetAllReleasesAsync_WhenNoReleases_ShouldReturnEmptyList`
- `CreateReleaseAsync_WithValidData_ShouldCreateRelease`
- `UpdateReleaseAsync_WhenReleaseDoesNotExist_ShouldThrowException`

## Best Practices

1. **One Assert Per Test** (when possible)
2. **Use Theory for Multiple Similar Tests**
3. **Mock External Dependencies**
4. **Use InMemory Database for Integration Tests**
5. **Clean Up Resources** (IDisposable for database tests)
6. **Test Both Happy Path and Error Cases**
7. **Use FluentAssertions** for readable assertions

## Continuous Integration

These tests are designed to run in CI/CD pipelines:

```yaml
# GitHub Actions example
- name: Run tests
  run: dotnet test --verbosity normal --collect:"XPlat Code Coverage"

- name: Upload coverage
  uses: codecov/codecov-action@v2
```

## Troubleshooting

### Test Discovery Issues
If tests aren't discovered:
1. Build the solution: `dotnet build`
2. Rebuild test project: `dotnet build ReleaseNoteBuilder.Tests`

### InMemory Database Issues
- Always use unique database names: `Guid.NewGuid().ToString()`
- Dispose context properly: implement `IDisposable`

### Async Test Issues
- Always use `async Task` for async tests
- Use `await` for async operations
- Use `Func<Task>` for testing exceptions

## Next Steps

1. **Add More Test Coverage**
   - Edge cases
   - Error scenarios
   - Performance tests

2. **Add Integration Tests**
   - Full stack tests
   - API endpoint tests (if added)

3. **Add Code Coverage Reports**
   ```bash
   dotnet add package coverlet.collector
   ```

4. **Set Up CI/CD**
   - Automated test runs
   - Coverage reporting
   - Test result publishing

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [FluentAssertions Documentation](https://fluentassertions.com/introduction)
- [EF Core Testing](https://docs.microsoft.com/en-us/ef/core/testing/)
