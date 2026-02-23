# ScoutCode

App de cifrados scout hecha en .NET MAUI 8. Funciona completamente offline. La idea es tener todas las claves que se usan en actividades scout en un solo lugar, sin depender de internet.

## Cifrados que incluye

| # | Cifrado | Que hace | Ejemplo |
|---|---------|----------|---------|
| 1 | Morse | Codigo Morse | `HOLA` → `.... --- .-.. .-` |
| 2 | Numerica | Cada letra se vuelve un numero de 2 digitos | `HOLA` → `07151100` |
| 3 | Celular | Teclado de telefono viejo (T9) | `HOLA` → `4^2-6^3-5^3-2` |
| 4 | Cenit-Polar | Intercambio C-P, E-O, N-L, I-A, T-R | `cenit` → `polar` |
| 5 | Baden-Powell | Intercambio B-P, A-O, D-W, N-L | `baden` → `powel` |
| 6 | Murcielago | Las letras de MURCIELAGO son los digitos 0-9 | `MURCIELAGO` → `0123456789` |
| 7 | Clave +1 | Cada letra se mueve una posicion adelante | `ABC` → `BCD` |
| 8 | Clave -1 | Cada letra se mueve una posicion atras | `BCD` → `ABC` |
| 9 | Parelinofo | Intercambio P-U, A-F, R-O, E-N, L-I | `parent` → `ufonet` |
| 10 | Dametupico | Intercambio D-O, A-C, M-I, E-P, T-U | `damep` → `ocipe` |
| 11 | Agujerito | Intercambio A-O, G-T, U-I, J-R | `agujerit` → `otirejug` |

Algunas cosas a tener en cuenta:
- Se mantiene mayuscula/minuscula: si escribis `Hola` con +1 sale `Ipñb`.
- Los caracteres que no estan en la clave (numeros, signos, acentos) se copian tal cual.
- Los cifrados +1 y -1 usan el alfabeto español de 27 letras (con la Ñ).

## Estructura del proyecto

```
ScoutCode/
├── Models/          Enums y definiciones (CipherType, OperationMode, etc.)
├── Ciphers/         Los 11 algoritmos + utilidades compartidas
├── Services/        CipherService que rutea al algoritmo correcto
├── Pipelines/       Placeholders para reconocimiento por camara (todavia no implementado)
├── ViewModels/      HomeViewModel y CipherDetailViewModel
├── Views/           Las dos pantallas de la app
├── Converters/      Converter para bindings
├── MauiProgram.cs   Configuracion de DI
└── AppShell.xaml    Navegacion
```

## Cifrados en detalle

### Morse
```
Cifrar:    HOLA MUNDO → .... --- .-.. .- / -- ..- -. -.. ---
Descifrar: .... --- .-.. .- → HOLA
Las letras se separan con espacio, las palabras con " / "
```

### Numerica
```
Cifrar:    HOLA → 07151100  (H=07, O=15, L=11, A=00)
Descifrar: 07151100 → HOLA  (lee de a pares de digitos)
Usa el alfabeto español: A=00, B=01 ... Ñ=14 ... Z=26
```

### Celular (T9)
```
Cifrar:    HOLA → 4^2-6^3-5^3-2
Descifrar: 4^2-6^3-5^3-2 → HOLA
El espacio es 0, los simbolos se separan con guion
```

### Cenit-Polar
```
CENIT <-> POLAR
Ejemplo: "Cenit" → "Polar"  |  "h2!ceni" → "h2!pola"
Es simetrico, asi que cifrar dos veces te da el texto original
```

### Baden-Powell
```
BADEN <-> POWEL (la E se queda igual)
Ejemplo: "baden" → "powel"  |  "powel" → "baden"
Lo que no esta en la clave se copia igual
```

### Murcielago
```
M=0  U=1  R=2  C=3  I=4  E=5  L=6  A=7  G=8  O=9
Ejemplo: "MURCIELAGO" → "0123456789"
         "hola" → "h967"  (la h no esta en la clave, se copia)
```

### Clave +1 / Clave -1
```
+1: cada letra se mueve a la siguiente (Z vuelve a A, N pasa a Ñ)
-1: cada letra se mueve a la anterior (A vuelve a Z, Ñ pasa a N)
Ejemplo: "abc" con +1 → "bcd"  |  "BCD" con -1 → "ABC"
```

### Parelinofo
```
PARELI <-> UFONEL (P-U, A-F, R-O, E-N, L-I)
Ejemplo: "parent" → "ufonet"
Simetrico
```

### Dametupico
```
DAMET <-> OCIPE (D-O, A-C, M-I, E-P, T-U)
Ejemplo: "damep" → "ocipe"
Simetrico
```

### Agujerito
```
AGUJER <-> OTIREJ (A-O, G-T, U-I, J-R, la E se queda)
Ejemplo: "agujerit" → "otirejug"
Simetrico
```

## Tests

Hay 65 pruebas en `ScoutCode.Tests` con xUnit. Cubren cada algoritmo, la simetria, que se mantengan mayusculas/minusculas, y que los caracteres que no son parte de la clave pasen sin cambios.

```bash
dotnet test ScoutCode.Tests/ScoutCode.Tests.csproj
```

## Agregar una clave nueva

1. Crear una clase en `Ciphers/` que implemente `ICipherAlgorithm`
2. Agregar un valor al enum `CipherType`
3. Registrarla en `CipherService` (constructor y `GetAvailableCiphers()`)

Con eso ya aparece en la pantalla principal.

## Compilar

```bash
# Android
dotnet build -f net8.0-android

# Correr en emulador
dotnet build -f net8.0-android -t:Run

# Tests
dotnet test ScoutCode.Tests/ScoutCode.Tests.csproj
```

## Notas

- No necesita internet
- El alfabeto español con Ñ se usa en las claves +1, -1 y Numerica
- Los cifrados simetricos (Cenit-Polar, Baden-Powell, Parelinofo, Dametupico, Agujerito) usan la misma operacion para cifrar y descifrar
- El modulo de camara esta preparado pero todavia no hace nada real, es un placeholder para cuando se integre OpenCV y ONNX

## Licencia

Proyecto educativo para actividades scout.
