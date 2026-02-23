namespace ScoutCode.Pipelines;

// Para cuando se implemente la segmentacion de imagenes con OpenCV
public interface IImageSegmenter
{
    Task<List<byte[]>> SegmentSymbolsAsync(byte[] imageBytes);
}
