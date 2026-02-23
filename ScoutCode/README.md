# ScoutCode - Proyecto MAUI

Proyecto .NET MAUI 8 que implementa la aplicacion multiplataforma de cifrados scouts. Este documento detalla la arquitectura interna, la organizacion de carpetas, cada modulo y las instrucciones para extender la aplicacion.

---

## Tabla de contenidos

1. [Estructura de carpetas](#estructura-de-carpetas)
2. [Ciphers - Algoritmos de cifrado](#ciphers---algoritmos-de-cifrado)
3. [Models - Modelos de datos](#models---modelos-de-datos)
4. [Services - Capa de servicios](#services---capa-de-servicios)
5. [ViewModels - Logica de presentacion](#viewmodels---logica-de-presentacion)
6. [Views - Interfaz de usuario](#views---interfaz-de-usuario)
7. [Controls - Controles graficos](#controls---controles-graficos)
8. [Converters - Convertidores XAML](#converters---convertidores-xaml)
9. [Pipelines - Integracion de vision](#pipelines---integracion-de-vision)
10. [Platforms - Codigo nativo](#platforms---codigo-nativo)
11. [Resources - Recursos de la aplicacion](#resources---recursos-de-la-aplicacion)
12. [Configuracion y punto de entrada](#configuracion-y-punto-de-entrada)
13. [Dependencias NuGet](#dependencias-nuget)
14. [Guia para agregar un nuevo cifrado](#guia-para-agregar-un-nuevo-cifrado)

---

## Estructura de carpetas

```
ScoutCode/
|-- App.xaml(.cs)               Punto de entrada, configuracion global
|-- AppShell.xaml(.cs)          Shell de navegacion y registro de rutas
|-- MainPage.xaml(.cs)          Pagina inicial (redirige a HomePage)
|-- MauiProgram.cs              Builder de la aplicacion, DI y servicios
|-- ScoutCode.csproj            Definicion del proyecto y dependencias
|
|-- Ciphers/                    13 algoritmos + utilidades
|-- Models/                     Enumeraciones y modelos de datos
|-- Services/                   CipherService + OCR
|-- ViewModels/                 HomeViewModel + CipherDetailViewModel
|-- Views/                      3 vistas XAML
|-- Controls/                   2 drawables personalizados
|-- Converters/                 4 convertidores de valor
|-- Pipelines/                  3 interfaces + 3 implementaciones placeholder
|-- Platforms/                  Codigo nativo (Android, iOS, Mac, Windows, Tizen)
|-- Resources/                  Iconos, fuentes, imagenes, estilos
```

---

## Ciphers - Algoritmos de cifrado

Carpeta que contiene los 13 algoritmos de cifrado, todos implementando la interfaz `ICipherAlgorithm` (Strategy Pattern).

### Interfaz base

```csharp
public interface ICipherAlgorithm
{
    string Encrypt(string input);
    string Decrypt(string input);
    string SupportedCharacters { get; }
    string DisplayName { get; }
}
```

### Utilidades compartidas (CipherUtils.cs)

Clase estatica que provee funcionalidad comun a todos los cifrados:

- **SpanishAlphabet**: Cadena con las 27 letras del alfabeto espanol (A-Z incluyendo N con tilde).
- **GetSpanishIndex(char)**: Devuelve la posicion (0-26) de una letra en el alfabeto espanol.
- **GetSpanishLetter(int)**: Devuelve la letra correspondiente a un indice.
- **IsLetterSpanish(char)**: Verifica si un caracter pertenece al alfabeto espanol.
- **PreserveCase(char original, char result)**: Aplica la casing del caracter original al resultado.
- **ApplyCharMap(string, Dictionary)**: Aplica una tabla de sustitucion caracter a caracter preservando mayusculas/minusculas.

### Algoritmos implementados

| Archivo                          | Cifrado           | Tipo                | Descripcion tecnica                                                                |
|----------------------------------|-------------------|---------------------|------------------------------------------------------------------------------------|
| MorseCipherAlgorithm.cs          | Morse             | Codificacion        | Diccionario letra->patron. Separador de letras: espacio. Separador de palabras: /  |
| NumericCipherAlgorithm.cs        | Numerica          | Codificacion        | Indice espanol de 2 digitos (00-26). Decodifica agrupando de a 2.                 |
| CellphoneCipherAlgorithm.cs      | Celular (T9)      | Codificacion        | Tecla + repeticiones con ^. Separador: guion. Espacio=0.                           |
| CenitPolarCipherAlgorithm.cs     | Cenit-Polar       | Sustitucion simetrica | Pares C-P, E-O, N-L, I-A, T-R. Usa `ApplyCharMap`.                              |
| BadenPowelCipherAlgorithm.cs     | Baden-Powell      | Sustitucion simetrica | Pares B-P, A-O, D-W, E=E, N-L. Usa `ApplyCharMap`.                              |
| MurcielagoCipherAlgorithm.cs     | Murcielago        | Sustitucion a digitos | M=0, U=1, R=2, C=3, I=4, E=5, L=6, A=7, G=8, O=9.                              |
| ShiftPlusOneCipherAlgorithm.cs   | Clave +1          | Desplazamiento      | Avanza una posicion en el alfabeto espanol (27 letras). Z->A.                      |
| ShiftMinusOneCipherAlgorithm.cs  | Clave -1          | Desplazamiento      | Retrocede una posicion en el alfabeto espanol (27 letras). A->Z.                   |
| ParelinofoCipherAlgorithm.cs     | Parelinofo        | Sustitucion simetrica | Pares P-U, A-F, R-O, E-N, L-I. Usa `ApplyCharMap`.                              |
| DametupicoCipherAlgorithm.cs     | Dametupico        | Sustitucion simetrica | Pares D-O, A-C, M-I, E-P, T-U. Usa `ApplyCharMap`.                              |
| AgujeritoCipherAlgorithm.cs      | Agujerito         | Sustitucion simetrica | Pares A-O, G-T, U-I, J-R, E=E. Usa `ApplyCharMap`.                              |
| GatoCipherAlgorithm.cs           | Gato (Pigpen)     | Simbolico           | Genera claves con prefijo `GATO:`. 27 simbolos (A-Z + N con tilde).               |
| SemaforoCipherAlgorithm.cs       | Semaforo          | Simbolico           | Genera claves con prefijo `SEMAFORO:`. 26 simbolos (A-Z sin N con tilde).          |

### Comportamiento comun

- Los caracteres fuera del dominio de la clave (digitos, signos de puntuacion, espacios en cifrados no-Morse) se copian sin modificar.
- Se preserva mayuscula/minuscula en todos los cifrados.
- Los cifrados simetricos usan la misma logica para cifrar y descifrar.
- Los cifrados simbolicos devuelven cadenas con formato `PREFIJO:clave` (ej: `GATO:a GATO:b`) que la vista interpreta para mostrar imagenes.

---

## Models - Modelos de datos

| Archivo                  | Descripcion                                                                                      |
|--------------------------|--------------------------------------------------------------------------------------------------|
| CipherType.cs            | Enumeracion con 13 valores, uno por cada algoritmo de cifrado disponible.                        |
| CipherDefinition.cs      | Modelo que define la vista de un cifrado: Name, Description, Type, Icon, AccentColorHex, etc.    |
| OperationMode.cs         | Enumeracion: Encrypt, Decrypt.                                                                   |
| InputMode.cs             | Enumeracion: Manual, Camera.                                                                     |
| GatoSymbolViewModel.cs   | Modelo para simbolos graficos: Key (letra), ImageSource (ruta de imagen), IsSelected (estado).   |

---

## Services - Capa de servicios

### ICipherService / CipherService

- **Process(CipherType, OperationMode, string)**: Rutea la entrada al algoritmo correcto y devuelve el resultado.
- **GetSupportedCharacters(CipherType)**: Devuelve la cadena de caracteres soportados por el algoritmo.
- **GetAvailableCiphers()**: Retorna la lista de `CipherDefinition` con los 13 cifrados, sus iconos y colores de acento ciclicos (azul, verde, ambar).

El constructor de `CipherService` instancia un `Dictionary<CipherType, ICipherAlgorithm>` con las 13 implementaciones.

### ITextRecognitionService

Interfaz para OCR multiplataforma:

- **RecognizeTextAsync(ImageSource)**: Recibe una imagen y devuelve el texto reconocido.

Implementaciones por plataforma:
- **Android**: `AndroidTextRecognizer` usando Google ML Kit (Text Recognition Latin).
- **iOS**: `IosTextRecognizer` usando el framework Vision de Apple.
- **Windows/macOS**: `UnsupportedTextRecognizer` que retorna un mensaje informativo indicando que OCR no esta disponible.

---

## ViewModels - Logica de presentacion

Se utiliza CommunityToolkit.Mvvm con source generators (`[ObservableProperty]`, `[RelayCommand]`).

### HomeViewModel

- Carga la lista de cifrados disponibles al inicializar.
- Comando `NavigateToCipher` que navega a `CipherDetailPage` pasando tipo, nombre e icono via query string.

### CipherDetailViewModel

ViewModel principal con aproximadamente 576 lineas. Maneja:

- **Propiedades de estado**: texto de entrada/salida, modo de operacion (cifrar/descifrar), modo de entrada (manual/camara), tipo de cifrado activo.
- **IsSymbolicCipher**: Propiedad calculada que devuelve `true` para Gato y Semaforo. Controla la visibilidad de secciones especificas en la vista.
- **LoadSymbolicSymbols()**: Carga las imagenes correspondientes (27 para Gato, 26 para Semaforo) como coleccion de `GatoSymbolViewModel`.
- **ProcessGatoEncrypt(string)**: Cifra letra a letra generando claves con el prefijo dinamico del cifrado simbolico activo.
- **DecryptFromSymbols()**: Descifra una secuencia de simbolos seleccionados por el usuario.
- **ProcessInput() / ExecuteProcess()**: Logica central que invoca `CipherService.Process()` para cifrados de texto.
- **Camara**: Integra `ICameraPipeline`, `IImageSegmenter`, `ISymbolClassifier` y `ITextRecognitionService` para el flujo de OCR.
- **SymbolicCipherInfoText**: Texto informativo dinamico que indica al usuario como funciona el cifrado simbolico activo.

---

## Views - Interfaz de usuario

### HomePage.xaml

Pagina principal que muestra el catalogo de cifrados como una lista. Cada elemento muestra el icono, nombre y descripcion del cifrado con su color de acento. Al seleccionar un cifrado, navega a `CipherDetailPage`.

### CipherDetailPage.xaml

Pagina de detalle con tres secciones principales:

1. **Seccion de texto** (cifrados no simbolicos): Campo de entrada, selector de operacion (cifrar/descifrar), area de resultado con boton de copiar.
2. **Seccion simbolica** (Gato y Semaforo):
   - **Cifrar**: Campo de texto de entrada y cuadricula de imagenes resultado.
   - **Descifrar**: Cuadricula de simbolos seleccionables, boton de descifrar y area de resultado.
3. **Seccion de camara** (lazy-loaded): Componente `CameraSectionView` para captura y OCR.

Incluye un panel informativo para cifrados simbolicos con instrucciones dinamicas por binding.

### CameraSectionView.xaml

Componente reutilizable para la funcionalidad de camara:
- Selector de imagen desde galeria.
- Area de previsualizacion de la imagen capturada.
- Boton para ejecutar el reconocimiento de texto.
- Area de resultado del OCR con opcion de copiar.

---

## Controls - Controles graficos

| Archivo              | Descripcion                                                                         |
|----------------------|-------------------------------------------------------------------------------------|
| WaveDrawable.cs      | Implementa `IDrawable`. Dibuja una ola animada con curvas Bezier en tonos verdes.   |
| MountainDrawable.cs  | Implementa `IDrawable`. Dibuja una silueta de montana con gradiente.                |

Estos controles se utilizan como elementos decorativos en la interfaz para mantener la tematica scout.

---

## Converters - Convertidores XAML

| Archivo                    | Entrada       | Salida | Descripcion                                              |
|----------------------------|---------------|--------|----------------------------------------------------------|
| BoolToColorConverter.cs    | bool          | Color  | true -> color primario, false -> color secundario        |
| HexToColorConverter.cs     | string (hex)  | Color  | Convierte cadenas hexadecimales (#RRGGBB) a Color        |
| InvertedBoolConverter.cs   | bool          | bool   | Invierte el valor booleano                               |
| StringNotEmptyConverter.cs | string        | bool   | Devuelve true si la cadena no es null ni vacia           |

---

## Pipelines - Integracion de vision

Interfaces preparadas para futura integracion con OpenCV y modelos ONNX:

| Interfaz              | Proposito                                              | Implementacion actual            |
|-----------------------|--------------------------------------------------------|----------------------------------|
| ICameraPipeline       | Pipeline completo de camara a resultado                | PlaceholderCameraPipeline        |
| IImageSegmenter       | Segmentacion de imagen en regiones de interes          | PlaceholderImageSegmenter        |
| ISymbolClassifier     | Clasificacion de simbolos segmentados                  | PlaceholderSymbolClassifier      |

Las implementaciones placeholder devuelven resultados vacios o mensajes informativos. Estan registradas como singletons en la DI.

---

## Platforms - Codigo nativo

| Carpeta        | Contenido                                                                     |
|----------------|-------------------------------------------------------------------------------|
| Android/       | MainActivity, MainApplication, AndroidManifest.xml, AndroidTextRecognizer     |
| iOS/           | AppDelegate, Program, Info.plist, IosTextRecognizer                           |
| MacCatalyst/   | AppDelegate, Program, Info.plist                                              |
| Windows/       | App.xaml, Package.appxmanifest                                                |
| Tizen/         | Main.cs, tizen-manifest.xml                                                   |

El reconocimiento de texto (OCR) se implementa de forma nativa:
- **Android**: Usa `Xamarin.Google.MLKit.TextRecognition` con el modelo latino.
- **iOS**: Usa el framework `Vision` de Apple con `VNRecognizeTextRequest`.

---

## Resources - Recursos de la aplicacion

| Carpeta        | Contenido                                                     |
|----------------|---------------------------------------------------------------|
| AppIcon/       | Icono de la aplicacion en formato SVG                         |
| Fonts/         | Fuentes personalizadas (.ttf)                                 |
| Images/Gato/   | 27 imagenes PNG: gato_a.png a gato_z.png + gato_enie.png     |
| Images/Semaforo/ | 26 imagenes PNG: semaforo_a.png a semaforo_z.png            |
| Raw/           | Recursos crudos                                               |
| Splash/        | Pantalla de carga (splash screen)                             |
| Styles/        | Colors.xaml y Styles.xaml con el tema visual de la app        |

Los recursos de imagen de Gato y Semaforo estan configurados en el `.csproj` con `Resize="False"` para mantener la calidad original de los simbolos.

---

## Configuracion y punto de entrada

### MauiProgram.cs

Configura el builder de MAUI y registra todos los servicios en el contenedor de DI:

| Registro                    | Lifetime   | Descripcion                                        |
|-----------------------------|------------|----------------------------------------------------|
| ICipherService              | Singleton  | Servicio principal de cifrado                      |
| ICameraPipeline             | Singleton  | Pipeline de camara (placeholder)                   |
| IImageSegmenter             | Singleton  | Segmentador de imagen (placeholder)                |
| ISymbolClassifier           | Singleton  | Clasificador de simbolos (placeholder)             |
| ITextRecognitionService     | Singleton  | OCR (condicional por plataforma)                   |
| HomeViewModel               | Transient  | ViewModel de la pagina principal                   |
| CipherDetailViewModel       | Transient  | ViewModel de detalle del cifrado                   |
| HomePage                    | Transient  | Vista de la pagina principal                       |
| CipherDetailPage            | Transient  | Vista de detalle del cifrado                       |

### AppShell.xaml.cs

Registra la ruta `CipherDetailPage` para la navegacion Shell:

```csharp
Routing.RegisterRoute("CipherDetailPage", typeof(CipherDetailPage));
```

---

## Dependencias NuGet

| Paquete                                        | Version | Proposito                             |
|------------------------------------------------|---------|---------------------------------------|
| Microsoft.Maui.Controls                        | 8.0.x   | Framework UI multiplataforma          |
| Microsoft.Maui.Controls.Compatibility          | 8.0.x   | Compatibilidad con controles legacy   |
| Microsoft.Extensions.Logging.Debug             | 8.0.1   | Logging en modo debug                 |
| CommunityToolkit.Mvvm                          | 8.2.2   | MVVM con source generators            |
| Xamarin.Google.MLKit.TextRecognition           | 16.0.1  | OCR en Android (solo Android)         |
| Xamarin.Google.MLKit.TextRecognition.Latin      | 16.0.1  | Modelo de texto latino (solo Android) |

---

## Guia para agregar un nuevo cifrado

### Cifrado de texto

1. **Crear el algoritmo**: Crear una clase en `Ciphers/` que implemente `ICipherAlgorithm`. Utilizar las utilidades de `CipherUtils` para el manejo del alfabeto espanol.

2. **Registrar el tipo**: Agregar un nuevo valor en la enumeracion `CipherType` en `Models/CipherType.cs`.

3. **Registrar en CipherService**: En `Services/CipherService.cs`:
   - Agregar la instancia al diccionario `_algorithms` en el constructor.
   - Agregar una nueva `CipherDefinition` en el metodo `GetAvailableCiphers()`.

4. **Escribir pruebas**: Agregar pruebas unitarias en `ScoutCode.Tests/CipherAlgorithmTests.cs`. Como minimo cubrir: cifrado, descifrado, roundtrip, preservacion de case, y caracteres no soportados.

### Cifrado simbolico

Ademas de los pasos anteriores:

5. **Agregar imagenes**: Colocar las imagenes PNG en `Resources/Images/NombreCifrado/` con el patron `nombrecifrado_letra.png`.

6. **Actualizar .csproj**: Agregar una entrada `MauiImage` con `Resize="False"` para la nueva carpeta de imagenes.

7. **Actualizar CipherDetailViewModel**: Expandir la propiedad `IsSymbolicCipher` y el metodo `LoadSymbolicSymbols()` para incluir el nuevo cifrado simbolico.

8. **Actualizar conteo de pruebas**: Ajustar la prueba `CipherService_GetAvailableCiphers_ReturnsN` con el nuevo total.
