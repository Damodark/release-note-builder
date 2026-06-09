using System;
using FluentAssertions;
using ReleaseNoteBuilder.Core.Entities;
using Xunit;

namespace ReleaseNoteBuilder.Tests.Core.Entities;

/// <summary>
/// Unit tests for Release domain entity
/// </summary>
public class ReleaseTests
{
    [Fact]
    public void Release_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var release = new Release();

        // Assert
        release.Id.Should().Be(0);
        release.BuildId.Should().Be(0);
        release.Project.Should().BeEmpty();
        release.Environment.Should().BeEmpty();
        release.Branch.Should().BeEmpty();
        release.Notes.Should().BeEmpty();
        release.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Release_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var release = new Release
        {
            Id = 1,
            BuildId = 100,
            Project = "ACG",
            Environment = "PROD",
            Branch = "main",
            Notes = "Test release notes"
        };

        // Assert
        release.Id.Should().Be(1);
        release.BuildId.Should().Be(100);
        release.Project.Should().Be("ACG");
        release.Environment.Should().Be("PROD");
        release.Branch.Should().Be("main");
        release.Notes.Should().Be("Test release notes");
    }

    [Theory]
    [InlineData("PROD", true)]
    [InlineData("prod", true)]
    [InlineData("Prod", true)]
    [InlineData("UAT", false)]
    [InlineData("TEST", false)]
    [InlineData("DEV", false)]
    [InlineData("", false)]
    public void IsProduction_ShouldReturnCorrectValue(string environment, bool expectedResult)
    {
        // Arrange
        var release = new Release { Environment = environment };

        // Act
        var result = release.IsProduction;

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateAllProperties()
    {
        // Arrange
        var release = new Release
        {
            Id = 1,
            BuildId = 100,
            Project = "OldProject",
            Environment = "DEV",
            Branch = "develop",
            Notes = "Old notes"
        };

        // Act
        release.UpdateDetails(
            project: "NewProject",
            buildId: 200,
            environment: "PROD",
            branch: "main",
            notes: "New release notes"
        );

        // Assert
        release.Id.Should().Be(1); // ID should not change
        release.BuildId.Should().Be(200);
        release.Project.Should().Be("NewProject");
        release.Environment.Should().Be("PROD");
        release.Branch.Should().Be("main");
        release.Notes.Should().Be("New release notes");
    }

    [Fact]
    public void UpdateDetails_ShouldNotChangeId()
    {
        // Arrange
        var release = new Release { Id = 5 };
        var originalId = release.Id;

        // Act
        release.UpdateDetails("Project", 100, "PROD", "main", "Notes");

        // Assert
        release.Id.Should().Be(originalId);
    }

    [Fact]
    public void UpdateDetails_ShouldNotChangeCreatedAt()
    {
        // Arrange
        var release = new Release();
        var originalCreatedAt = release.CreatedAt;

        // Act
        release.UpdateDetails("Project", 100, "PROD", "main", "Notes");

        // Assert
        release.CreatedAt.Should().Be(originalCreatedAt);
    }
}
