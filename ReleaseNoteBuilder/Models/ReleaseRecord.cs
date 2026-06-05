using System;

namespace ReleaseNoteBuilder.Models;

public class ReleaseRecord
{
    public int Id { get; set; }
    public int BuildId { get; set; }
    public string Project { get; set; } = "";
    public string Environment { get; set; } = "";
    public string Branch { get; set; } = "";   // ✅ NEW
    public string Notes { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
