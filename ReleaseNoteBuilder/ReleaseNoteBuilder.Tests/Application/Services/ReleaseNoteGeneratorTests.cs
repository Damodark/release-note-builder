using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using ReleaseNoteBuilder.Application.DTOs;
using ReleaseNoteBuilder.Application.Services;
using Xunit;

namespace ReleaseNoteBuilder.Tests.Application.Services;

/// <summary>
/// Unit tests for ReleaseNoteGenerator service
/// </summary>
public class ReleaseNoteGeneratorTests
{
    private readonly ReleaseNoteGenerator _generator;

    public ReleaseNoteGeneratorTests()
    {
        _generator = new ReleaseNoteGenerator();
    }

    [Fact]
    public void GenerateReleaseNote_ShouldGenerateCorrectFormat()
    {
        // Arrange
        var release = new ReleaseDto
        {
            Id = 1,
            BuildId = 100,
            Project = "ACG",
            Environment = "PROD",
            Branch = "main",
            Notes = "Bug fixes and improvements",
            CreatedAt = new DateTime(2026, 6, 5, 11, 23, 0)
        };

        // Act
        var result = _generator.GenerateReleaseNote(release);

        // Assert
        result.Should().Contain("Release PROD Build 100");
        result.Should().Contain("Project: ACG");
        result.Should().Contain("Branch: main");
        result.Should().Contain("Date: 2026-06-05 11:23");
        result.Should().Contain("Notes:");
        result.Should().Contain("Bug fixes and improvements");
    }

    [Fact]
    public void GenerateReleaseNote_ShouldHandleEmptyNotes()
    {
        // Arrange
        var release = new ReleaseDto
        {
            BuildId = 100,
            Environment = "UAT",
            Notes = string.Empty
        };

        // Act
        var result = _generator.GenerateReleaseNote(release);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Release UAT Build 100");
    }

    [Fact]
    public void GenerateReleaseNotes_ShouldFilterByEnvironment()
    {
        // Arrange
        var releases = new List<ReleaseDto>
        {
            new ReleaseDto { Id = 1, BuildId = 100, Environment = "PROD", Project = "ACG", Branch = "main", Notes = "Prod release" },
            new ReleaseDto { Id = 2, BuildId = 101, Environment = "UAT", Project = "ACG", Branch = "main", Notes = "UAT release" },
            new ReleaseDto { Id = 3, BuildId = 102, Environment = "PROD", Project = "ACG", Branch = "main", Notes = "Another prod" }
        };

        // Act
        var result = _generator.GenerateReleaseNotes(releases, "PROD");

        // Assert
        result.Should().Contain("Build 100");
        result.Should().Contain("Build 102");
        result.Should().NotContain("Build 101");
        result.Should().Contain("---"); // Separator between releases
    }

    [Fact]
    public void GenerateReleaseNotes_ShouldFilterByBuildId()
    {
        // Arrange
        var releases = new List<ReleaseDto>
        {
            new ReleaseDto { Id = 1, BuildId = 100, Environment = "PROD", Project = "ACG", Branch = "main", Notes = "Build 100" },
            new ReleaseDto { Id = 2, BuildId = 101, Environment = "PROD", Project = "ACG", Branch = "main", Notes = "Build 101" }
        };

        // Act
        var result = _generator.GenerateReleaseNotes(releases, "PROD", buildId: 100);

        // Assert
        result.Should().Contain("Build 100");
        result.Should().NotContain("Build 101");
    }

    [Fact]
    public void GenerateReleaseNotes_WithNoMatches_ShouldReturnEmptyString()
    {
        // Arrange
        var releases = new List<ReleaseDto>
        {
            new ReleaseDto { Id = 1, Environment = "UAT", Project = "ACG", Branch = "main" }
        };

        // Act
        var result = _generator.GenerateReleaseNotes(releases, "PROD");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExportToWordAsync_ShouldGenerateFormattedContent()
    {
        // Arrange
        var releases = new List<ReleaseDto>
        {
            new ReleaseDto
            {
                Id = 1,
                BuildId = 100,
                Project = "ACG",
                Environment = "PROD",
                Branch = "main",
                Notes = "Release notes",
                CreatedAt = new DateTime(2026, 6, 5, 11, 23, 0)
            },
            new ReleaseDto
            {
                Id = 2,
                BuildId = 101,
                Project = "ACG",
                Environment = "UAT",
                Branch = "develop",
                Notes = "UAT notes",
                CreatedAt = new DateTime(2026, 6, 5, 10, 0, 0)
            }
        };

        // Act
        var result = await _generator.ExportToWordAsync(releases);

        // Assert
        result.Should().Contain("Project: ACG");
        result.Should().Contain("Build: 100");
        result.Should().Contain("Build: 101");
        result.Should().Contain("Env: PROD");
        result.Should().Contain("Env: UAT");
        result.Should().Contain("Branch: main");
        result.Should().Contain("Branch: develop");
        result.Should().Contain("Date: 2026-06-05 11:23");
        result.Should().Contain("Date: 2026-06-05 10:00");
    }

    [Fact]
    public async Task ExportToWordAsync_WithEmptyList_ShouldReturnEmptyString()
    {
        // Arrange
        var releases = new List<ReleaseDto>();

        // Act
        var result = await _generator.ExportToWordAsync(releases);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExportToWordAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var releases = new List<ReleaseDto>
        {
            new ReleaseDto { BuildId = 1, Project = "Test", Environment = "DEV", Branch = "main", Notes = "Test" }
        };

        // Act
        Func<Task> act = async () => await _generator.ExportToWordAsync(releases);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData("PROD")]
    [InlineData("UAT")]
    [InlineData("TEST")]
    [InlineData("DEV")]
    public void GenerateReleaseNotes_ShouldBeCaseInsensitiveForEnvironment(string environment)
    {
        // Arrange
        var releases = new List<ReleaseDto>
        {
            new ReleaseDto { Environment = "prod", Project = "ACG", Branch = "main", Notes = "Test" }
        };

        // Act
        var result = _generator.GenerateReleaseNotes(releases, environment);

        // Assert
        if (environment.ToUpper() == "PROD")
        {
            result.Should().NotBeEmpty();
        }
        else
        {
            result.Should().BeEmpty();
        }
    }
}
