using Microsoft.Extensions.Logging;
using ScoutCode.Models;
using SkiaSharp;

namespace ScoutCode.Pipelines;

/// <summary>
/// Pipeline de reconocimiento de símbolos usando SkiaSharp para procesamiento
/// de imagen y template matching con firmas binarias pre-computadas.
///
/// Flujo: Imagen → Escala de grises → Binarización → Segmentación → Matching
/// </summary>
public class SkiaSharpSymbolPipeline : ICameraPipeline
{
    private readonly ILogger<SkiaSharpSymbolPipeline> _logger;
    private const int TemplateSize = TemplateData.Size; // 48x48
    private const float SpaceGapMultiplier = 1.8f;
    private const float MinMatchRatio = 0.55f; // Mínimo 55% de píxeles coinciden

    public SkiaSharpSymbolPipeline(ILogger<SkiaSharpSymbolPipeline> logger)
    {
        _logger = logger;
    }

    public Task<string> ProcessImageAsync(byte[] imageBytes)
    {
        return Task.FromResult("Usa ProcessSymbolicImageAsync para cifrados simbólicos.");
    }

    public Task<string> ProcessSymbolicImageAsync(byte[] imageBytes, CipherType cipherType)
    {
        try
        {
            var templates = GetTemplates(cipherType);
            if (templates == null || templates.Count == 0)
                return Task.FromResult("Error: no hay plantillas para este cifrado.");

            using var bitmap = SKBitmap.Decode(imageBytes);
            if (bitmap == null)
                return Task.FromResult("Error: no se pudo decodificar la imagen.");

            _logger.LogInformation("Pipeline: imagen {W}x{H}, cifrado {Type}", bitmap.Width, bitmap.Height, cipherType);

            // 1. Preprocesar
            var gray = ToGrayscale(bitmap);
            var binary = AdaptiveThreshold(gray, bitmap.Width, bitmap.Height);

            // 2. Segmentar
            var regions = SegmentSymbols(binary, bitmap.Width, bitmap.Height);
            _logger.LogInformation("Pipeline: {Count} regiones detectadas", regions.Count);

            if (regions.Count == 0)
                return Task.FromResult("No se detectaron símbolos en la imagen. Intenta con mejor iluminación y contraste.");

            // 3. Matchear cada región contra plantillas
            var keys = new List<string>();
            foreach (var region in regions)
            {
                if (region.IsSpace)
                {
                    keys.Add(GetSpaceKey(cipherType));
                    continue;
                }

                var symbolBinary = ExtractAndNormalize(binary, bitmap.Width, region);
                var (bestKey, matchRatio) = FindBestMatch(symbolBinary, templates);

                _logger.LogDebug("Pipeline: región ({L},{T})-({R},{B}) → '{Key}' ({Ratio:P0})",
                    region.Left, region.Top, region.Right, region.Bottom, bestKey, matchRatio);

                keys.Add(matchRatio >= MinMatchRatio ? bestKey : "?");
            }

            // 4. Formatear resultado
            var prefix = cipherType switch
            {
                CipherType.Gato => "GATO:",
                CipherType.Semaforo => "SEMAFORO:",
                CipherType.Electrica => "ELECTRICA:",
                _ => ""
            };

            var result = prefix + string.Join(",", keys);
            _logger.LogInformation("Pipeline: resultado = {Result}", result);
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Pipeline: error procesando imagen");
            return Task.FromResult($"Error al procesar: {ex.Message}");
        }
    }

    // --- Preprocesamiento ---

    private static byte[] ToGrayscale(SKBitmap bitmap)
    {
        var pixels = bitmap.Pixels;
        var gray = new byte[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            var c = pixels[i];
            gray[i] = (byte)(0.299 * c.Red + 0.587 * c.Green + 0.114 * c.Blue);
        }
        return gray;
    }

    private static bool[] AdaptiveThreshold(byte[] gray, int width, int height)
    {
        // Otsu global threshold
        var hist = new int[256];
        foreach (var v in gray) hist[v]++;
        int total = gray.Length;
        long sumAll = 0;
        for (int i = 0; i < 256; i++) sumAll += (long)i * hist[i];

        int bestThresh = 128;
        double bestVar = 0;
        int w0 = 0;
        long sum0 = 0;
        for (int t = 0; t < 256; t++)
        {
            w0 += hist[t];
            if (w0 == 0) continue;
            int w1 = total - w0;
            if (w1 == 0) break;
            sum0 += (long)t * hist[t];
            double m0 = (double)sum0 / w0;
            double m1 = (double)(sumAll - sum0) / w1;
            double var = (double)w0 * w1 * (m0 - m1) * (m0 - m1);
            if (var > bestVar)
            {
                bestVar = var;
                bestThresh = t;
            }
        }

        var binary = new bool[gray.Length];
        for (int i = 0; i < gray.Length; i++)
            binary[i] = gray[i] < bestThresh;

        return binary;
    }

    // --- Segmentación basada en proyecciones ---

    private struct SymbolRegion
    {
        public int Left, Top, Right, Bottom;
        public bool IsSpace;
        public int Width => Right - Left;
        public int Height => Bottom - Top;
    }

    private static List<SymbolRegion> SegmentSymbols(bool[] binary, int width, int height)
    {
        // 1. Proyección horizontal: encontrar líneas de texto
        var hProjection = new int[height];
        for (int y = 0; y < height; y++)
        {
            int count = 0;
            for (int x = 0; x < width; x++)
                if (binary[y * width + x]) count++;
            hProjection[y] = count;
        }

        var lines = FindRanges(hProjection, height, width * 0.005);

        if (lines.Count == 0)
            return new List<SymbolRegion>();

        var allRegions = new List<SymbolRegion>();

        // 2. Para cada línea, proyección vertical: encontrar símbolos
        foreach (var (lineTop, lineBottom) in lines)
        {
            var vProjection = new int[width];
            for (int x = 0; x < width; x++)
            {
                int count = 0;
                for (int y = lineTop; y < lineBottom; y++)
                    if (binary[y * width + x]) count++;
                vProjection[x] = count;
            }

            int lineHeight = lineBottom - lineTop;
            var columns = FindRanges(vProjection, width, lineHeight * 0.01);

            if (columns.Count == 0) continue;

            // Detectar espacios entre símbolos
            var symbolWidths = columns.Select(c => c.end - c.start).ToList();
            double avgWidth = symbolWidths.Average();

            for (int i = 0; i < columns.Count; i++)
            {
                // Agregar espacio si el gap es grande
                if (i > 0)
                {
                    int gap = columns[i].start - columns[i - 1].end;
                    if (gap > avgWidth * SpaceGapMultiplier)
                    {
                        allRegions.Add(new SymbolRegion { IsSpace = true });
                    }
                }

                allRegions.Add(new SymbolRegion
                {
                    Left = columns[i].start,
                    Top = lineTop,
                    Right = columns[i].end,
                    Bottom = lineBottom,
                    IsSpace = false
                });
            }

            // Agregar espacio entre líneas
            allRegions.Add(new SymbolRegion { IsSpace = true });
        }

        // Quitar espacio final
        if (allRegions.Count > 0 && allRegions[^1].IsSpace)
            allRegions.RemoveAt(allRegions.Count - 1);

        return allRegions;
    }

    private static List<(int start, int end)> FindRanges(int[] projection, int length, double threshold)
    {
        var ranges = new List<(int start, int end)>();
        int? start = null;

        for (int i = 0; i < length; i++)
        {
            if (projection[i] > threshold)
            {
                start ??= i;
            }
            else if (start.HasValue)
            {
                ranges.Add((start.Value, i));
                start = null;
            }
        }

        if (start.HasValue)
            ranges.Add((start.Value, length));

        // Merge ranges that are very close (small gaps within a symbol)
        var merged = new List<(int start, int end)>();
        foreach (var r in ranges)
        {
            if (merged.Count > 0)
            {
                var last = merged[^1];
                int gap = r.start - last.end;
                int avgSize = (last.end - last.start + r.end - r.start) / 2;
                if (gap < avgSize * 0.3) // Gap < 30% of average size → merge
                {
                    merged[^1] = (last.start, r.end);
                    continue;
                }
            }
            merged.Add(r);
        }

        return merged;
    }

    // --- Extracción y normalización ---

    private static byte[] ExtractAndNormalize(bool[] binary, int srcWidth, SymbolRegion region)
    {
        int rw = region.Width;
        int rh = region.Height;

        if (rw <= 0 || rh <= 0)
            return new byte[TemplateData.SignatureBytes];

        // Hacer cuadrado (padding para mantener aspect ratio)
        int maxDim = Math.Max(rw, rh);
        int padX = (maxDim - rw) / 2;
        int padY = (maxDim - rh) / 2;

        // Area-average resize a TemplateSize x TemplateSize
        var packed = new byte[TemplateData.SignatureBytes];

        for (int dy = 0; dy < TemplateSize; dy++)
        {
            for (int dx = 0; dx < TemplateSize; dx++)
            {
                // Map destination pixel to source coordinates
                int sx = dx * maxDim / TemplateSize - padX + region.Left;
                int sy = dy * maxDim / TemplateSize - padY + region.Top;

                // Area sample (2x2 block for basic anti-aliasing)
                int fgCount = 0;
                int samples = 0;
                int blockSize = Math.Max(1, maxDim / TemplateSize);
                for (int by = 0; by < blockSize && sy + by < binary.Length / srcWidth; by++)
                {
                    for (int bx = 0; bx < blockSize; bx++)
                    {
                        int px = sx + bx;
                        int py = sy + by;
                        if (px >= 0 && px < srcWidth && py >= 0 && py < binary.Length / srcWidth)
                        {
                            samples++;
                            if (binary[py * srcWidth + px])
                                fgCount++;
                        }
                    }
                }

                bool isForeground = samples > 0 && fgCount > samples / 2;

                if (isForeground)
                {
                    int byteIdx = dy * TemplateData.BytesPerRow + dx / 8;
                    int bitIdx = 7 - (dx % 8);
                    packed[byteIdx] |= (byte)(1 << bitIdx);
                }
            }
        }

        return packed;
    }

    // --- Template matching ---

    private static (string key, float matchRatio) FindBestMatch(
        byte[] symbolSignature,
        Dictionary<string, (byte[] Signature, int ForegroundCount)> templates)
    {
        // Contar píxeles foreground del input
        int inputFg = 0;
        foreach (var b in symbolSignature)
            inputFg += CountBits(b);

        string bestKey = "?";
        int bestDistance = int.MaxValue;
        int totalBits = TemplateSize * TemplateSize;

        foreach (var (key, (templateSig, templateFg)) in templates)
        {
            // Pre-filtro: si la cantidad de píxeles difiere mucho, skip
            if (Math.Abs(inputFg - templateFg) > totalBits * 0.35)
                continue;

            // Hamming distance
            int distance = 0;
            for (int i = 0; i < symbolSignature.Length && i < templateSig.Length; i++)
                distance += CountBits((byte)(symbolSignature[i] ^ templateSig[i]));

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestKey = key;
            }
        }

        float matchRatio = 1f - (float)bestDistance / totalBits;
        return (bestKey, matchRatio);
    }

    private static int CountBits(byte value)
    {
        int count = 0;
        while (value != 0)
        {
            count += value & 1;
            value >>= 1;
        }
        return count;
    }

    // --- Helpers ---

    private static Dictionary<string, (byte[] Signature, int ForegroundCount)>? GetTemplates(CipherType type)
    {
        return type switch
        {
            CipherType.Gato => TemplateData.GatoTemplates,
            CipherType.Semaforo => TemplateData.SemaforoTemplates,
            CipherType.Electrica => TemplateData.ElectricaTemplates,
            _ => null
        };
    }

    private static string GetSpaceKey(CipherType type)
    {
        return type == CipherType.Electrica ? "space" : " ";
    }
}
