using ScoutCode.Models;

namespace ScoutCode.Services;

public interface ICipherService
{
    string Process(CipherType type, OperationMode operation, string input);
    string GetSupportedCharacters(CipherType type);
    List<CipherDefinition> GetAvailableCiphers();
}
