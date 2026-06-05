# Branching Strategy & CI/CD Pipeline

This document outlines the branching strategy and CI/CD pipeline configuration for the ReleaseNoteBuilder project.

## Branch Structure

The project uses a multi-branch strategy to support different environments:

### 1. **main** (Primary Branch)
- Main development branch
- All features are merged here first
- Serves as the integration branch

### 2. **development**
- Development environment branch
- Automatically deploys to development environment
- Used for testing new features and bug fixes
- Triggers: Push to `development` branch

### 3. **staging**
- Staging environment branch
- Pre-production testing environment
- Used for QA and user acceptance testing
- Triggers: Push to `staging` branch

### 4. **hotfix**
- Hotfix environment branch
- Live production environment
- Only stable, tested code should be merged here
- Triggers: Push to `hotfix` branch
- Automatically creates release tags on deployment

## Environment Configurations

Each branch has its own configuration file:

| Branch | Config File | Environment | Build Config | Detailed Errors | Database |
|--------|-------------|-------------|--------------|-----------------|----------|
| development | `appsettings.Development.json` | Development | Debug | Yes | In-Memory |
| staging | `appsettings.Staging.json` | Staging | Release | No | Staging DB |
| hotfix | `appsettings.Production.json` | Production | Release | No | Production DB |

## CI/CD Pipelines

### GitHub Actions Workflows

Each branch has a dedicated GitHub Actions workflow:

#### Development Pipeline (`.github/workflows/development.yml`)
- **Trigger**: Push or PR to `development` branch
- **Environment**: Development
- **Build Configuration**: Debug
- **Steps**:
  1. Checkout code
  2. Setup .NET 8
  3. Restore dependencies
  4. Build (Debug mode)
  5. Run tests
  6. Publish artifacts
  7. Deploy to development environment

#### Staging Pipeline (`.github/workflows/staging.yml`)
- **Trigger**: Push or PR to `staging` branch
- **Environment**: Staging
- **Build Configuration**: Release
- **Steps**:
  1. Checkout code
  2. Setup .NET 8
  3. Restore dependencies
  4. Build (Release mode)
  5. Run tests
  6. Publish artifacts
  7. Deploy to staging environment (with environment protection)

#### Hotfix Pipeline (`.github/workflows/hotfix.yml`)
- **Trigger**: Push or PR to `hotfix` branch
- **Environment**: Production
- **Build Configuration**: Release
- **Steps**:
  1. Checkout code
  2. Setup .NET 8
  3. Restore dependencies
  4. Build (Release mode)
  5. Run tests
  6. Publish artifacts
  7. Deploy to hotfix environment (with environment protection)
  8. Create release tag with timestamp

## Workflow

### Feature Development
```
1. Create feature branch from main
2. Develop and test locally
3. Create PR to main
4. Merge to main after review
5. Merge main to development for deployment
```

### Deployment to Staging
```
1. Ensure main is stable
2. Merge main to staging
3. Automatic CI/CD pipeline triggers
4. QA testing in staging environment
```

### Deployment to Hotfix
```
1. Ensure staging is stable and tested
2. Merge staging to hotfix
3. Automatic CI/CD pipeline triggers
4. Hotfix deployment with release tag
```

## Environment Variables & Secrets

Each environment requires the following secrets (configure in GitHub repository settings):

### Common Secrets
- Azure DevOps PAT (if using Azure DevOps integration)
- Database connection strings (for staging/production)

### Per-Environment Secrets
Configure these in GitHub → Settings → Environments:
- `AZURE_WEBAPP_PUBLISH_PROFILE` (if deploying to Azure App Service)
- `DEPLOYMENT_URL`
- Any environment-specific API keys or tokens

## Branch Protection Rules

Recommended branch protection rules:

### main
- Require pull request reviews before merging
- Require status checks to pass
- Require branches to be up to date before merging

### staging
- Require pull request reviews before merging
- Require status checks to pass

### hotfix
- Require pull request reviews before merging (at least 2 reviewers)
- Require status checks to pass
- Require signed commits (optional)
- Include administrators in restrictions

## Notes

- All workflows use .NET 8.0.x
- Tests are run on every build
- Artifacts are preserved for 90 days (default)
- Hotfix deployments create automatic release tags with format: `vYYYY.MM.DD.HHMM`
- Remember to update deployment steps in workflow files with your actual deployment target (Azure App Service, IIS, Docker, etc.)

## Getting Started

1. Clone the repository
2. All branches are now available on GitHub:
   - `development`
   - `staging`
   - `hotfix`
3. Configure GitHub secrets for your deployment targets
4. Update the deployment steps in workflow files with your specific deployment commands
5. Push to respective branches to trigger pipelines

## Configuration Updates

To update environment-specific settings, modify the appropriate `appsettings.{Environment}.json` file and push to the corresponding branch.
