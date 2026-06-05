using Microsoft.EntityFrameworkCore;
using ReleaseNoteBuilder.Models;
namespace ReleaseNoteBuilder.Data;
public class AppDbContext:DbContext{
public AppDbContext(DbContextOptions<AppDbContext> opt):base(opt){}
public DbSet<ReleaseRecord> Releases => Set<ReleaseRecord>();}