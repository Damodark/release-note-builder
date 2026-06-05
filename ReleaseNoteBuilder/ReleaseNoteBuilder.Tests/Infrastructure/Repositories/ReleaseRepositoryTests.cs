using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ReleaseNoteBuilder.Core.Entities;
using ReleaseNoteBuilder.Infrastructure.Persistence;
using ReleaseNoteBuilder.Infrastructure.Persistence.Repositories;
using Xunit;

namespace ReleaseNoteBuilder.Tests.Infrastructure.Repositories;

/// <summary>
/// Integration tests for ReleaseRepository using InMemory database
/// </summary>
public class ReleaseRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ReleaseRepository _repository;

    public ReleaseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ReleaseRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllReleases_OrderedByCreatedAtDescending()
    {
        // Arrange
        var release1 = new Release { BuildId = 1, Project = "ACG", Environment = "PROD", Branch = "main", CreatedAt = DateTime.UtcNow.AddHours(-2) };
        var release2 = new Release { BuildId = 2, Project = "ACG", Environment = "UAT", Branch = "develop", CreatedAt = DateTime.UtcNow.AddHours(-1) };
        var release3 = new Release { BuildId = 3, Project = "ACG", Environment = "DEV", Branch = "feature", CreatedAt = DateTime.UtcNow };

        _context.Releases.AddRange(release1, release2, release3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result[0].BuildId.Should().Be(3); // Most recent first
        result[1].BuildId.Should().Be(2);
        result[2].BuildId.Should().Be(1);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoReleases_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenReleaseExists_ShouldReturnRelease()
    {
        // Arrange
        var release = new Release { BuildId = 100, Project = "ACG", Environment = "PROD", Branch = "main" };
        _context.Releases.Add(release);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(release.Id);

        // Assert
        result.Should().NotBeNull();
        result!.BuildId.Should().Be(100);
        result.Project.Should().Be("ACG");
    }

    [Fact]
    public async Task GetByIdAsync_WhenReleaseDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldAddReleaseToDatabase()
    {
        // Arrange
        var release = new Release
        {
            BuildId = 100,
            Project = "ACG",
            Environment = "PROD",
            Branch = "main",
            Notes = "Test notes"
        };

        // Act
        var result = await _repository.AddAsync(release);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        var savedRelease = await _context.Releases.FindAsync(result.Id);
        savedRelease.Should().NotBeNull();
        savedRelease!.BuildId.Should().Be(100);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingRelease()
    {
        // Arrange
        var release = new Release { BuildId = 100, Project = "OldProject", Environment = "DEV", Branch = "develop" };
        _context.Releases.Add(release);
        await _context.SaveChangesAsync();

        // Detach to simulate a new context
        _context.Entry(release).State = EntityState.Detached;

        // Act
        release.UpdateDetails("NewProject", 200, "PROD", "main", "Updated notes");
        await _repository.UpdateAsync(release);

        // Assert
        var updated = await _context.Releases.FindAsync(release.Id);
        updated.Should().NotBeNull();
        updated!.Project.Should().Be("NewProject");
        updated.BuildId.Should().Be(200);
        updated.Environment.Should().Be("PROD");
        updated.Branch.Should().Be("main");
    }

    [Fact]
    public async Task DeleteAsync_WhenReleaseExists_ShouldRemoveFromDatabase()
    {
        // Arrange
        var release = new Release { BuildId = 100, Project = "ACG", Environment = "PROD", Branch = "main" };
        _context.Releases.Add(release);
        await _context.SaveChangesAsync();
        var releaseId = release.Id;

        // Act
        await _repository.DeleteAsync(releaseId);

        // Assert
        var deleted = await _context.Releases.FindAsync(releaseId);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenReleaseDoesNotExist_ShouldNotThrow()
    {
        // Act
        Func<Task> act = async () => await _repository.DeleteAsync(999);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SeedDataAsync_WhenDatabaseEmpty_ShouldSeed30Releases()
    {
        // Act
        await _repository.SeedDataAsync();

        // Assert
        var releases = await _context.Releases.ToListAsync();
        releases.Should().HaveCount(30);
        releases.Should().Contain(r => r.Environment == "PROD");
        releases.Should().Contain(r => r.Environment == "UAT");
        releases.Should().Contain(r => r.Environment == "TEST");
        releases.Should().Contain(r => r.Environment == "DEV");
    }

    [Fact]
    public async Task SeedDataAsync_WhenDatabaseHasData_ShouldNotSeedAgain()
    {
        // Arrange
        var existingRelease = new Release { BuildId = 999, Project = "Existing", Environment = "PROD", Branch = "main" };
        _context.Releases.Add(existingRelease);
        await _context.SaveChangesAsync();

        // Act
        await _repository.SeedDataAsync();

        // Assert
        var releases = await _context.Releases.ToListAsync();
        releases.Should().HaveCount(1); // Only the existing one
        releases[0].BuildId.Should().Be(999);
    }

    [Fact]
    public async Task SeedDataAsync_ShouldCreateReleasesWithCorrectPattern()
    {
        // Act
        await _repository.SeedDataAsync();

        // Assert
        var releases = await _context.Releases.ToListAsync();

        // Check environment distribution
        releases.Count(r => r.Environment == "PROD").Should().Be(7); // Builds divisible by 4 (4,8,12...28)
        releases.Count(r => r.Environment == "UAT").Should().Be(8);  // Remainder 1
        releases.Count(r => r.Environment == "TEST").Should().Be(7); // Remainder 2
        releases.Count(r => r.Environment == "DEV").Should().Be(8);  // Remainder 3

        // All should be ACG project
        releases.Should().AllSatisfy(r => r.Project.Should().Be("ACG"));

        // All should be develop branch
        releases.Should().AllSatisfy(r => r.Branch.Should().Be("develop"));
    }

    [Fact]
    public async Task AddAsync_ShouldSetCreatedAtToCurrentTime()
    {
        // Arrange
        var release = new Release { BuildId = 100, Project = "ACG", Environment = "PROD", Branch = "main" };
        var beforeAdd = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var result = await _repository.AddAsync(release);
        var afterAdd = DateTime.UtcNow.AddSeconds(1);

        // Assert
        result.CreatedAt.Should().BeAfter(beforeAdd);
        result.CreatedAt.Should().BeBefore(afterAdd);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
