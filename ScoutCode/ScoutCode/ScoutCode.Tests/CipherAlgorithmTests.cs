using ScoutCode.Ciphers;
using ScoutCode.Models;
using ScoutCode.Services;
using Xunit;

namespace ScoutCode.Tests;

// Tests para todos los algoritmos de cifrado
public class CipherAlgorithmTests
{
    // ==================== CIPHER UTILS ====================

    [Theory]
    [InlineData('a', 'B', 'b')]  // original minúscula → resultado minúscula
    [InlineData('A', 'b', 'B')]  // original mayúscula → resultado mayúscula
    [InlineData('z', 'A', 'a')]
    public void CipherUtils_PreserveCase_Works(char original, char newChar, char expected)
    {
        Assert.Equal(expected, CipherUtils.PreserveCase(original, newChar));
    }

    [Theory]
    [InlineData('A', true)]
    [InlineData('z', true)]
    [InlineData('Ñ', true)]
    [InlineData('ñ', true)]
    [InlineData('1', false)]
    [InlineData(' ', false)]
    public void CipherUtils_IsLetterSpanish_Works(char c, bool expected)
    {
        Assert.Equal(expected, CipherUtils.IsLetterSpanish(c));
    }

    [Fact]
    public void CipherUtils_SpanishAlphabet_Has27Letters()
    {
        Assert.Equal(27, CipherUtils.SpanishAlphabet.Length);
        Assert.Equal('A', CipherUtils.SpanishAlphabet[0]);
        Assert.Equal('Ñ', CipherUtils.SpanishAlphabet[14]);
        Assert.Equal('Z', CipherUtils.SpanishAlphabet[26]);
    }

    [Theory]
    [InlineData('A', 0)]
    [InlineData('N', 13)]
    [InlineData('Ñ', 14)]
    [InlineData('O', 15)]
    [InlineData('Z', 26)]
    [InlineData('ñ', 14)]  // minúscula también funciona
    public void CipherUtils_GetSpanishIndex_Works(char c, int expected)
    {
        Assert.Equal(expected, CipherUtils.GetSpanishIndex(c));
    }

    // ==================== MORSE ====================

    [Fact]
    public void Morse_Encrypt_SOS()
    {
        var algo = new MorseCipherAlgorithm();
        Assert.Equal("... --- ...", algo.Encrypt("SOS"));
    }

    [Fact]
    public void Morse_Encrypt_WithSpaces_WordsSeparatedBySlash()
    {
        var algo = new MorseCipherAlgorithm();
        // "HI MOM" → H=...., I=.. / M=--, O=---, M=--
        Assert.Equal(".... .. / -- --- --", algo.Encrypt("HI MOM"));
    }

    [Fact]
    public void Morse_Decrypt_SOS()
    {
        var algo = new MorseCipherAlgorithm();
        Assert.Equal("SOS", algo.Decrypt("... --- ..."));
    }

    [Fact]
    public void Morse_Roundtrip()
    {
        var algo = new MorseCipherAlgorithm();
        var original = "HELLO WORLD";
        Assert.Equal(original, algo.Decrypt(algo.Encrypt(original)));
    }

    // ==================== NUMÉRICA ====================

    [Fact]
    public void Numeric_Encrypt_ABC()
    {
        var algo = new NumericCipherAlgorithm();
        // A=00, B=01, C=02
        Assert.Equal("000102", algo.Encrypt("ABC"));
    }

    [Fact]
    public void Numeric_Encrypt_Ñ_IsIndex14()
    {
        var algo = new NumericCipherAlgorithm();
        Assert.Equal("14", algo.Encrypt("Ñ"));
    }

    [Fact]
    public void Numeric_Encrypt_Z_IsIndex26()
    {
        var algo = new NumericCipherAlgorithm();
        Assert.Equal("26", algo.Encrypt("Z"));
    }

    [Fact]
    public void Numeric_Encrypt_PreservesSpaces()
    {
        var algo = new NumericCipherAlgorithm();
        // A=00, espacio se copia, Z=26
        Assert.Equal("00 26", algo.Encrypt("A Z"));
    }

    [Fact]
    public void Numeric_Decrypt_Basic()
    {
        var algo = new NumericCipherAlgorithm();
        Assert.Equal("ABC", algo.Decrypt("000102"));
    }

    [Fact]
    public void Numeric_Roundtrip_HOLA()
    {
        var algo = new NumericCipherAlgorithm();
        Assert.Equal("HOLA", algo.Decrypt(algo.Encrypt("HOLA")));
    }

    // ==================== CELULAR ====================

    [Fact]
    public void Cellphone_Encrypt_HOLA()
    {
        var algo = new CellphoneCipherAlgorithm();
        // H=4^2, O=6^3, L=5^3, A=2
        Assert.Equal("4^2-6^3-5^3-2", algo.Encrypt("HOLA"));
    }

    [Fact]
    public void Cellphone_Encrypt_SpaceEquals0()
    {
        var algo = new CellphoneCipherAlgorithm();
        // H=4^2, I=4^3, space=0
        Assert.Equal("4^2-4^3-0-2", algo.Encrypt("HI A"));
    }

    [Fact]
    public void Cellphone_Decrypt_HOLA()
    {
        var algo = new CellphoneCipherAlgorithm();
        Assert.Equal("HOLA", algo.Decrypt("4^2-6^3-5^3-2"));
    }

    [Fact]
    public void Cellphone_Roundtrip()
    {
        var algo = new CellphoneCipherAlgorithm();
        Assert.Equal("HOLA MUNDO", algo.Decrypt(algo.Encrypt("HOLA MUNDO")));
    }

    // ==================== CENIT-POLAR ====================

    [Fact]
    public void CenitPolar_Encrypt_cenitToPolar()
    {
        var algo = new CenitPolarCipherAlgorithm();
        Assert.Equal("polar", algo.Encrypt("cenit"));
    }

    [Fact]
    public void CenitPolar_IsSymmetric()
    {
        var algo = new CenitPolarCipherAlgorithm();
        Assert.Equal("cenit", algo.Encrypt("polar"));
    }

    [Fact]
    public void CenitPolar_PreservesCase()
    {
        var algo = new CenitPolarCipherAlgorithm();
        Assert.Equal("Polar", algo.Encrypt("Cenit"));
    }

    [Fact]
    public void CenitPolar_OnlyChangesMapLetters_OthersStay()
    {
        var algo = new CenitPolarCipherAlgorithm();
        // h, 2, ! no están en C,E,N,I,T,P,O,L,A,R → se mantienen
        // c→p, e→o, n→l, i→a  =>  "h2!pola"
        Assert.Equal("h2!pola", algo.Encrypt("h2!ceni"));
    }

    // ==================== BADEN-POWELL ====================

    [Fact]
    public void BadenPowel_Encrypt_badenToPowel()
    {
        var algo = new BadenPowelCipherAlgorithm();
        Assert.Equal("powel", algo.Encrypt("baden"));
    }

    [Fact]
    public void BadenPowel_IsSymmetric()
    {
        var algo = new BadenPowelCipherAlgorithm();
        Assert.Equal("baden", algo.Encrypt("powel"));
    }

    [Fact]
    public void BadenPowel_PreservesUnmappedChars()
    {
        var algo = new BadenPowelCipherAlgorithm();
        // x,y,z,h,2 no están en B,A,D,E,N,P,O,W,L → se mantienen
        Assert.Equal("h2xyz", algo.Encrypt("h2xyz"));
    }

    // ==================== MURCIÉLAGO ====================

    [Fact]
    public void Murcielago_Encrypt_LettersToDigits()
    {
        var algo = new MurcielagoCipherAlgorithm();
        Assert.Equal("0123456789", algo.Encrypt("MURCIELAGO"));
    }

    [Fact]
    public void Murcielago_Decrypt_DigitsToLetters()
    {
        var algo = new MurcielagoCipherAlgorithm();
        Assert.Equal("MURCIELAGO", algo.Decrypt("0123456789"));
    }

    [Fact]
    public void Murcielago_PreservesNonMappedLettersAndNumbers()
    {
        var algo = new MurcielagoCipherAlgorithm();
        // "h2ola1" → h(no map)=h, 2=2, o→9, l→6, a→7, 1=1
        Assert.Equal("h29671", algo.Encrypt("h2ola1"));
    }

    [Fact]
    public void Murcielago_Roundtrip()
    {
        var algo = new MurcielagoCipherAlgorithm();
        Assert.Equal("MURCIELAGO", algo.Decrypt(algo.Encrypt("MURCIELAGO")));
    }

    // ==================== CLAVE +1 ====================

    [Fact]
    public void ShiftPlusOne_Encrypt_ABCtoBCD()
    {
        var algo = new ShiftPlusOneCipherAlgorithm();
        Assert.Equal("BCD", algo.Encrypt("ABC"));
    }

    [Fact]
    public void ShiftPlusOne_WrapAround_ZtoA()
    {
        var algo = new ShiftPlusOneCipherAlgorithm();
        Assert.Equal("A", algo.Encrypt("Z"));
    }

    [Fact]
    public void ShiftPlusOne_Spanish_NtoÑ_ÑtoO()
    {
        var algo = new ShiftPlusOneCipherAlgorithm();
        Assert.Equal("Ñ", algo.Encrypt("N"));
        Assert.Equal("O", algo.Encrypt("Ñ"));
    }

    [Fact]
    public void ShiftPlusOne_PreservesCase()
    {
        var algo = new ShiftPlusOneCipherAlgorithm();
        Assert.Equal("bcd", algo.Encrypt("abc"));
    }

    [Fact]
    public void ShiftPlusOne_PreservesNonLetters()
    {
        var algo = new ShiftPlusOneCipherAlgorithm();
        Assert.Equal("b2c!d", algo.Encrypt("a2b!c"));
    }

    [Fact]
    public void ShiftPlusOne_Roundtrip()
    {
        var algo = new ShiftPlusOneCipherAlgorithm();
        var original = "Hola Mundo!";
        Assert.Equal(original, algo.Decrypt(algo.Encrypt(original)));
    }

    // ==================== CLAVE -1 ====================

    [Fact]
    public void ShiftMinusOne_Encrypt_BCDtoABC()
    {
        var algo = new ShiftMinusOneCipherAlgorithm();
        Assert.Equal("ABC", algo.Encrypt("BCD"));
    }

    [Fact]
    public void ShiftMinusOne_WrapAround_AtoZ()
    {
        var algo = new ShiftMinusOneCipherAlgorithm();
        Assert.Equal("Z", algo.Encrypt("A"));
    }

    [Fact]
    public void ShiftMinusOne_Spanish_ÑtoN_OtoÑ()
    {
        var algo = new ShiftMinusOneCipherAlgorithm();
        Assert.Equal("N", algo.Encrypt("Ñ"));
        Assert.Equal("Ñ", algo.Encrypt("O"));
    }

    [Fact]
    public void ShiftMinusOne_Roundtrip()
    {
        var algo = new ShiftMinusOneCipherAlgorithm();
        var original = "Hola Mundo!";
        Assert.Equal(original, algo.Decrypt(algo.Encrypt(original)));
    }

    // ==================== PARELINOFO ====================

    [Fact]
    public void Parelinofo_Encrypt_parentToUfonet()
    {
        var algo = new ParelinofoCipherAlgorithm();
        // P→U, A→F, R→O, E→N, N→E, T no está → T
        Assert.Equal("ufonet", algo.Encrypt("parent"));
    }

    [Fact]
    public void Parelinofo_IsSymmetric()
    {
        var algo = new ParelinofoCipherAlgorithm();
        var original = "parelinofu";
        var encrypted = algo.Encrypt(original);
        Assert.Equal(original, algo.Encrypt(encrypted));
    }

    [Fact]
    public void Parelinofo_PreservesUnmappedChars()
    {
        var algo = new ParelinofoCipherAlgorithm();
        // B,C,D,G,H,J,K,M,Q,S,T,V,W,X,Y,Z no están en el mapa
        Assert.Equal("bcdgh", algo.Encrypt("bcdgh"));
    }

    // ==================== DAMETUPICO ====================

    [Fact]
    public void Dametupico_Encrypt_damepToOcipe()
    {
        var algo = new DametupicoCipherAlgorithm();
        // D→O, A→C, M→I, E→P, P→E
        Assert.Equal("ocipe", algo.Encrypt("damep"));
    }

    [Fact]
    public void Dametupico_IsSymmetric()
    {
        var algo = new DametupicoCipherAlgorithm();
        var original = "dametupico";
        var encrypted = algo.Encrypt(original);
        Assert.Equal(original, algo.Encrypt(encrypted));
    }

    // ==================== AGUJERITO ====================

    [Fact]
    public void Agujerito_Encrypt()
    {
        var algo = new AgujeritoCipherAlgorithm();
        // a→o, g→t, u→i, j→r, e→e, r→j, i→u, t→g
        Assert.Equal("otirejug", algo.Encrypt("agujerit"));
    }

    [Fact]
    public void Agujerito_IsSymmetric()
    {
        var algo = new AgujeritoCipherAlgorithm();
        var original = "agujerito";
        var encrypted = algo.Encrypt(original);
        Assert.Equal(original, algo.Encrypt(encrypted));
    }

    [Fact]
    public void Agujerito_PreservesUnmappedChars()
    {
        var algo = new AgujeritoCipherAlgorithm();
        Assert.Equal("bcd123!@#", algo.Encrypt("bcd123!@#"));
    }

    // ==================== CIPHER SERVICE ====================

    [Fact]
    public void CipherService_Process_AllTypes_ShouldNotThrow()
    {
        var service = new CipherService();
        foreach (CipherType type in Enum.GetValues<CipherType>())
        {
            var result = service.Process(type, OperationMode.Encrypt, "TEST");
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }

    [Fact]
    public void CipherService_GetAvailableCiphers_Returns14()
    {
        var service = new CipherService();
        var ciphers = service.GetAvailableCiphers();
        Assert.Equal(14, ciphers.Count);
    }

    // ==================== REGLA GLOBAL: longitud y roundtrip para sustituciones char-a-char ====================

    [Theory]
    [InlineData("h2ola1")]
    [InlineData("abc 123 !@# ñ")]
    [InlineData("test@email.com")]
    [InlineData("¡Hola! ¿qué tal?")]
    public void SymmetricCiphers_PreserveLength_And_Roundtrip(string input)
    {
        var service = new CipherService();
        var symmetricTypes = new[]
        {
            CipherType.CenitPolar, CipherType.BadenPowel,
            CipherType.Parelinofo, CipherType.Dametupico, CipherType.Agujerito,
            CipherType.ShiftPlusOne, CipherType.ShiftMinusOne
        };

        foreach (var type in symmetricTypes)
        {
            var encrypted = service.Process(type, OperationMode.Encrypt, input);
            // Las sustituciones char-a-char mantienen longitud
            Assert.Equal(input.Length, encrypted.Length);
            // Roundtrip: descifrar lo cifrado devuelve el original
            var decrypted = service.Process(type, OperationMode.Decrypt, encrypted);
            Assert.Equal(input, decrypted);
        }
    }
}
