namespace ScoutCode.Models;

public class CipherDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CipherType Type { get; set; }
    public string Icon { get; set; } = "--";
    public bool IsAvailable { get; set; } = true;
    /// <summary>Accent color hex for the badge (cycles Blue → Green → Amber).</summary>
    public string AccentColorHex { get; set; } = "#34657f";
}
