using ScoutCode.Ciphers;
using ScoutCode.Models;

namespace ScoutCode.Services;

// Registro de todos los cifrados. Recibe el tipo y la operacion
// y lo manda al algoritmo que corresponde.
public class CipherService : ICipherService
{
    private readonly Dictionary<CipherType, ICipherAlgorithm> _algorithms;

    public CipherService()
    {
        _algorithms = new Dictionary<CipherType, ICipherAlgorithm>
        {
            { CipherType.Morse, new MorseCipherAlgorithm() },
            { CipherType.Numeric, new NumericCipherAlgorithm() },
            { CipherType.Cellphone, new CellphoneCipherAlgorithm() },
            { CipherType.CenitPolar, new CenitPolarCipherAlgorithm() },
            { CipherType.BadenPowel, new BadenPowelCipherAlgorithm() },
            { CipherType.Murcielago, new MurcielagoCipherAlgorithm() },
            { CipherType.ShiftPlusOne, new ShiftPlusOneCipherAlgorithm() },
            { CipherType.ShiftMinusOne, new ShiftMinusOneCipherAlgorithm() },
            { CipherType.Parelinofo, new ParelinofoCipherAlgorithm() },
            { CipherType.Dametupico, new DametupicoCipherAlgorithm() },
            { CipherType.Agujerito, new AgujeritoCipherAlgorithm() },
        };
    }

    public string Process(CipherType type, OperationMode operation, string input)
    {
        if (!_algorithms.TryGetValue(type, out var algorithm))
            return $"Error: Cifrado '{type}' no implementado aún.";

        return operation switch
        {
            OperationMode.Encrypt => algorithm.Encrypt(input),
            OperationMode.Decrypt => algorithm.Decrypt(input),
            _ => "Error: Operación no válida."
        };
    }

    public string GetSupportedCharacters(CipherType type)
    {
        if (_algorithms.TryGetValue(type, out var algorithm))
            return algorithm.SupportedCharacters;

        return "Información no disponible.";
    }

    public List<CipherDefinition> GetAvailableCiphers()
    {
        return new List<CipherDefinition>
        {
            new()
            {
                Name = "Morse",
                Description = "Código Morse internacional: puntos y rayas.",
                Type = CipherType.Morse,
                Icon = "MO",
                IsAvailable = true
            },
            new()
            {
                Name = "Numérica",
                Description = "A=00, B=01, C=02 ... Ñ=14 ... Z=26.",
                Type = CipherType.Numeric,
                Icon = "01",
                IsAvailable = true
            },
            new()
            {
                Name = "Celular (Teléfono)",
                Description = "Teclado T9: A=2, B=2^2, C=2^3, espacio=0.",
                Type = CipherType.Cellphone,
                Icon = "T9",
                IsAvailable = true
            },
            new()
            {
                Name = "Cenit-Polar",
                Description = "Intercambio: C↔P, E↔O, N↔L, I↔A, T↔R.",
                Type = CipherType.CenitPolar,
                Icon = "CP",
                IsAvailable = true
            },
            new()
            {
                Name = "Baden-Powell",
                Description = "Intercambio: B↔P, A↔O, D↔W, E=E, N↔L.",
                Type = CipherType.BadenPowel,
                Icon = "BP",
                IsAvailable = true
            },
            new()
            {
                Name = "Murciélago",
                Description = "MURCIELAGO = 0123456789.",
                Type = CipherType.Murcielago,
                Icon = "MU",
                IsAvailable = true
            },
            new()
            {
                Name = "Clave +1",
                Description = "Cada letra → la siguiente (con Ñ, Z→A).",
                Type = CipherType.ShiftPlusOne,
                Icon = "+1",
                IsAvailable = true
            },
            new()
            {
                Name = "Clave -1",
                Description = "Cada letra → la anterior (con Ñ, A→Z).",
                Type = CipherType.ShiftMinusOne,
                Icon = "-1",
                IsAvailable = true
            },
            new()
            {
                Name = "Parelinofo",
                Description = "Intercambio: P↔U, A↔F, R↔O, E↔N, L↔I.",
                Type = CipherType.Parelinofo,
                Icon = "PA",
                IsAvailable = true
            },
            new()
            {
                Name = "Dametupico",
                Description = "Intercambio: D↔O, A↔C, M↔I, E↔P, T↔U.",
                Type = CipherType.Dametupico,
                Icon = "DA",
                IsAvailable = true
            },
            new()
            {
                Name = "Agujerito",
                Description = "Intercambio: A↔O, G↔T, U↔I, J↔R, E=E.",
                Type = CipherType.Agujerito,
                Icon = "AG",
                IsAvailable = true
            },
        };
    }
}
