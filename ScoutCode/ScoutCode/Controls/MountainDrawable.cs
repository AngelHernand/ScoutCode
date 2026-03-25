namespace ScoutCode.Controls;

/// <summary>
/// Draws the mountain header with sky gradient, three mountain silhouettes
/// (Blue back-left, Green middle, Amber front-right), and subtle fog.
/// Matches the SVG reference: viewBox="0 0 400 220".
/// </summary>
public class MountainDrawable : IDrawable
{
    public static MountainDrawable Instance { get; } = new();

    // Design-space constants (matching SVG viewBox 0 0 400 220)
    private const float DesignW = 400f;
    private const float DesignH = 220f;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        float w = dirtyRect.Width;
        float h = dirtyRect.Height;
        if (w <= 0 || h <= 0) return;

        float sx = w / DesignW;
        float sy = h / DesignH;

        // ── Sky background ──
        // Draw a solid-to-gradient sky. Because LinearGradientPaint can be
        // unreliable on some renderers, paint it as two halves for safety.
        canvas.FillColor = Color.FromArgb("#1a2a3a");
        canvas.FillRectangle(0, 0, w, h * 0.5f);
        canvas.FillColor = Color.FromArgb("#2c3e50");
        canvas.FillRectangle(0, h * 0.5f, w, h * 0.5f);
        // Blend middle band
        canvas.FillColor = Color.FromArgb("#233644");
        canvas.FillRectangle(0, h * 0.35f, w, h * 0.3f);

        // ── Mountain 1 — Blue/teal (back left, tallest) ──
        // Light face: full triangle
        FillTriangle(canvas, Color.FromArgb("#34657f"),
            -70 * sx, 220 * sy,
             10 * sx,  40 * sy,
            130 * sx, 220 * sy);
        // Shadow face (right side)
        FillTriangle(canvas, Color.FromArgb("#2b5468"), 0.7f,
             10 * sx,  40 * sy,
            130 * sx, 220 * sy,
             10 * sx, 220 * sy);

        // ── Mountain 2 — Green (middle) ──
        FillTriangle(canvas, Color.FromArgb("#4a7a4e"),
            -20 * sx, 220 * sy,
            110 * sx,  55 * sy,
            240 * sx, 220 * sy);
        FillTriangle(canvas, Color.FromArgb("#3d6640"), 0.7f,
            110 * sx,  55 * sy,
            240 * sx, 220 * sy,
            110 * sx, 220 * sy);

        // ── Mountain 3 — Orange/amber (front, overlapping) ──
        FillTriangle(canvas, Color.FromArgb("#d4943c"),
             50 * sx, 220 * sy,
            180 * sx,  80 * sy,
            300 * sx, 220 * sy);
        FillTriangle(canvas, Color.FromArgb("#c4842e"), 0.7f,
            180 * sx,  80 * sy,
            300 * sx, 220 * sy,
            180 * sx, 220 * sy);

        // ── Subtle ground fog (bottom 30 design-px) ──
        // Just a semi-transparent band fading into the background
        float fogY = 190 * sy;
        float fogH = h - fogY;
        canvas.FillColor = Color.FromArgb("#2c3e50").WithAlpha(0.0f);
        canvas.FillRectangle(0, fogY, w, fogH * 0.5f);
        canvas.FillColor = Color.FromArgb("#f5f6f8").WithAlpha(0.3f);
        canvas.FillRectangle(0, fogY + fogH * 0.5f, w, fogH * 0.5f);
    }

    /// <summary>Fill a triangle with a solid color at full opacity.</summary>
    private static void FillTriangle(ICanvas canvas, Color color,
        float x1, float y1, float x2, float y2, float x3, float y3)
    {
        var path = new PathF();
        path.MoveTo(x1, y1);
        path.LineTo(x2, y2);
        path.LineTo(x3, y3);
        path.Close();
        canvas.FillColor = color;
        canvas.FillPath(path);
    }

    /// <summary>Fill a triangle with a solid color and custom opacity.</summary>
    private static void FillTriangle(ICanvas canvas, Color color, float opacity,
        float x1, float y1, float x2, float y2, float x3, float y3)
    {
        var path = new PathF();
        path.MoveTo(x1, y1);
        path.LineTo(x2, y2);
        path.LineTo(x3, y3);
        path.Close();
        canvas.FillColor = color.WithAlpha(opacity);
        canvas.FillPath(path);
    }
}
