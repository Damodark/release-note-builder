using Microsoft.EntityFrameworkCore;
using ReleaseNoteBuilder.Data;
using ReleaseNoteBuilder.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ReleaseDb"));

builder.Services.AddScoped<AdoService>();
builder.Services.AddScoped<ReleaseService>();
builder.Services.AddScoped<ReleaseRepository>();
builder.Services.AddScoped<ApprovalService>();

var app = builder.Build();

// ✅ SEED DATA (PUT IT HERE ✅)
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<ReleaseRepository>();
    await repo.SeedData();
}

// ✅ MIDDLEWARE

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();