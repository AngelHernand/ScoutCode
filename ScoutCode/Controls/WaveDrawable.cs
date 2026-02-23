namespace ScoutCode.Controls;

/// <summary>
/// Draws a wave transition from HeaderDark to Background for the detail page header.
/// </summary>
public class WaveDrawable : IDrawable
{
    public Color TopColor { get; set; } = Color.FromArgb("#2c3e50");
    public Color BottomColor { get; set; } = Color.FromArgb("#f8f9fb");

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float w = dirtyRect.Width;
        float h = dirtyRect.Height;

        // Fill top color
        canvas.FillColor = TopColor;
        canvas.FillRectangle(0, 0, w, h * 0.5f);

        // Draw wave curve
        var wavePath = new PathF();
        wavePath.MoveTo(0, h * 0.3f);
        wavePath.CurveTo(w * 0.25f, h * 0.1f, w * 0.75f, h * 0.7f, w, h * 0.4f);
        wavePath.LineTo(w, h);
        wavePath.LineTo(0, h);
        wavePath.Close();

        canvas.FillColor = BottomColor;
        canvas.FillPath(wavePath);
    }
}
