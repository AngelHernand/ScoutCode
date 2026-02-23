namespace ScoutCode.Pipelines;

// Placeholder, devuelve '?' con confianza 0
public class PlaceholderSymbolClassifier : ISymbolClassifier
{
    public Task<(char Character, float Confidence)> ClassifyAsync(byte[] symbolImage)
    {
        return Task.FromResult(('?', 0f));
    }
}
