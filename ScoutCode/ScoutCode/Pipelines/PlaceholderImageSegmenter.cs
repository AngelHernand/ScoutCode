namespace ScoutCode.Pipelines;

// Placeholder, devuelve lista vacia por ahora
public class PlaceholderImageSegmenter : IImageSegmenter
{
    public Task<List<byte[]>> SegmentSymbolsAsync(byte[] imageBytes)
    {
        return Task.FromResult(new List<byte[]>());
    }
}
