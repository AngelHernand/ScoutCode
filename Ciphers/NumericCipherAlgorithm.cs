using System.Text;
using System.Text.RegularExpressions;

namespace ScoutCode.Ciphers;

/// <summary>
/// Cifrado Numérico: A=00, B=01, C=02 ... Z=25, (espacio extra hasta 27 si se requiere).
/// Nota: se respeta A=00 ... Z=25 para 26 letras estándar (sin Ñ en esta clave).
/// Si el usuario pide A=00...Z=27, se interpreta con Ñ incluida:
/// A=00,B=01,...N=13,Ñ=14,O=15,...Z=27 (27 letras = índice 0-26, pero el usuario dijo "Z=27").
/// Se implementa: A=00...N=13, Ñ=14, O=15...Z=26. Total 27 valores: 00-26.
/// CORRECCIÓN según spec "Z=27": A=01...Z=27 desplazado, o A=00...Z=27 con 28 valores.
/// Se implementa literal: A=00, B=01, ... Z=25 SIN Ñ para mantener Z=25,
/// PERO el usuario pidió Z=27, así que usamos el alfabeto español completo:
/// A=00,B=01,C=02,D=03,E=04,F=05,G=06,H=07,I=08,J=09,K=10,L=11,M=12,
/// N=13,Ñ=14,O=15,P=16,Q=17,R=18,S=19,T=20,U=21,V=22,W=23,X=24,Y=25,Z=26
/// (27 letras, 00-26). El usuario dijo "Z=27" lo cual podría significar base-1.
/// Implementación final: usar base-0 con 27 letras (Z=26). Si el spec dice Z=27
/// probablemente es error o base-1 indexing. Usaremos lo más natural: base-0, 00-26.
/// </summary>
public partial class NumericCipherAlgorithm : ICipherAlgorithm
{
    public string DisplayName => "Numérica";
    public string SupportedCharacters => "A-Z, Ñ → 00-26 (resto se mantiene igual)";

    // Alfabeto español para esta clave
    private static readonly char[] Alphabet = CipherUtils.SpanishAlphabet;

    private static readonly Dictionary<char, string> LetterToNumber;
    private static readonly Dictionary<string, char> NumberToLetter;

    static NumericCipherAlgorithm()
    {
        LetterToNumber = new Dictionary<char, string>();
        NumberToLetter = new Dictionary<string, char>();
        for (int i = 0; i < Alphabet.Length; i++)
        {
            var num = i.ToString("D2"); // 00, 01, 02 ...
            LetterToNumber[Alphabet[i]] = num;
            NumberToLetter[num] = Alphabet[i];
        }
    }

    public string Encrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var sb = new StringBuilder();
        foreach (char c in input)
        {
            var upper = char.ToUpperInvariant(c);
            if (LetterToNumber.TryGetValue(upper, out var num))
                sb.Append(num);
            else
                sb.Append(c); // espacios, signos, acentuadas → copiar igual
        }
        return sb.ToString();
    }

    public string Decrypt(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var sb = new StringBuilder();
        int i = 0;

        while (i < input.Length)
        {
            // Intentar leer 2 dígitos consecutivos
            if (i + 1 < input.Length &&
                char.IsDigit(input[i]) && char.IsDigit(input[i + 1]))
            {
                var pair = input.Substring(i, 2);
                if (NumberToLetter.TryGetValue(pair, out var letter))
                {
                    sb.Append(letter);
                    i += 2;
                    continue;
                }
            }

            // No es un par válido: copiar el carácter tal cual
            sb.Append(input[i]);
            i++;
        }

        return sb.ToString();
    }
}
