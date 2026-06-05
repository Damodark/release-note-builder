using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using ReleaseNoteBuilder.Application.DTOs;
using ReleaseNoteBuilder.Application.Services;
using ReleaseNoteBuilder.Core.Entities;
using ReleaseNoteBuilder.Core.Interfaces;
using Xunit;

namespace ReleaseNoteBuilder.Tests.Application.Services;

/// <summary>
/// Unit tests for ReleaseService
/// </summary>
public class ReleaseServiceTests
{
    private readonly Mock<IReleaseRepository> _mockRepository;
    private readonly ReleaseService _service;

    public ReleaseServiceTests()
    {
        _mockRepository = new Mock<IReleaseRepository>();
        _service = new ReleaseService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllReleasesAsync_ShouldReturnAllReleases()
    {
        // Arrange
        var releases = new List<Release>
        {
            new Release { Id = 1, BuildId = 100, Project = "ACG", Environment = "PROD", Branch = "main", Notes = "Notes 1" },
            new Release { Id = 2, BuildId = 101, Project = "ACG", Environment = "UAT", Branch = "develop", Notes = "Notes 2" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(releases);

        // Act
        var result = await _service.GetAllReleasesAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Id.Should().Be(1);
        result[0].BuildId.Should().Be(100);
        result[1].Id.Should().Be(2);
        result[1].BuildId.Should().Be(101);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllReleasesAsync_WhenNoReleases_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Release>());

        // Act
        var result = await _service.GetAllReleasesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReleaseByIdAsync_WhenReleaseExists_ShouldReturnRelease()
    {
        // Arrange
        var release = new Release
        {
            Id = 1,
            BuildId = 100,
            Project = "ACG",
            Environment = "PROD",
            Branch = "main",
            Notes = "Test notes"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(release);

        // Act
        var result = await _service.GetReleaseByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.BuildId.Should().Be(100);
        result.Project.Should().Be("ACG");
    }

    [Fact]
    public async Task GetReleaseByIdAsync_WhenReleaseDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Release?)null);

        // Act
        var result = await _service.GetReleaseByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateReleaseAsync_ShouldCreateAndReturnRelease()
    {
        // Arrange
        var createDto = new CreateReleaseDto
        {
            BuildId = 100,
            Project = "ACG",
            Environment = "PROD",
            Branch = "main",
            Notes = "Test notes"
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Release>()))
            .ReturnsAsync((Release r) => { r.Id = 1; return r; });

        // Act
        var result = await _service.CreateReleaseAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.BuildId.Should().Be(100);
        result.Project.Should().Be("ACG");
        result.Environment.Should().Be("PROD");
        result.Branch.Should().Be("main");
        result.Notes.Should().Be("Test notes");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Release>()), Times.Once);
    }

    [Fact]
    public async Task UpdateReleaseAsync_WhenReleaseExists_ShouldUpdateRelease()
    {
        // Arrange
        var existingRelease = new Release
        {
            Id = 1,
            BuildId = 100,
            Project = "OldProject",
            Environment = "DEV",
            Branch = "develop",
            Notes = "Old notes"
        };

        var updateDto = new UpdateReleaseDto
        {
            Id = 1,
            BuildId = 200,
            Project = "NewProject",
            Environment = "PROD",
            Branch = "main",
            Notes = "New notes"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingRelease);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Release>())).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateReleaseAsync(updateDto);

        // Assert
        existingRelease.BuildId.Should().Be(200);
        existingRelease.Project.Should().Be("NewProject");
        existingRelease.Environment.Should().Be("PROD");
        existingRelease.Branch.Should().Be("main");
        existingRelease.Notes.Should().Be("New notes");
        _mockRepository.Verify(r => r.UpdateAsync(existingRelease), Times.Once);
    }

    [Fact]
    public async Task UpdateReleaseAsync_WhenReleaseDoesNotExist_ShouldThrowException()
    {
        // Arrange
        var updateDto = new UpdateReleaseDto { Id = 999 };
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Release?)null);

        // Act
        Func<Task> act = async () => await _service.UpdateReleaseAsync(updateDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Release with ID 999 not found");
    }

    [Fact]
    public async Task DeleteReleaseAsync_ShouldCallRepositoryDelete()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteReleaseAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetReleasesByEnvironmentAsync_ShouldReturnFilteredReleases()
    {
        // Arrange
        var releases = new List<Release>
        {
            new Release { Id = 1, Environment = "PROD" },
            new Release { Id = 2, Environment = "UAT" },
            new Release { Id = 3, Environment = "PROD" },
            new Release { Id = 4, Environment = "DEV" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(releases);

        // Act
        var result = await _service.GetReleasesByEnvironmentAsync("PROD");

        // Assert
        result.Should().HaveCount(2);
        result.All(r => r.Environment == "PROD").Should().BeTrue();
    }

    [Theory]
    [InlineData("ACG", null, 3)] // Search by project
    [InlineData("", "PROD", 1)]  // Filter by environment
    [InlineData("ACG", "PROD", 1)] // Both search and filter
    public async Task SearchReleasesAsync_ShouldReturnFilteredResults(
        string searchTerm, string? environmentFilter, int expectedCount)
    {
        // Arrange
        var releases = new List<Release>
        {
            new Release { Id = 1, Project = "ACG", Environment = "PROD", Branch = "main" },
            new Release { Id = 2, Project = "ACG", Environment = "UAT", Branch = "develop" },
            new Release { Id = 3, Project = "ACG", Environment = "DEV", Branch = "feature" },
            new Release { Id = 4, Project = "XYZ", Environment = "PROD", Branch = "main" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(releases);

        // Act
        var result = await _service.SearchReleasesAsync(searchTerm, environmentFilter);

        // Assert
        result.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task SearchReleasesAsync_ShouldSearchInMultipleFields()
    {
        // Arrange
        var releases = new List<Release>
        {
            new Release { Id = 1, Project = "ACG", Branch = "develop", Notes = "Test" },
            new Release { Id = 2, Project = "XYZ", Branch = "main", Notes = "ACG mentioned here" },
            new Release { Id = 3, Project = "ABC", Branch = "ACG-branch", Notes = "Other" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(releases);

        // Act - Search for "ACG" which appears in project, notes, and branch
        var result = await _service.SearchReleasesAsync("ACG");

        // Assert
        result.Should().HaveCount(3); // All three contain "ACG" in some field
    }
}
