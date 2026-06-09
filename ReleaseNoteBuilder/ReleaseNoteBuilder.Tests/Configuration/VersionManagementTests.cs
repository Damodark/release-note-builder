// ReleaseNoteBuilder.Tests/Configuration/VersionManagementTests.cs
using System;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace ReleaseNoteBuilder.Tests.Configuration;

/// <summary>
/// Tests for Directory.Build.props version management (XXX.XXX.XXX format)
/// Verifies that versioning is correctly applied to assemblies
/// </summary>
public class VersionManagementTests
{
    #region Setup & Constants

    private const string ExpectedVersionFormat = @"^\d{3}\.\d{3}\.\d{3}$";
    private const string ExpectedAssemblyVersionFormat = @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$";

    private Assembly GetMainAssembly()
    {
        return typeof(Program).Assembly;
    }

    #endregion

    #region Version Format Tests

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void Assembly_ShouldHaveValidVersion()
    {
        // Arrange
        var assembly = GetMainAssembly();

        // Act
        var version = assembly.GetName().Version;

        // Assert
        version.Should().NotBeNull("Assembly should have a version");
        version.Should().BeOfType<Version>();
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void AssemblyVersion_ShouldFollowSemanticVersioning()
    {
        // Arrange
        var assembly = GetMainAssembly();
        var version = assembly.GetName().Version;

        // Act & Assert
        version!.Major.Should().BeGreaterThanOrEqualTo(0).And.BeLessThan(1000);
        version.Minor.Should().BeGreaterThanOrEqualTo(0).And.BeLessThan(1000);
        version.Build.Should().BeGreaterThanOrEqualTo(0).And.BeLessThan(1000);
        version.Revision.Should().Be(0, "Revision should be 0 for compatibility");
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void InformationalVersion_ShouldIncludeBuildMetadata()
    {
        // Arrange
        var assembly = GetMainAssembly();
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        // Act & Assert
        informationalVersion.Should().NotBeNullOrEmpty("Should have informational version");
        informationalVersion.Should().Contain("+build", "Should include build metadata");
    }

    #endregion

    #region Version Component Tests

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void Version_ShouldHaveThreeComponents()
    {
        // Arrange
        var assembly = GetMainAssembly();
        var version = assembly.GetName().Version;

        // Act
        var versionString = $"{version!.Major:D3}.{version.Minor:D3}.{version.Build:D3}";

        // Assert
        versionString.Should().Match(@"^\d{3}\.\d{3}\.\d{3}$", "Version should be XXX.XXX.XXX format");
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void MajorVersion_ShouldBeValidRange()
    {
        // Arrange
        var assembly = GetMainAssembly();
        var version = assembly.GetName().Version;

        // Act & Assert
        version!.Major.Should()
            .BeGreaterThanOrEqualTo(0, "Major version should be >= 0")
            .And.BeLessThan(1000, "Major version should be < 1000 (3 digits)");
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void MinorVersion_ShouldBeValidRange()
    {
        // Arrange
        var assembly = GetMainAssembly();
        var version = assembly.GetName().Version;

        // Act & Assert
        version!.Minor.Should()
            .BeGreaterThanOrEqualTo(0, "Minor version should be >= 0")
            .And.BeLessThan(1000, "Minor version should be < 1000 (3 digits)");
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void PatchVersion_ShouldBeValidRange()
    {
        // Arrange
        var assembly = GetMainAssembly();
        var version = assembly.GetName().Version;

        // Act & Assert
        version!.Build.Should()
            .BeGreaterThanOrEqualTo(0, "Patch version should be >= 0")
            .And.BeLessThan(1000, "Patch version should be < 1000 (3 digits)");
    }

    #endregion

    #region Version Consistency Tests

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void FileVersion_ShouldMatchAssemblyVersion()
    {
        // Arrange
        var assembly = GetMainAssembly();
        var assemblyVersion = assembly.GetName().Version;
        var fileVersionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();

        // Act
        var fileVersionString = fileVersionAttribute?.Version ?? "Not set";

        // Assert
        fileVersionString.Should().NotBeNullOrEmpty("File version should be set");
        fileVersionString.Should().StartWith($"{assemblyVersion!.Major}", "File version should start with major version");
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void InformationalVersion_ShouldStartWithAssemblyVersion()
    {
        // Arrange
        var assembly = GetMainAssembly();
        var assemblyVersion = assembly.GetName().Version;
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        // Act & Assert
        informationalVersion.Should().NotBeNullOrEmpty();
        informationalVersion.Should().StartWith($"{assemblyVersion!.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}",
            "Informational version should start with semantic version");
    }

    #endregion

    #region Version Increment Tests

    [Theory]
    [Trait("Category", "VersionManagement")]
    [InlineData(0, 0, 0)]      // Initial version
    [InlineData(1, 2, 0)]      // Common version
    [InlineData(1, 2, 5)]      // With patch increments
    [InlineData(2, 0, 0)]      // Major bump
    [InlineData(1, 10, 50)]    // Larger numbers
    [InlineData(10, 20, 100)]  // Double/triple digits
    [InlineData(999, 999, 999)] // Maximum values
    public void Version_ShouldAcceptValidVersionNumbers(int major, int minor, int patch)
    {
        // Arrange
        var version = new Version(major, minor, patch, 0);

        // Act & Assert
        version.Major.Should().Be(major);
        version.Minor.Should().Be(minor);
        version.Build.Should().Be(patch);
        version.Revision.Should().Be(0);
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void CurrentVersion_ShouldBeValidSemanticVersion()
    {
        // Arrange
        var assembly = GetMainAssembly();
        var version = assembly.GetName().Version;

        // Act
        var formattedVersion = $"{version!.Major:D3}.{version.Minor:D3}.{version.Build:D3}";

        // Assert - Verify current version in Directory.Build.props
        formattedVersion.Should().NotBeNullOrEmpty();
        formattedVersion.Should().Match(@"^\d{3}\.\d{3}\.\d{3}$");

        // Expected: 001.002.000 from Directory.Build.props
        version.Major.Should().BeGreaterThanOrEqualTo(1, "Major version should be at least 1");
        version.Minor.Should().BeGreaterThanOrEqualTo(2, "Minor version should be at least 2");
        version.Build.Should().BeGreaterThanOrEqualTo(0, "Patch version should be at least 0");
    }

    #endregion

    #region Version Scenario Tests

    [Fact]
    [Trait("Category", "VersionManagement")]
    [Trait("Scenario", "BugFix")]
    public void Scenario_BugFixVersionIncrement()
    {
        // Scenario: Bug fix release
        // Version: 001.002.000 → 001.002.001

        // Arrange
        var currentVersion = new Version(1, 2, 0, 0);  // 001.002.000
        var expectedPatchedVersion = new Version(1, 2, 1, 0);  // 001.002.001

        // Act
        var patchIncrement = new Version(
            currentVersion.Major,
            currentVersion.Minor,
            currentVersion.Build + 1,
            0
        );

        // Assert
        patchIncrement.Should().Be(expectedPatchedVersion);
        patchIncrement.Build.Should().Be(1, "Patch version should increment");
        patchIncrement.Major.Should().Be(1, "Major version should remain unchanged");
        patchIncrement.Minor.Should().Be(2, "Minor version should remain unchanged");
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    [Trait("Scenario", "NewFeature")]
    public void Scenario_NewFeatureVersionIncrement()
    {
        // Scenario: New feature release
        // Version: 001.002.005 → 001.003.000

        // Arrange
        var currentVersion = new Version(1, 2, 5, 0);  // 001.002.005
        var expectedFeatureVersion = new Version(1, 3, 0, 0);  // 001.003.000

        // Act
        var minorIncrement = new Version(
            currentVersion.Major,
            currentVersion.Minor + 1,
            0,  // Reset patch version
            0
        );

        // Assert
        minorIncrement.Should().Be(expectedFeatureVersion);
        minorIncrement.Minor.Should().Be(3, "Minor version should increment");
        minorIncrement.Build.Should().Be(0, "Patch version should reset");
        minorIncrement.Major.Should().Be(1, "Major version should remain unchanged");
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    [Trait("Scenario", "MajorRelease")]
    public void Scenario_MajorReleaseVersionIncrement()
    {
        // Scenario: Major breaking change
        // Version: 001.002.010 → 002.000.000

        // Arrange
        var currentVersion = new Version(1, 2, 10, 0);  // 001.002.010
        var expectedMajorVersion = new Version(2, 0, 0, 0);  // 002.000.000

        // Act
        var majorIncrement = new Version(
            currentVersion.Major + 1,
            0,  // Reset minor version
            0,  // Reset patch version
            0
        );

        // Assert
        majorIncrement.Should().Be(expectedMajorVersion);
        majorIncrement.Major.Should().Be(2, "Major version should increment");
        majorIncrement.Minor.Should().Be(0, "Minor version should reset");
        majorIncrement.Build.Should().Be(0, "Patch version should reset");
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    [Trait("Scenario", "MultipleBugFixes")]
    public void Scenario_MultipleBugFixesVersionProgression()
    {
        // Scenario: Multiple bug fixes over time
        // Version progression: 001.002.000 → 001.002.050

        // Arrange
        var versions = new[]
        {
            new Version(1, 2, 0, 0),   // 001.002.000
            new Version(1, 2, 1, 0),   // 001.002.001
            new Version(1, 2, 5, 0),   // 001.002.005
            new Version(1, 2, 10, 0),  // 001.002.010
            new Version(1, 2, 50, 0),  // 001.002.050
        };

        // Act & Assert
        for (int i = 0; i < versions.Length - 1; i++)
        {
            versions[i].Should().BeLessThan(versions[i + 1]);
            versions[i].Major.Should().Be(versions[i + 1].Major, "Major should stay same");
            versions[i].Minor.Should().Be(versions[i + 1].Minor, "Minor should stay same");
            versions[i].Build.Should().BeLessThan(versions[i + 1].Build, "Patch should increment");
        }

        // Verify final version
        versions[^1].Build.Should().Be(50);
        versions[^1].ToString().Should().Be("1.2.50.0");
    }

    #endregion

    #region Version Display Tests

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void VersionDisplay_ShouldFormatAsXXXDotXXXDotXXX()
    {
        // Arrange
        var assembly = GetMainAssembly();
        var version = assembly.GetName().Version;

        // Act
        var formattedVersion = $"{version!.Major:D3}.{version.Minor:D3}.{version.Build:D3}";

        // Assert
        formattedVersion.Should().Match(@"^\d{3}\.\d{3}\.\d{3}$", "Should be XXX.XXX.XXX format");
        formattedVersion.Should().HaveLength(11, "Format should be 11 characters (XXX.XXX.XXX)");
    }

    [Theory]
    [Trait("Category", "VersionManagement")]
    [InlineData(1, 2, 0, "001.002.000")]
    [InlineData(1, 2, 5, "001.002.005")]
    [InlineData(1, 10, 50, "001.010.050")]
    [InlineData(2, 0, 0, "002.000.000")]
    [InlineData(10, 20, 100, "010.020.100")]
    [InlineData(999, 999, 999, "999.999.999")]
    public void VersionDisplay_ShouldFormatCorrectly(int major, int minor, int patch, string expected)
    {
        // Arrange
        var version = new Version(major, minor, patch, 0);

        // Act
        var formatted = $"{version.Major:D3}.{version.Minor:D3}.{version.Build:D3}";

        // Assert
        formatted.Should().Be(expected);
    }

    #endregion

    #region Version Validation Tests

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void Version_ShouldNotAllowNegativeNumbers()
    {
        // Test that negative values throw ArgumentOutOfRangeException
        // Arrange & Act & Assert
        var ex1 = Xunit.Record.Exception(() => new Version(-1, 0, 0, 0));
        var ex2 = Xunit.Record.Exception(() => new Version(1, -1, 0, 0));
        var ex3 = Xunit.Record.Exception(() => new Version(1, 0, -1, 0));

        ex1.Should().BeOfType<ArgumentOutOfRangeException>("Negative major version should throw");
        ex2.Should().BeOfType<ArgumentOutOfRangeException>("Negative minor version should throw");
        ex3.Should().BeOfType<ArgumentOutOfRangeException>("Negative patch version should throw");
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void Version_ShouldAllowZeroValues()
    {
        // Arrange & Act
        var version = new Version(0, 0, 0, 0);

        // Assert
        version.Major.Should().Be(0);
        version.Minor.Should().Be(0);
        version.Build.Should().Be(0);
        version.Revision.Should().Be(0);
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void Version_ShouldSupportMaximumThreeDigitValues()
    {
        // Arrange & Act
        var version = new Version(999, 999, 999, 0);

        // Assert
        version.Major.Should().Be(999);
        version.Minor.Should().Be(999);
        version.Build.Should().Be(999);
    }

    #endregion

    #region Assembly Attributes Tests

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void Assembly_ShouldHaveFileVersionAttribute()
    {
        // Arrange
        var assembly = GetMainAssembly();

        // Act
        var fileVersionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();

        // Assert
        fileVersionAttribute.Should().NotBeNull("Assembly should have FileVersion attribute");
        fileVersionAttribute!.Version.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void Assembly_ShouldHaveInformationalVersionAttribute()
    {
        // Arrange
        var assembly = GetMainAssembly();

        // Act
        var informationalVersionAttribute = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        // Assert
        informationalVersionAttribute.Should().NotBeNull("Assembly should have InformationalVersion attribute");
        informationalVersionAttribute!.InformationalVersion.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Version Comparison Tests

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void Version_ShouldCompareCorrectly()
    {
        // Arrange
        var v1 = new Version(1, 2, 0, 0);
        var v2 = new Version(1, 2, 1, 0);
        var v3 = new Version(1, 3, 0, 0);

        // Act & Assert
        v1.Should().BeLessThan(v2, "1.2.0 should be less than 1.2.1");
        v2.Should().BeLessThan(v3, "1.2.1 should be less than 1.3.0");
        v1.Should().BeLessThan(v3, "1.2.0 should be less than 1.3.0");
    }

    [Fact]
    [Trait("Category", "VersionManagement")]
    public void Version_ShouldEqualWhenIdentical()
    {
        // Arrange
        var v1 = new Version(1, 2, 0, 0);
        var v2 = new Version(1, 2, 0, 0);

        // Act & Assert
        v1.Should().Be(v2);
        (v1 == v2).Should().BeTrue();
    }

    #endregion
}