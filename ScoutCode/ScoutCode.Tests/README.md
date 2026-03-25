# ScoutCode.Tests - Pruebas Unitarias

Proyecto de pruebas unitarias para ScoutCode. Utiliza xUnit como framework de testing y valida el correcto funcionamiento de los 13 algoritmos de cifrado, las utilidades compartidas y el servicio de cifrado.

---

## Tabla de contenidos

1. [Resumen](#resumen)
2. [Arquitectura del proyecto de pruebas](#arquitectura-del-proyecto-de-pruebas)
3. [Ejecucion](#ejecucion)
4. [Catalogo de pruebas](#catalogo-de-pruebas)
5. [Estrategia de vinculacion de archivos](#estrategia-de-vinculacion-de-archivos)
6. [Dependencias](#dependencias)
7. [Convenciones de pruebas](#convenciones-de-pruebas)
8. [Guia para agregar pruebas](#guia-para-agregar-pruebas)

---

## Resumen

| Metrica               | Valor  |
|------------------------|--------|
| Total de pruebas       | 73     |
| Framework              | xUnit 2.6.6 |
| Target framework       | net8.0 |
| Archivo de pruebas     | CipherAlgorithmTests.cs |
| Algoritmos cubiertos   | 13 de 13 |
| Clase de prueba        | CipherAlgorithmTests |

---

## Arquitectura del proyecto de pruebas

```
ScoutCode.Tests/
|-- CipherAlgorithmTests.cs     Todas las pruebas unitarias (73 tests)
|-- ScoutCode.Tests.csproj      Configuracion y vinculacion de archivos fuente
```

El proyecto de pruebas **no usa ProjectReference** al proyecto MAUI principal. En su lugar, vincula directamente los archivos fuente necesarios mediante directivas `<Compile Include>` en el `.csproj`. Esta estrategia evita la dependencia de paquetes MAUI y permite ejecutar las pruebas sobre `net8.0` estandar sin necesidad de los workloads de MAUI instalados.

---

## Ejecucion

### Ejecutar todas las pruebas

```bash
dotnet test ScoutCode.Tests/ScoutCode.Tests.csproj -c Debug --verbosity normal
```

### Ejecutar pruebas con filtro

```bash
# Solo pruebas de Morse
dotnet test --filter "FullyQualifiedName~Morse"

# Solo pruebas de simetria
dotnet test --filter "FullyQualifiedName~IsSymmetric"

# Solo pruebas de roundtrip
dotnet test --filter "FullyQualifiedName~Roundtrip"
```

### Ejecutar con cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Los reportes de cobertura se generan en la carpeta `TestResults/` gracias al paquete `coverlet.collector`.

---

## Catalogo de pruebas

### CipherUtils (4 pruebas)

| N | Prueba                                         | Tipo   | Descripcion                                                   |
|---|------------------------------------------------|--------|---------------------------------------------------------------|
| 1 | CipherUtils_PreserveCase_Works                 | Theory | Preserva mayuscula/minuscula del caracter original al resultado |
| 2 | CipherUtils_IsLetterSpanish_Works              | Theory | Identifica letras del alfabeto espanol (incluyendo la N con tilde) |
| 3 | CipherUtils_SpanishAlphabet_Has27Letters        | Fact   | El alfabeto tiene 27 letras, A en posicion 0, N con tilde en 14, Z en 26 |
| 4 | CipherUtils_GetSpanishIndex_Works              | Theory | Devuelve el indice correcto para cada letra                   |

### Morse (4 pruebas)

| N | Prueba                                         | Tipo   | Descripcion                                                   |
|---|------------------------------------------------|--------|---------------------------------------------------------------|
| 5 | Morse_Encrypt_SOS                              | Fact   | Cifra SOS correctamente                                       |
| 6 | Morse_Encrypt_WithSpaces_WordsSeparatedBySlash | Fact   | Las palabras se separan con /                                 |
| 7 | Morse_Decrypt_SOS                              | Fact   | Descifra SOS desde Morse                                      |
| 8 | Morse_Roundtrip                                | Fact   | Cifrar y descifrar devuelve el texto original                 |

### Numerica (6 pruebas)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 9  | Numeric_Encrypt_ABC                            | Fact   | Cifra ABC como 000102                                         |
| 10 | Numeric_Encrypt_N_IsIndex14                    | Fact   | La N con tilde se cifra como 14                               |
| 11 | Numeric_Encrypt_Z_IsIndex26                    | Fact   | Z se cifra como 26                                            |
| 12 | Numeric_Encrypt_PreservesSpaces                | Fact   | Los espacios se mantienen                                     |
| 13 | Numeric_Decrypt_Basic                          | Fact   | Descifra correctamente pares de digitos                       |
| 14 | Numeric_Roundtrip_HOLA                         | Fact   | Ida y vuelta con HOLA                                         |

### Celular / T9 (4 pruebas)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 15 | Cellphone_Encrypt_HOLA                         | Fact   | Cifra HOLA con formato T9                                     |
| 16 | Cellphone_Encrypt_SpaceEquals0                 | Fact   | El espacio se codifica como 0                                 |
| 17 | Cellphone_Decrypt_HOLA                         | Fact   | Descifra desde formato T9                                     |
| 18 | Cellphone_Roundtrip                            | Fact   | Ida y vuelta completa                                         |

### Cenit-Polar (4 pruebas)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 19 | CenitPolar_Encrypt_cenitToPolar                | Fact   | cenit se convierte en polar                                   |
| 20 | CenitPolar_IsSymmetric                         | Fact   | Cifrar dos veces devuelve el original                         |
| 21 | CenitPolar_PreservesCase                       | Fact   | Se mantiene mayuscula/minuscula                               |
| 22 | CenitPolar_OnlyChangesMapLetters_OthersStay    | Fact   | Caracteres fuera del mapa no cambian                          |

### Baden-Powell (3 pruebas)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 23 | BadenPowel_Encrypt_badenToPowel                | Fact   | baden se convierte en powel                                   |
| 24 | BadenPowel_IsSymmetric                         | Fact   | Cifrar dos veces devuelve el original                         |
| 25 | BadenPowel_PreservesUnmappedChars              | Fact   | Caracteres no mapeados se preservan                           |

### Murcielago (4 pruebas)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 26 | Murcielago_Encrypt_LettersToDigits             | Fact   | MURCIELAGO se cifra como 0123456789                           |
| 27 | Murcielago_Decrypt_DigitsToLetters             | Fact   | 0123456789 se descifra como MURCIELAGO                        |
| 28 | Murcielago_PreservesNonMappedLettersAndNumbers  | Fact   | Letras y numeros fuera del mapa se copian                     |
| 29 | Murcielago_Roundtrip                           | Fact   | Ida y vuelta completa                                         |

### Clave +1 (6 pruebas)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 30 | ShiftPlusOne_Encrypt_ABCtoBCD                  | Fact   | ABC se desplaza a BCD                                         |
| 31 | ShiftPlusOne_WrapAround_ZtoA                   | Fact   | Z vuelve a A (wrap around)                                    |
| 32 | ShiftPlusOne_Spanish_NtoN_NtoO                 | Fact   | N pasa a N con tilde, N con tilde pasa a O                    |
| 33 | ShiftPlusOne_PreservesCase                     | Fact   | Se preserva mayuscula/minuscula                               |
| 34 | ShiftPlusOne_PreservesNonLetters               | Fact   | Digitos y signos no se modifican                              |
| 35 | ShiftPlusOne_Roundtrip                         | Fact   | Cifrar con +1 y descifrar con +1 devuelve el original         |

### Clave -1 (4 pruebas)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 36 | ShiftMinusOne_Encrypt_BCDtoABC                 | Fact   | BCD retrocede a ABC                                           |
| 37 | ShiftMinusOne_WrapAround_AtoZ                  | Fact   | A vuelve a Z (wrap around)                                    |
| 38 | ShiftMinusOne_Spanish_NtoN_OtoN                | Fact   | N con tilde pasa a N, O pasa a N con tilde                    |
| 39 | ShiftMinusOne_Roundtrip                        | Fact   | Ida y vuelta completa                                         |

### Parelinofo (3 pruebas)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 40 | Parelinofo_Encrypt_parentToUfonet              | Fact   | parent se convierte en ufonet                                 |
| 41 | Parelinofo_IsSymmetric                         | Fact   | Cifrar dos veces devuelve el original                         |
| 42 | Parelinofo_PreservesUnmappedChars              | Fact   | Caracteres no mapeados se preservan                           |

### Dametupico (2 pruebas)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 43 | Dametupico_Encrypt_damepToOcipe                | Fact   | damep se convierte en ocipe                                   |
| 44 | Dametupico_IsSymmetric                         | Fact   | Cifrar dos veces devuelve el original                         |

### Agujerito (3 pruebas)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 45 | Agujerito_Encrypt                              | Fact   | Cifra correctamente con los pares definidos                   |
| 46 | Agujerito_IsSymmetric                          | Fact   | Cifrar dos veces devuelve el original                         |
| 47 | Agujerito_PreservesUnmappedChars               | Fact   | Caracteres no mapeados se preservan                           |

### Semaforo (8 pruebas)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 48 | Semaforo_Encrypt_HOLA                          | Fact   | Cifra HOLA con prefijo SEMAFORO:                              |
| 49 | Semaforo_Encrypt_WithSpaces                    | Fact   | Los espacios se mantienen entre claves                        |
| 50 | Semaforo_Encrypt_IgnoresN                      | Fact   | La N con tilde se ignora (no tiene simbolo)                   |
| 51 | Semaforo_Encrypt_EmptyInput                    | Fact   | Entrada vacia devuelve cadena vacia                           |
| 52 | Semaforo_Decrypt_Basic                         | Fact   | Descifra claves SEMAFORO: correctamente                       |
| 53 | Semaforo_Decrypt_WithSpaces                    | Fact   | Descifra con espacios intercalados                            |
| 54 | Semaforo_Decrypt_InvalidPrefix                 | Fact   | Prefijo invalido se copia tal cual                            |
| 55 | Semaforo_Roundtrip                             | Fact   | Cifrar y descifrar devuelve el texto original                 |

### CipherService (2 pruebas)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 56 | CipherService_Process_AllTypes_ShouldNotThrow  | Fact   | Ningun tipo de cifrado lanza excepcion al procesar            |
| 57 | CipherService_GetAvailableCiphers_Returns13    | Fact   | El servicio devuelve exactamente 13 cifrados disponibles       |

### Pruebas cruzadas (1 prueba parametrizada = 16 casos)

| N  | Prueba                                         | Tipo   | Descripcion                                                   |
|----|------------------------------------------------|--------|---------------------------------------------------------------|
| 58 | SymmetricCiphers_PreserveLength_And_Roundtrip  | Theory | 7 cifrados simetricos con varias entradas: valida longitud idemntica y roundtrip |

Los 7 cifrados simetricos validados son: CenitPolar, BadenPowel, ShiftPlusOne, ShiftMinusOne, Parelinofo, Dametupico, Agujerito.

---

## Estrategia de vinculacion de archivos

El `.csproj` utiliza `<Compile Include>` para referenciar directamente los archivos fuente del proyecto MAUI:

```xml
<ItemGroup>
    <Compile Include="..\ScoutCode\Ciphers\*.cs" Link="Ciphers\%(Filename)%(Extension)" />
    <Compile Include="..\ScoutCode\Services\CipherService.cs" Link="Services\CipherService.cs" />
    <Compile Include="..\ScoutCode\Services\ICipherService.cs" Link="Services\ICipherService.cs" />
    <Compile Include="..\ScoutCode\Models\CipherType.cs" Link="Models\CipherType.cs" />
    <Compile Include="..\ScoutCode\Models\CipherDefinition.cs" Link="Models\CipherDefinition.cs" />
    <Compile Include="..\ScoutCode\Models\OperationMode.cs" Link="Models\OperationMode.cs" />
</ItemGroup>
```

### Justificacion de esta estrategia

- **Evita dependencia de MAUI**: Un `ProjectReference` al proyecto MAUI requeriria tener instalados todos los workloads de MAUI para compilar las pruebas.
- **Compilacion rapida**: Solo se compilan los archivos necesarios (algoritmos, modelos, servicios), sin la capa de UI.
- **Portabilidad**: Las pruebas se pueden ejecutar en cualquier entorno con .NET 8, sin necesidad de SDKs de plataforma (Android, iOS, etc.).
- **Archivos vinculados**: Los archivos se compilan directamente desde su ubicacion original, manteniendo una unica fuente de verdad.

### Archivos vinculados

| Patron / Archivo                              | Carpeta virtual   | Contenido                              |
|-----------------------------------------------|-------------------|----------------------------------------|
| ..\ScoutCode\Ciphers\*.cs                     | Ciphers\          | Los 13 algoritmos + ICipherAlgorithm + CipherUtils |
| ..\ScoutCode\Services\CipherService.cs        | Services\         | Implementacion del servicio de cifrado |
| ..\ScoutCode\Services\ICipherService.cs       | Services\         | Interfaz del servicio                  |
| ..\ScoutCode\Models\CipherType.cs             | Models\           | Enumeracion de tipos                   |
| ..\ScoutCode\Models\CipherDefinition.cs       | Models\           | Modelo de definicion visual            |
| ..\ScoutCode\Models\OperationMode.cs          | Models\           | Enumeracion de operaciones             |

---

## Dependencias

| Paquete                          | Version | Proposito                                   |
|----------------------------------|---------|---------------------------------------------|
| Microsoft.NET.Test.Sdk           | 17.8.0  | Infraestructura de ejecucion de pruebas     |
| xunit                           | 2.6.6   | Framework de pruebas (Fact, Theory, Assert) |
| xunit.runner.visualstudio       | 2.5.6   | Runner para Visual Studio y dotnet test     |
| coverlet.collector               | 6.0.0   | Recoleccion de cobertura de codigo          |

---

## Convenciones de pruebas

### Nomenclatura

Todas las pruebas siguen el patron: `Componente_Accion_Resultado`

Ejemplos:
- `Morse_Encrypt_SOS` - Componente: Morse, Accion: Encrypt, Caso: SOS
- `CenitPolar_IsSymmetric` - Componente: CenitPolar, Validacion: simetria
- `CipherUtils_PreserveCase_Works` - Componente: CipherUtils, Funcion: PreserveCase

### Categorias de validacion

Cada algoritmo se prueba con las siguientes categorias cuando aplica:

1. **Cifrado basico**: Valida que una entrada conocida produzca la salida esperada.
2. **Descifrado basico**: Valida la operacion inversa.
3. **Roundtrip (ida y vuelta)**: `Decrypt(Encrypt(x)) == x` para todas las entradas.
4. **Simetria**: Para cifrados simetricos, `Encrypt(Encrypt(x)) == x`.
5. **Preservacion de case**: Verifica que mayusculas/minusculas se mantengan.
6. **Caracteres no mapeados**: Verifica que digitos, signos y espacios pasen sin modificar.
7. **Casos limite**: Entrada vacia, caracteres especiales, wrap around del alfabeto.

### Tipos de prueba xUnit utilizados

- **[Fact]**: Prueba individual con un caso fijo.
- **[Theory] + [InlineData]**: Prueba parametrizada con multiples casos de entrada.

---

## Guia para agregar pruebas

1. Abrir `CipherAlgorithmTests.cs`.
2. Agregar los metodos de prueba al final de la clase, agrupados por algoritmo.
3. Como minimo incluir:
   - Una prueba de cifrado con entrada/salida conocida.
   - Una prueba de descifrado.
   - Una prueba de roundtrip.
   - Si el cifrado es simetrico, una prueba de simetria.
4. Si se agrega un nuevo algoritmo al proyecto, tambien:
   - Agregar el archivo fuente al `<Compile Include>` del `.csproj` si no esta cubierto por el wildcard `*.cs`.
   - Actualizar la prueba `CipherService_GetAvailableCiphers_ReturnsN` con el nuevo total.
   - Agregar el nuevo tipo a la prueba `CipherService_Process_AllTypes_ShouldNotThrow`.
   - Si es simetrico, incluirlo en `SymmetricCiphers_PreserveLength_And_Roundtrip`.
