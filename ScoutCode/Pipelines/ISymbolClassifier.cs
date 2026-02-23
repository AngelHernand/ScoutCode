namespace ScoutCode.Pipelines;

// Para cuando se implemente el clasificador de simbolos con ONNX
public interface ISymbolClassifier
{
    Task<(char Character, float Confidence)> ClassifyAsync(byte[] symbolImage);
}
