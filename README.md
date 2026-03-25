# ScoutCode

Aplicacion movil multiplataforma para cifrado y descifrado de mensajes utilizando algoritmos clasicos del movimiento scout. Desarrollada con .NET MAUI 8.0, funciona completamente offline en Android, iOS, macOS y Windows.

ScoutCode permite a scouts, guias y educadores practicar criptografia basica de campo: desde sustituciones de letras y codigo Morse hasta cifrados simbolicos como el Gato (Pigpen), el Semaforo de banderas y la Electrica. Incluye reconocimiento por camara tanto para texto impreso o manuscrito (via OCR) como para simbolos graficos (via template matching con SkiaSharp).

---

## Tabla de contenidos

1. [Caracteristicas principales](#caracteristicas-principales)
2. [Plataformas soportadas](#plataformas-soportadas)
3. [Arquitectura del proyecto](#arquitectura-del-proyecto)
4. [Algoritmos de cifrado](#algoritmos-de-cifrado)
5. [Reconocimiento por camara](#reconocimiento-por-camara)
6. [Estructura del repositorio](#estructura-del-repositorio)
7. [Requisitos previos](#requisitos-previos)
8. [Compilacion y ejecucion](#compilacion-y-ejecucion)
9. [Ejecucion de pruebas](#ejecucion-de-pruebas)
10. [Dependencias](#dependencias)
11. [Guia para agregar un nuevo cifrado](#guia-para-agregar-un-nuevo-cifrado)
12. [Convenciones del proyecto](#convenciones-del-proyecto)
13. [Licencia](#licencia)

---

## Caracteristicas principales

- **14 algoritmos de cifrado** organizados en tres categorias: codificacion, sustitucion simetrica y simbolicos.
- **Soporte completo para el alfabeto espanol** de 27 letras (A-Z incluyendo la Enie), con preservacion de mayusculas y minusculas.
- **Reconocimiento por camara** con dos modos de procesamiento:
  - OCR (Google ML Kit en Android, Vision framework en iOS) para cifrados de texto.
  - Template matching (SkiaSharp) para cifrados simbolicos (Gato, Semaforo, Electrica).
- **Normalizacion inteligente de Morse**: postprocesamiento del OCR que corrige caracteres confundidos (comas por puntos, guiones largos por rayas, pipes por barras).
- **Cifrados simbolicos con rendering visual**: los cifrados Gato (Pigpen), Semaforo y Electrica muestran imagenes de cada simbolo, con grillas interactivas para seleccion manual.
- **Funcionamiento 100% offline**: todo el procesamiento se realiza localmente, sin conexion a internet.
- **Interfaz tematica scout**: paleta de colores inspirada en la naturaleza (azul montania, verde bosque, ambar), con drawables personalizados de paisajes de montana.
- **Arquitectura limpia MVVM** con inyeccion de dependencias, Strategy Pattern para los cifrados y 81 pruebas unitarias.

---

## Plataformas soportadas

| Plataforma     | Version minima            | OCR texto | Reconocimiento simbolos |
|----------------|---------------------------|-----------|-------------------------|
| Android        | API 21 (Android 5.0)      | Si        | Si                      |
| iOS            | 11.0                      | Si        | Si                      |
| macOS Catalyst | 13.1                      | No        | Si                      |
| Windows        | 10.0.17763.0              | No        | Si                      |

El OCR de texto utiliza servicios nativos de cada plataforma (ML Kit en Android, Vision en iOS). El reconocimiento de simbolos funciona en todas las plataformas ya que utiliza SkiaSharp para el procesamiento de imagenes y firmas binarias pre-computadas para el matching.

En las plataformas sin OCR de texto se registra un `UnsupportedTextRecognizer` que informa al usuario de la limitacion. La camara sigue siendo utilizable para los cifrados simbolicos.

---

## Arquitectura del proyecto

### Vista general

```
ScoutCode.sln
|
|-- ScoutCode/              Aplicacion .NET MAUI (proyecto principal)
|   |-- Ciphers/            14 algoritmos de cifrado (Strategy Pattern)
|   |-- Models/             Modelos de datos y enumeraciones
|   |-- Services/           Servicios de cifrado y OCR
|   |-- ViewModels/         ViewModels con CommunityToolkit.Mvvm
|   |-- Views/              Paginas XAML y code-behind
|   |-- Controls/           Drawables personalizados
|   |-- Converters/         Convertidores de valor XAML
|   |-- Pipelines/          Pipeline de reconocimiento de simbolos
|   |-- Platforms/          Codigo nativo por plataforma
|   |-- Resources/          Iconos, fuentes, imagenes y estilos
|
|-- ScoutCode.Tests/        Proyecto de pruebas unitarias (xUnit, 81 tests)
```

### Patrones de diseno

| Patron               | Aplicacion                                                                   |
|----------------------|------------------------------------------------------------------------------|
| Strategy             | `ICipherAlgorithm` con 14 implementaciones intercambiables                   |
| MVVM                 | ViewModels con `[ObservableProperty]` y `[RelayCommand]` (source generators) |
| Dependency Injection | Registro en `MauiProgram.cs` via `builder.Services`                          |
| Service Locator      | `CipherService` resuelve el algoritmo correcto a partir de `CipherType`      |
| Shell Navigation     | Rutas con `[QueryProperty]` para paso de parametros entre paginas            |
| Template Method      | Pipeline de camara con pasos intercambiables (OCR vs template matching)       |

### Capas de la aplicacion

```
Vista (XAML)
    |
ViewModel (CommunityToolkit.Mvvm)
    |
Servicios (CipherService, ITextRecognitionService, ICameraPipeline)
    |
Algoritmos (ICipherAlgorithm x 14)
```

Los ViewModels reciben los servicios por inyeccion de constructor. El `CipherService` actua como fachada que delega al algoritmo correspondiente segun el `CipherType` recibido. El pipeline de camara se bifurca segun el tipo de cifrado: texto por OCR nativo, simbolos por template matching con SkiaSharp.

### Registro de servicios (MauiProgram.cs)

| Servicio                 | Implementacion               | Lifetime  | Descripcion                                    |
|--------------------------|------------------------------|-----------|------------------------------------------------|
| ICipherService           | CipherService                | Singleton | Servicio central de cifrado/descifrado         |
| ICameraPipeline          | SkiaSharpSymbolPipeline      | Singleton | Pipeline de reconocimiento de simbolos         |
| IImageSegmenter          | PlaceholderImageSegmenter    | Singleton | Segmentacion de imagen (interfaz futura)       |
| ISymbolClassifier        | PlaceholderSymbolClassifier  | Singleton | Clasificacion de simbolos (interfaz futura)    |
| ITextRecognitionService  | (plataforma especifica)      | Singleton | OCR nativo condicional por plataforma          |
| HomeViewModel            | HomeViewModel                | Transient | ViewModel de la pagina principal               |
| CipherDetailViewModel    | CipherDetailViewModel        | Transient | ViewModel de detalle y procesamiento           |

---

## Algoritmos de cifrado

La aplicacion incluye 14 algoritmos organizados en tres categorias.

### Cifrados de codificacion

Transforman cada caracter en una representacion numerica o simbolica distinta.

| N  | Nombre         | Mecanismo                                                       | Ejemplo                         |
|----|----------------|-----------------------------------------------------------------|---------------------------------|
| 1  | Morse          | Codigo Morse internacional con puntos y rayas                   | `SOS` -> `... --- ...`         |
| 2  | Numerica       | Indice de 2 digitos en el alfabeto espanol (A=00 ... Z=26)     | `HOLA` -> `07151100`           |
| 3  | Celular (T9)   | Teclado telefonico con tecla y repeticiones (espacio=0)         | `HOLA` -> `4^2-6^3-5^3-2`     |

### Cifrados de sustitucion simetrica

Intercambian pares de letras. La misma operacion cifra y descifra.

| N  | Nombre       | Pares de intercambio               | Ejemplo                  |
|----|--------------|-------------------------------------|--------------------------|
| 4  | Cenit-Polar  | C-P, E-O, N-L, I-A, T-R            | `cenit` -> `polar`       |
| 5  | Baden-Powell | B-P, A-O, D-W, E=E, N-L            | `baden` -> `powel`       |
| 6  | Murcielago   | M=0, U=1, R=2, C=3, I=4, E=5, L=6, A=7, G=8, O=9 | `MURCIELAGO` -> `0123456789` |
| 7  | Clave +1     | Desplazamiento +1 posicion (Z->A, incluye Enie)    | `abc` -> `bcd`           |
| 8  | Clave -1     | Desplazamiento -1 posicion (A->Z, incluye Enie)    | `bcd` -> `abc`           |
| 9  | Parelinofo   | P-U, A-F, R-O, E-N, L-I            | `parent` -> `ufonet`     |
| 10 | Dametupico   | D-O, A-C, M-I, E-P, T-U            | `damep` -> `ocipe`       |
| 11 | Agujerito    | A-O, G-T, U-I, J-R, E=E            | `agujerit` -> `otirejug` |

### Cifrados simbolicos

Cada letra se convierte en una imagen grafica. La salida intermedia tiene formato `PREFIJO:clave1,clave2,...` que la interfaz interpreta para renderizar las imagenes correspondientes.

| N  | Nombre           | Conjunto de simbolos                                     | Imagenes              |
|----|------------------|----------------------------------------------------------|-----------------------|
| 12 | Gato (Pigpen)    | Cuadriculas y aspas con/sin punto (A-Z + Enie = 27)     | `Resources/Images/Gato/`      |
| 13 | Semaforo         | Posiciones de banderas de semaforo (A-Z = 26)            | `Resources/Images/Semaforo/`  |
| 14 | Electrica        | Simbolos en lineas electricas (A-Z + espacio = 27)       | `Resources/Images/Electrica/` |

### Comportamiento comun

- Todos los cifrados de texto soportan el alfabeto espanol completo (27 letras, incluyendo la Enie).
- Se preserva la casing original: si el caracter de entrada es minuscula, la salida tambien lo es.
- Los caracteres fuera del dominio de la clave (numeros, signos de puntuacion) se copian sin modificar.
- Los cifrados simetricos usan la misma logica para cifrar y descifrar.
- Cada algoritmo implementa la interfaz `ICipherAlgorithm` con los metodos `Encrypt`, `Decrypt`, `SupportedCharacters` y `DisplayName`.

---

## Reconocimiento por camara

La aplicacion ofrece dos motores de reconocimiento segun el tipo de cifrado.

### OCR para cifrados de texto

Utiliza servicios nativos de la plataforma para extraer texto de fotografias:

- **Android**: Google ML Kit Text Recognition con el modelo latino (`Xamarin.Google.MLKit.TextRecognition`).
- **iOS**: Framework Vision de Apple con `VNRecognizeTextRequest` en modo preciso.

El texto reconocido se muestra al usuario para revision y edicion antes de descifrar.

#### Normalizacion para Morse

El OCR tiene dificultades con los puntos y rayas del codigo Morse. La aplicacion aplica una capa de postprocesamiento (`CipherUtils.NormalizeMorseOcr`) que corrige automaticamente las confusiones mas comunes:

| Caracter del OCR            | Caracter corregido | Ejemplo          |
|-----------------------------|---------------------|------------------|
| `,` `*` `middle dot` `bullet` | `.` (punto)       | `,,,` -> `...`   |
| `em-dash` `en-dash` `_` `~`  | `-` (raya)        | `---` -> `---`   |
| `pipe` `backslash`           | `/` (separador)    | `...|---` -> `... / ---` |

Ademas, normaliza los espacios multiples y estandariza el formato de separacion de palabras a ` / `.

### Template matching para cifrados simbolicos

Para los cifrados Gato, Semaforo y Electrica, el OCR de texto no es util ya que los simbolos no son caracteres reconocibles. En su lugar, la aplicacion utiliza un pipeline de template matching implementado con SkiaSharp (`SkiaSharpSymbolPipeline`).

#### Flujo del pipeline

1. **Decodificacion**: La imagen de la camara se decodifica a un bitmap usando `SKBitmap.Decode`.
2. **Preprocesamiento**: Conversion a escala de grises y binarizacion con el metodo de Otsu para separar simbolos del fondo.
3. **Segmentacion**: Analisis de proyecciones horizontales (para detectar lineas de texto) y verticales (para detectar simbolos individuales dentro de cada linea). Los gaps entre simbolos mayores a 1.8 veces el ancho promedio se interpretan como espacios entre palabras.
4. **Normalizacion**: Cada simbolo segmentado se recorta, se rellena para mantener la proporcion de aspecto cuadrada, y se redimensiona a 48x48 pixeles con area-averaging.
5. **Matching**: La firma binaria del simbolo se compara contra las plantillas pre-computadas de ese cifrado usando la distancia de Hamming. Un pre-filtro por densidad de pixeles (foreground count) descarta rapidamente las plantillas claramente incompatibles.
6. **Resultado**: Las claves reconocidas se formatean como cadena intermedia (ej. `GATO:h,o,l,a`) y se descifran automaticamente. El usuario ve el texto plano directamente.

#### Plantillas pre-computadas (TemplateData.cs)

Las firmas de plantilla se generan offline a partir de las 80 imagenes PNG originales (300x300) mediante un script Python que:

1. Decodifica cada PNG con decodificador puro (sin dependencias externas).
2. Convierte a escala de grises, redimensiona a 48x48 con area-averaging.
3. Binariza con umbral de Otsu.
4. Empaqueta en 288 bytes (6 bytes por fila, 48 filas).
5. Registra la cantidad de pixeles foreground para pre-filtrado rapido.

Las firmas estan embebidas como constantes en el codigo C# (`Pipelines/TemplateData.cs`), eliminando cualquier necesidad de carga de archivos en tiempo de ejecucion.

**Estadisticas de discriminacion**:

| Cifrado    | Plantillas | Min. distancia Hamming | Pares unicos |
|------------|------------|------------------------|--------------|
| Gato       | 27         | 20 / 2304              | 27 / 27      |
| Semaforo   | 26         | 32 / 2304              | 26 / 26      |
| Electrica  | 27         | 50 / 2304              | 27 / 27      |

Todas las plantillas son completamente distinguibles entre si en el espacio de firmas 48x48.

---

## Estructura del repositorio

```
ScoutCode/
|-- README.md                              Este archivo
|-- ScoutCode.sln                          Solucion principal
|
|-- ScoutCode/                             Proyecto MAUI principal
|   |-- App.xaml / App.xaml.cs             Punto de entrada, recursos globales
|   |-- AppShell.xaml / AppShell.xaml.cs   Shell de navegacion y registro de rutas
|   |-- MainPage.xaml / MainPage.xaml.cs   Pagina inicial (redirige a HomePage)
|   |-- MauiProgram.cs                     Configuracion de DI, fuentes y servicios
|   |-- ScoutCode.csproj                   Definicion del proyecto y dependencias NuGet
|   |
|   |-- Ciphers/                           Algoritmos de cifrado
|   |   |-- ICipherAlgorithm.cs            Interfaz base (Strategy Pattern)
|   |   |-- CipherUtils.cs                 Utilidades: alfabeto espanol, preservacion
|   |   |                                  de casing, normalizacion OCR para Morse,
|   |   |                                  mapeo de caracteres
|   |   |-- MorseCipherAlgorithm.cs        Codigo Morse internacional
|   |   |-- NumericCipherAlgorithm.cs      Cifrado numerico (A=00...Z=26)
|   |   |-- CellphoneCipherAlgorithm.cs    Cifrado tipo teclado T9
|   |   |-- CenitPolarCipherAlgorithm.cs   Sustitucion simetrica Cenit-Polar
|   |   |-- BadenPowelCipherAlgorithm.cs   Sustitucion simetrica Baden-Powell
|   |   |-- MurcielagoCipherAlgorithm.cs   Sustitucion MURCIELAGO -> 0123456789
|   |   |-- ShiftPlusOneCipherAlgorithm.cs Desplazamiento +1 (Cesar)
|   |   |-- ShiftMinusOneCipherAlgorithm.cs Desplazamiento -1 (Cesar inverso)
|   |   |-- ParelinofoCipherAlgorithm.cs   Sustitucion simetrica Parelinofo
|   |   |-- DametupicoCipherAlgorithm.cs   Sustitucion simetrica Dametupico
|   |   |-- AgujeritoCipherAlgorithm.cs    Sustitucion simetrica Agujerito
|   |   |-- GatoCipherAlgorithm.cs         Cifrado simbolico Gato (Pigpen)
|   |   |-- SemaforoCipherAlgorithm.cs     Cifrado simbolico Semaforo
|   |   |-- ElectricaCipherAlgorithm.cs    Cifrado simbolico Electrica
|   |
|   |-- Models/                            Modelos de datos
|   |   |-- CipherType.cs                  Enumeracion de 14 tipos de cifrado
|   |   |-- CipherDefinition.cs            Definicion visual: nombre, icono, color
|   |   |-- OperationMode.cs               Enum: Encrypt, Decrypt
|   |   |-- InputMode.cs                   Enum: Manual, Camera
|   |   |-- GatoSymbolViewModel.cs         Modelo para simbolos graficos (key, imagen)
|   |
|   |-- Services/                          Capa de servicios
|   |   |-- ICipherService.cs              Interfaz del servicio de cifrado
|   |   |-- CipherService.cs               Implementacion: diccionario de algoritmos,
|   |   |                                  lista de cifrados disponibles con colores
|   |   |-- ITextRecognitionService.cs     Interfaz OCR multiplataforma
|   |
|   |-- ViewModels/                        ViewModels (MVVM)
|   |   |-- HomeViewModel.cs               Carga lista de cifrados, navega a detalle
|   |   |-- CipherDetailViewModel.cs       Logica principal: cifrado/descifrado manual,
|   |                                      grilla de simbolos, camara con bifurcacion
|   |                                      OCR/pipeline, copiar al portapapeles
|   |
|   |-- Views/                             Vistas XAML
|   |   |-- HomePage.xaml                  Catalogo de cifrados con entrada animada
|   |   |-- CipherDetailPage.xaml          Detalle: tabs manual/camara, grilla simbolica
|   |   |-- CameraSectionView.xaml         Componente reutilizable de camara y OCR
|   |
|   |-- Controls/                          Controles graficos personalizados
|   |   |-- MountainDrawable.cs            Paisaje de tres montanas con gradiente y niebla
|   |   |-- WaveDrawable.cs                Efecto de ola con curvas Bezier
|   |
|   |-- Converters/                        Convertidores de valor XAML
|   |   |-- BoolToColorConverter.cs        bool -> Color (con parametro)
|   |   |-- HexToColorConverter.cs         string hex (#RRGGBB) -> Color
|   |   |-- InvertedBoolConverter.cs       Negacion logica
|   |   |-- StringNotEmptyConverter.cs     Presencia de cadena -> bool
|   |
|   |-- Pipelines/                         Pipeline de reconocimiento de simbolos
|   |   |-- ICameraPipeline.cs             Interfaz con ProcessImageAsync y
|   |   |                                  ProcessSymbolicImageAsync
|   |   |-- SkiaSharpSymbolPipeline.cs     Implementacion completa: preprocesamiento,
|   |   |                                  segmentacion por proyecciones, template
|   |   |                                  matching con distancia Hamming
|   |   |-- TemplateData.cs                Firmas binarias 48x48 pre-computadas
|   |   |                                  (80 plantillas, generadas por script Python)
|   |   |-- PlaceholderCameraPipeline.cs   Fallback para plataformas sin soporte
|   |   |-- IImageSegmenter.cs             Interfaz para segmentacion (futura)
|   |   |-- ISymbolClassifier.cs           Interfaz para clasificacion (futura)
|   |   |-- PlaceholderImageSegmenter.cs   Implementacion temporal
|   |   |-- PlaceholderSymbolClassifier.cs Implementacion temporal
|   |
|   |-- Platforms/                         Codigo nativo por plataforma
|   |   |-- Android/
|   |   |   |-- MainActivity.cs
|   |   |   |-- MainApplication.cs
|   |   |   |-- AndroidManifest.xml        Permisos: CAMERA, INTERNET, storage
|   |   |   |-- Services/
|   |   |       |-- MlKitTextRecognizer.cs OCR con Google ML Kit (Latin)
|   |   |
|   |   |-- iOS/
|   |   |   |-- AppDelegate.cs
|   |   |   |-- Program.cs
|   |   |   |-- Info.plist                 Permisos de camara y galeria
|   |   |   |-- Services/
|   |   |       |-- VisionTextRecognizer.cs OCR con Vision framework
|   |   |
|   |   |-- MacCatalyst/
|   |   |   |-- Services/
|   |   |       |-- UnsupportedTextRecognizer.cs
|   |   |
|   |   |-- Windows/
|   |   |   |-- Services/
|   |   |       |-- UnsupportedTextRecognizer.cs
|   |   |
|   |   |-- Tizen/
|   |       |-- Main.cs
|   |
|   |-- Resources/                         Recursos de la aplicacion
|       |-- AppIcon/                       Icono SVG con foreground PNG
|       |-- Fonts/                         8 fuentes: Inter (5 pesos),
|       |                                  OpenSans (2 pesos), JetBrainsMono
|       |-- Images/
|       |   |-- Gato/                      27 PNG (300x300): gato_a..z + gato_enie
|       |   |-- Semaforo/                  26 PNG (300x300): semaforo_a..z
|       |   |-- Electrica/                 27 PNG (300x300): electrica_a..z + electrica_space
|       |-- Splash/                        Pantalla de carga SVG
|       |-- Styles/
|           |-- Colors.xaml                Paleta: Mountain Blue #34657f,
|           |                              Mountain Green #4a7a4e, Amber #d4943c
|           |-- Styles.xaml                Estilos globales de la aplicacion
|
|-- ScoutCode.Tests/                       Proyecto de pruebas unitarias
    |-- CipherAlgorithmTests.cs            81 pruebas (xUnit)
    |-- ScoutCode.Tests.csproj             net8.0, xUnit 2.6.6
```

---

## Requisitos previos

- **.NET 8 SDK** (version 8.0 o superior).
- **Visual Studio 2022** (version 17.8 o superior) con la carga de trabajo ".NET Multi-platform App UI development" instalada.
- **Android**: Android SDK con API 21 o superior.
- **iOS / macOS**: Xcode 15 o superior (requiere macOS como host de compilacion).
- **Windows**: Windows 10 SDK (10.0.17763.0 o superior).

---

## Compilacion y ejecucion

### Restaurar dependencias

```bash
dotnet restore ScoutCode.sln
```

### Compilar

```bash
dotnet build ScoutCode.sln -c Debug
```

### Ejecutar en una plataforma especifica

```bash
# Android (requiere emulador o dispositivo conectado)
dotnet build ScoutCode/ScoutCode.csproj -t:Run -f net8.0-android

# Windows
dotnet build ScoutCode/ScoutCode.csproj -t:Run -f net8.0-windows10.0.19041.0

# iOS (requiere macOS con Xcode)
dotnet build ScoutCode/ScoutCode.csproj -t:Run -f net8.0-ios

# macOS Catalyst (requiere macOS)
dotnet build ScoutCode/ScoutCode.csproj -t:Run -f net8.0-maccatalyst
```

---

## Ejecucion de pruebas

El proyecto de pruebas utiliza xUnit y se ejecuta sobre .NET 8 estandar (sin dependencias de MAUI). Contiene **81 pruebas** que cubren:

- Utilidades compartidas (`CipherUtils`): alfabeto espanol, preservacion de casing, indices.
- Normalizacion OCR para Morse: correccion de puntos, rayas y separadores.
- Los 14 algoritmos de cifrado: cifrado, descifrado, ida y vuelta (roundtrip).
- Cifrados simetricos: verificacion de longitud preservada y reversibilidad.
- Cifrados simbolicos (Semaforo y Electrica): formato de salida, prefijos y roundtrip.
- Servicio central (`CipherService`): procesamiento de todos los tipos y conteo de cifrados.

```bash
dotnet test ScoutCode.Tests/ScoutCode.Tests.csproj -c Debug --verbosity normal
```

### Cobertura por componente

| Componente            | Pruebas | Descripcion                                              |
|-----------------------|---------|----------------------------------------------------------|
| CipherUtils           | 7       | Alfabeto, indices, preservacion de casing                |
| Normalizacion Morse   | 7       | Puntos, rayas, separadores, roundtrip                    |
| Morse                 | 4       | Cifrado, descifrado, espacios, roundtrip                 |
| Numerica              | 6       | Digitos, Enie, espacios, roundtrip                       |
| Celular (T9)          | 4       | Mapeo, espacio=0, roundtrip                              |
| Cenit-Polar           | 4       | Simetria, preservacion de casing, no mapeados            |
| Baden-Powell          | 3       | Simetria, preservacion, no mapeados                      |
| Murcielago            | 4       | Letras a digitos, digitos a letras, roundtrip            |
| Clave +1              | 6       | Desplazamiento, wrap Z->A, Enie, casing, roundtrip      |
| Clave -1              | 4       | Desplazamiento inverso, wrap A->Z, Enie, roundtrip      |
| Parelinofo            | 3       | Simetria, roundtrip, no mapeados                         |
| Dametupico            | 2       | Pares, simetria                                          |
| Agujerito             | 3       | Cifrado, simetria, no mapeados                           |
| Semaforo              | 8       | Cifrado, descifrado, espacios, Enie, prefijo, roundtrip |
| Electrica             | 8       | Cifrado, descifrado, espacios, Enie, prefijo, roundtrip |
| CipherService         | 2       | Procesamiento de todos los tipos, conteo = 14            |
| Simetricos (global)   | 4       | Longitud preservada y roundtrip para 7 cifrados          |
| **Total**             | **81**  |                                                          |

---

## Dependencias

### Proyecto principal (ScoutCode)

| Paquete                                         | Version      | Proposito                                    |
|-------------------------------------------------|--------------|----------------------------------------------|
| Microsoft.Maui.Controls                         | 8.0.x        | Framework UI multiplataforma                 |
| Microsoft.Maui.Controls.Compatibility           | 8.0.x        | Compatibilidad con controles legacy          |
| Microsoft.Extensions.Logging.Debug              | 8.0.1        | Logging en modo debug                        |
| CommunityToolkit.Mvvm                           | 8.2.2        | MVVM con source generators                   |
| SkiaSharp                                       | 2.88.8       | Procesamiento de imagenes para template matching |
| Xamarin.Google.MLKit.TextRecognition            | 116.0.0.9    | OCR en Android (solo Android)                |
| Xamarin.Google.MLKit.TextRecognition.Latin       | 116.0.0.9    | Modelo de texto latino (solo Android)        |
| Xamarin.AndroidX.Collection.Ktx                 | 1.4.3.1      | Compatibilidad AndroidX (solo Android)       |
| Xamarin.AndroidX.Lifecycle.Runtime.Ktx          | 2.8.5.1      | Compatibilidad AndroidX (solo Android)       |
| Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx    | 2.8.5.1      | Compatibilidad AndroidX (solo Android)       |
| Xamarin.AndroidX.Lifecycle.ViewModel.Ktx        | 2.8.5.1      | Compatibilidad AndroidX (solo Android)       |
| Xamarin.AndroidX.Activity.Ktx                   | 1.9.2.1      | Compatibilidad AndroidX (solo Android)       |
| Xamarin.AndroidX.Fragment.Ktx                   | 1.8.3.1      | Compatibilidad AndroidX (solo Android)       |

### Proyecto de pruebas (ScoutCode.Tests)

| Paquete                          | Version | Proposito                             |
|----------------------------------|---------|---------------------------------------|
| Microsoft.NET.Test.Sdk           | 17.8.0  | Infraestructura de pruebas            |
| xunit                            | 2.6.6   | Framework de pruebas unitarias        |
| xunit.runner.visualstudio        | 2.5.6   | Integrador con Visual Studio          |
| coverlet.collector               | 6.0.0   | Recoleccion de cobertura de codigo    |

---

## Guia para agregar un nuevo cifrado

### Cifrado de texto

1. **Crear el algoritmo**: Agregar una clase en `Ciphers/` que implemente `ICipherAlgorithm`. Utilizar `CipherUtils.SpanishAlphabet`, `CipherUtils.ApplyCharMap` y `CipherUtils.PreserveCase` para mantener consistencia con el resto de la aplicacion.

2. **Registrar el tipo**: Agregar un nuevo valor en la enumeracion `CipherType` (`Models/CipherType.cs`).

3. **Registrar en el servicio**: En `Services/CipherService.cs`:
   - Agregar la instancia al diccionario `_algorithms` en el constructor.
   - Agregar una nueva `CipherDefinition` en `GetAvailableCiphers()` con nombre, descripcion, tipo, icono (2 caracteres) y flag de disponibilidad.

4. **Escribir pruebas**: Agregar pruebas unitarias en `ScoutCode.Tests/CipherAlgorithmTests.cs`. Como minimo incluir: cifrado basico, descifrado basico, ida y vuelta (roundtrip), preservacion de mayusculas/minusculas, y comportamiento con caracteres no soportados. Actualizar la prueba `CipherService_GetAvailableCiphers_ReturnsN` con el nuevo total.

### Cifrado simbolico

Ademas de los pasos anteriores:

5. **Agregar imagenes**: Colocar los archivos PNG en `Resources/Images/NombreCifrado/` con el patron de nombres `nombrecifrado_clave.png`. Las imagenes deben ser de 300x300 pixeles para consistencia con las existentes.

6. **Actualizar el proyecto**: Agregar una entrada `<MauiImage Include="Resources\Images\NombreCifrado\*.png" Resize="False" />` en el `.csproj`.

7. **Actualizar el ViewModel**: En `CipherDetailViewModel.cs`:
   - Agregar el nuevo tipo a la condicion `IsSymbolicCipher`.
   - Expandir `LoadSymbolicSymbols()` con la logica de carga de imagenes.
   - Agregar los casos necesarios en `ProcessGatoEncrypt()` y `ProcessGatoDecrypt()`.
   - Agregar el caso en `IsOcrCompatibleCipher()` (retornar `false` para simbolicos).

8. **Generar plantillas**: Ejecutar el script de generacion de firmas (`/tmp/gen_v3.py` o equivalente) para incluir las nuevas plantillas en `TemplateData.cs`. Verificar que las firmas sean unicas y que la distancia minima de Hamming sea suficiente (recomendado: mayor a 15).

---

## Convenciones del proyecto

- **Idioma del codigo fuente**: espanol para nombres de variables, comentarios e interfaz de usuario.
- **Alfabeto base**: espanol de 27 letras (A-Z incluyendo la Enie en la posicion 14).
- **Reversibilidad**: cada algoritmo debe cumplir `Decrypt(Encrypt(x)) == x` para todas las entradas validas.
- **Preservacion de caracteres**: los caracteres fuera del dominio del cifrado (numeros, signos) se copian sin modificar; nunca se descartan silenciosamente.
- **Preservacion de casing**: si la entrada es minuscula, la salida debe ser minuscula; idem para mayusculas.
- **Formato de simbolicos**: la salida intermedia de cifrados simbolicos sigue el patron `PREFIJO:clave1,clave2,...` donde los espacios se representan como `" "` (Gato, Semaforo) o `"space"` (Electrica).
- **Pruebas obligatorias**: cada nuevo algoritmo debe incluir pruebas de cifrado, descifrado, roundtrip y manejo de caracteres especiales antes de ser integrado.

---

## Licencia

Este proyecto esta licenciado bajo los terminos incluidos en el archivo [LICENSE](ScoutCode/LICENSE).
