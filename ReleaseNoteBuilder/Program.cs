using Microsoft.EntityFrameworkCore;
using ReleaseNoteBuilder.Infrastructure.Persistence;
using ReleaseNoteBuilder.Infrastructure.Persistence.Repositories;
using ReleaseNoteBuilder.Infrastructure.ExternalServices;
using ReleaseNoteBuilder.Core.Interfaces;
using ReleaseNoteBuilder.Application.Interfaces;
using ReleaseNoteBuilder.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

// Infrastructure - Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ReleaseDb"));

// Infrastructure - Repositories
builder.Services.AddScoped<IReleaseRepository, ReleaseRepository>();
builder.Services.AddScoped<IBuildRepository, BuildRepository>();
builder.Services.AddScoped<IWorkItemRepository, WorkItemRepository>();

// Application - Services
builder.Services.AddScoped<IReleaseService, ReleaseService>();
builder.Services.AddScoped<IBuildService, BuildService>();
builder.Services.AddScoped<IWorkItemService, WorkItemService>();
builder.Services.AddScoped<IReleaseNoteGenerator, ReleaseNoteGenerator>();
builder.Services.AddScoped<IReleaseDocumentService, ReleaseDocumentService>();

// Infrastructure - External Services
builder.Services.AddScoped<AzureDevOpsService>();
builder.Services.AddScoped<ApprovalService>();

var app = builder.Build();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var releaseRepo = scope.ServiceProvider.GetRequiredService<IReleaseRepository>();
    var buildRepo = scope.ServiceProvider.GetRequiredService<IBuildRepository>();
    var workItemRepo = scope.ServiceProvider.GetRequiredService<IWorkItemRepository>();
    
    await releaseRepo.SeedDataAsync();
    await buildRepo.SeedDataAsync();
    await workItemRepo.SeedDataAsync();
}

// Middleware
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();