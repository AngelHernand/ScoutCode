namespace ScoutCode.Models;

/// <summary>
/// Define una clave de cifrado scout con su metadata.
/// </summary>
public class CipherDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CipherType Type { get; set; }
    public string Icon { get; set; } = "🔐";
    public bool IsAvailable { get; set; } = true;
}
