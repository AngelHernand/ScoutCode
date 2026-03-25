# ScoutCode

Aplicacion multiplataforma de cifrados scouts desarrollada con .NET MAUI 8. Permite cifrar y descifrar mensajes utilizando 13 algoritmos clasicos del escultismo, incluyendo cifrados de texto, sustitucion por simbolos graficos y codificacion con banderas de semaforo. Funciona completamente offline.

---

## Tabla de contenidos

1. [Descripcion general](#descripcion-general)
2. [Arquitectura del proyecto](#arquitectura-del-proyecto)
3. [Algoritmos de cifrado](#algoritmos-de-cifrado)
4. [Plataformas soportadas](#plataformas-soportadas)
5. [Requisitos previos](#requisitos-previos)
6. [Compilacion y ejecucion](#compilacion-y-ejecucion)
7. [Ejecucion de pruebas](#ejecucion-de-pruebas)
8. [Estructura del repositorio](#estructura-del-repositorio)
9. [Dependencias](#dependencias)
10. [Contribuciones](#contribuciones)
11. [Licencia](#licencia)

---

## Descripcion general

ScoutCode es una herramienta educativa disenada para que scouts y guias practiquen la criptografia basica utilizada en actividades al aire libre, campamentos y competencias. La aplicacion ofrece:

- **Cifrado y descifrado** de texto libre con 13 algoritmos distintos.
- **Cifrados simbolicos** (Gato/Pigpen y Semaforo) que convierten texto en imagenes representativas.
- **Reconocimiento optico de caracteres (OCR)** por camara en Android e iOS para descifrar mensajes fotografiados.
- **Interfaz adaptable** con tema scout (tonos verdes, ambar y azul) y componentes graficos personalizados (olas, montanas).
- **Arquitectura limpia** MVVM con inyeccion de dependencias y cobertura de pruebas unitarias.

---

## Arquitectura del proyecto

```
ScoutCode.sln
|
|-- ScoutCode/              Aplicacion .NET MAUI (proyecto principal)
|   |-- Ciphers/            13 algoritmos de cifrado (Strategy Pattern)
|   |-- Models/             Modelos de datos y enumeraciones
|   |-- Services/           Servicios de cifrado y OCR
|   |-- ViewModels/         ViewModels con CommunityToolkit.Mvvm
|   |-- Views/              Paginas XAML y code-behind
|   |-- Controls/           Drawables personalizados (olas, montanas)
|   |-- Converters/         Convertidores XAML (bool, color, string)
|   |-- Pipelines/          Interfaces para futura integracion de vision
|   |-- Platforms/          Codigo especifico por plataforma
|   |-- Resources/          Iconos, fuentes, imagenes y estilos
|
|-- ScoutCode.Tests/        Proyecto de pruebas unitarias (xUnit)
```

### Patrones de diseno

| Patron              | Aplicacion                                                                 |
|----------------------|----------------------------------------------------------------------------|
| Strategy             | `ICipherAlgorithm` con 13 implementaciones intercambiables                |
| MVVM                 | ViewModels con `[ObservableProperty]` y `[RelayCommand]` (source gen)     |
| Dependency Injection | Registro en `MauiProgram.cs` via `builder.Services`                       |
| Service Locator      | `CipherService` resuelve el algoritmo por `CipherType`                    |
| Shell Navigation     | Rutas con `[QueryProperty]` para paso de parametros                       |

---

## Algoritmos de cifrado

La aplicacion incluye 13 cifrados organizados en las siguientes categorias:

### Cifrados de texto

| N  | Nombre           | Descripcion                                                     | Ejemplo                         |
|----|------------------|-----------------------------------------------------------------|---------------------------------|
| 1  | Morse            | Codigo Morse internacional: puntos (.) y rayas (-)              | `HOLA` -> `.... --- .-.. .-`   |
| 2  | Numerica         | A=00, B=01, C=02 ... N=14 ... Z=26                             | `HOLA` -> `07151100`           |
| 3  | Celular (T9)     | Teclado telefonico: A=2, B=22, C=222, espacio=0                | `HOLA` -> `4^2-6^3-5^3-2`     |
| 4  | Cenit-Polar      | Sustitucion por pares: C-P, E-O, N-L, I-A, T-R                 | `cenit` -> `polar`             |
| 5  | Baden-Powell     | Sustitucion por pares: B-P, A-O, D-W, E=E, N-L                 | `baden` -> `powel`             |
| 6  | Murcielago       | Las letras de MURCIELAGO se mapean a 0-9                        | `MURCIELAGO` -> `0123456789`   |
| 7  | Clave +1         | Desplazamiento: cada letra avanza una posicion (Z vuelve a A)   | `abc` -> `bcd`                 |
| 8  | Clave -1         | Desplazamiento: cada letra retrocede una posicion (A vuelve a Z)| `bcd` -> `abc`                 |
| 9  | Parelinofo       | Sustitucion por pares: P-U, A-F, R-O, E-N, L-I                 | `parent` -> `ufonet`           |
| 10 | Dametupico       | Sustitucion por pares: D-O, A-C, M-I, E-P, T-U                 | `damep` -> `ocipe`             |
| 11 | Agujerito        | Sustitucion por pares: A-O, G-T, U-I, J-R, E=E                 | `agujerit` -> `otirejug`       |

### Cifrados simbolicos

| N  | Nombre           | Descripcion                                                     |
|----|------------------|-----------------------------------------------------------------|
| 12 | Gato (Pigpen)    | Cada letra se convierte en un simbolo grafico de cuadricula     |
| 13 | Semaforo         | Cada letra se convierte en una posicion de banderas              |

### Notas sobre el comportamiento

- Todos los cifrados de texto soportan el alfabeto espanol completo (27 letras, incluyendo la N con tilde).
- Se preserva mayuscula/minuscula: si se escribe `Hola` con +1, el resultado es `Ipnb`.
- Los caracteres que no pertenecen al dominio de la clave (numeros, signos, acentos) se copian sin modificar.
- Los cifrados simetricos (Cenit-Polar, Baden-Powell, Parelinofo, Dametupico, Agujerito) usan la misma operacion para cifrar y descifrar.
- Los cifrados simbolicos generan secuencias de claves de imagen que la interfaz renderiza como graficos.

---

## Plataformas soportadas

| Plataforma   | Version minima          | OCR disponible |
|--------------|--------------------------|----------------|
| Android      | API 21 (Android 5.0)     | Si (ML Kit)    |
| iOS          | 11.0                     | Si (Vision)    |
| macOS        | 13.1 (Catalyst)          | No             |
| Windows      | 10.0.17763.0             | No             |

En las plataformas sin OCR se registra un `UnsupportedTextRecognizer` que devuelve un mensaje informativo.

---

## Requisitos previos

- **.NET 8 SDK** (8.0 o superior)
- **Visual Studio 2022** (17.8+) con la carga de trabajo ".NET Multi-platform App UI development"
- Para Android: Android SDK API 21 o superior
- Para iOS/macOS: Xcode 15 o superior (solo en macOS)
- Para Windows: Windows 10 SDK (10.0.17763.0)

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
# Android
dotnet build ScoutCode/ScoutCode.csproj -t:Run -f net8.0-android

# Windows
dotnet build ScoutCode/ScoutCode.csproj -t:Run -f net8.0-windows10.0.17763.0

# iOS (requiere macOS)
dotnet build ScoutCode/ScoutCode.csproj -t:Run -f net8.0-ios

# macOS Catalyst (requiere macOS)
dotnet build ScoutCode/ScoutCode.csproj -t:Run -f net8.0-maccatalyst
```

---

## Ejecucion de pruebas

El proyecto de pruebas utiliza xUnit y se ejecuta sobre `net8.0` (no MAUI). Actualmente contiene **73 pruebas** que cubren los 13 algoritmos y el servicio de cifrado.

```bash
dotnet test ScoutCode.Tests/ScoutCode.Tests.csproj -c Debug --verbosity normal
```

Para mas detalles sobre las pruebas, consultar [ScoutCode.Tests/README.md](ScoutCode.Tests/README.md).

---

## Estructura del repositorio

```
ScoutCode/
|-- README.md                       Este archivo
|-- ScoutCode.sln                   Solucion principal
|
|-- ScoutCode/                      Proyecto MAUI principal
|   |-- App.xaml(.cs)               Punto de entrada de la aplicacion
|   |-- AppShell.xaml(.cs)          Shell y rutas de navegacion
|   |-- MainPage.xaml(.cs)          Pagina de inicio
|   |-- MauiProgram.cs              Configuracion de DI y servicios
|   |-- ScoutCode.csproj            Definicion del proyecto
|   |
|   |-- Ciphers/                    Algoritmos de cifrado
|   |   |-- ICipherAlgorithm.cs     Interfaz base
|   |   |-- CipherUtils.cs          Utilidades comunes
|   |   |-- MorseCipherAlgorithm.cs
|   |   |-- NumericCipherAlgorithm.cs
|   |   |-- CellphoneCipherAlgorithm.cs
|   |   |-- CenitPolarCipherAlgorithm.cs
|   |   |-- BadenPowelCipherAlgorithm.cs
|   |   |-- MurcielagoCipherAlgorithm.cs
|   |   |-- ShiftPlusOneCipherAlgorithm.cs
|   |   |-- ShiftMinusOneCipherAlgorithm.cs
|   |   |-- ParelinofoCipherAlgorithm.cs
|   |   |-- DametupicoCipherAlgorithm.cs
|   |   |-- AgujeritoCipherAlgorithm.cs
|   |   |-- GatoCipherAlgorithm.cs
|   |   |-- SemaforoCipherAlgorithm.cs
|   |
|   |-- Models/                     Modelos de datos
|   |   |-- CipherType.cs           Enumeracion de 13 tipos
|   |   |-- CipherDefinition.cs     Definicion visual de cada cifrado
|   |   |-- InputMode.cs            Modos de entrada (Manual, Camara)
|   |   |-- OperationMode.cs        Operaciones (Cifrar, Descifrar)
|   |   |-- GatoSymbolViewModel.cs  Modelo para simbolos graficos
|   |
|   |-- Services/                   Capa de servicios
|   |   |-- ICipherService.cs       Interfaz del servicio de cifrado
|   |   |-- CipherService.cs        Implementacion con Strategy Pattern
|   |   |-- ITextRecognitionService.cs  Interfaz OCR multiplataforma
|   |
|   |-- ViewModels/                 ViewModels (MVVM)
|   |   |-- HomeViewModel.cs        Listado de cifrados disponibles
|   |   |-- CipherDetailViewModel.cs  Logica de cifrado/descifrado
|   |
|   |-- Views/                      Vistas XAML
|   |   |-- HomePage.xaml(.cs)      Pagina principal con catalogo
|   |   |-- CipherDetailPage.xaml(.cs)  Detalle de cada cifrado
|   |   |-- CameraSectionView.xaml(.cs) Componente de camara/OCR
|   |
|   |-- Controls/                   Controles graficos
|   |   |-- WaveDrawable.cs         Efecto de ola animada
|   |   |-- MountainDrawable.cs     Silueta de montana
|   |
|   |-- Converters/                 Convertidores XAML
|   |   |-- BoolToColorConverter.cs
|   |   |-- HexToColorConverter.cs
|   |   |-- InvertedBoolConverter.cs
|   |   |-- StringNotEmptyConverter.cs
|   |
|   |-- Pipelines/                  Interfaces de vision (futuro)
|   |   |-- ICameraPipeline.cs
|   |   |-- IImageSegmenter.cs
|   |   |-- ISymbolClassifier.cs
|   |   |-- Placeholder*.cs         Implementaciones temporales
|   |
|   |-- Platforms/                  Codigo nativo por plataforma
|   |   |-- Android/
|   |   |-- iOS/
|   |   |-- MacCatalyst/
|   |   |-- Windows/
|   |   |-- Tizen/
|   |
|   |-- Resources/                  Recursos de la aplicacion
|       |-- AppIcon/                Icono de la app
|       |-- Fonts/                  Fuentes personalizadas
|       |-- Images/Gato/            27 simbolos del cifrado Gato
|       |-- Images/Semaforo/        26 banderas del cifrado Semaforo
|       |-- Splash/                 Pantalla de carga
|       |-- Styles/                 Estilos y temas XAML
|
|-- ScoutCode.Tests/                Proyecto de pruebas unitarias
    |-- CipherAlgorithmTests.cs     73 pruebas (xUnit)
    |-- ScoutCode.Tests.csproj      Configuracion del proyecto
```

---

## Dependencias

### Proyecto principal (ScoutCode)

| Paquete                                        | Version | Proposito                             |
|------------------------------------------------|---------|---------------------------------------|
| Microsoft.Maui.Controls                        | 8.0.x   | Framework UI multiplataforma          |
| Microsoft.Maui.Controls.Compatibility          | 8.0.x   | Compatibilidad con controles legacy   |
| Microsoft.Extensions.Logging.Debug             | 8.0.1   | Logging en modo debug                 |
| CommunityToolkit.Mvvm                          | 8.2.2   | MVVM con source generators            |
| Xamarin.Google.MLKit.TextRecognition           | 16.0.1  | OCR en Android (ML Kit)               |
| Xamarin.Google.MLKit.TextRecognition.Latin      | 16.0.1  | Modelo de texto latino (Android)      |

### Proyecto de pruebas (ScoutCode.Tests)

| Paquete                                        | Version | Proposito                             |
|------------------------------------------------|---------|---------------------------------------|
| Microsoft.NET.Test.Sdk                         | 17.8.0  | Infraestructura de pruebas            |
| xunit                                          | 2.6.6   | Framework de pruebas unitarias        |
| xunit.runner.visualstudio                      | 2.5.6   | Integrador con Visual Studio          |
| coverlet.collector                             | 6.0.0   | Recoleccion de cobertura de codigo    |

---

## Contribuciones

### Agregar un nuevo cifrado

1. Crear una clase en `ScoutCode/Ciphers/` que implemente `ICipherAlgorithm`.
2. Agregar una entrada en la enumeracion `CipherType`.
3. Registrar la instancia en el diccionario de `CipherService`.
4. Agregar una `CipherDefinition` en `GetAvailableCiphers()` con nombre, descripcion e icono.
5. Escribir pruebas unitarias en `ScoutCode.Tests/CipherAlgorithmTests.cs`.
6. Si el cifrado es simbolico, agregar las imagenes en `Resources/Images/` y actualizar el `.csproj`.

### Convenciones del proyecto

- Idioma del codigo fuente: espanol (nombres de variables, comentarios, UI).
- Alfabeto base: espanol de 27 letras (A-Z incluyendo N con tilde).
- Cada algoritmo debe ser completamente simetrico: `Decrypt(Encrypt(x)) == x`.
- Las pruebas deben cubrir cifrado, descifrado, ida y vuelta (roundtrip), y caracteres especiales.

---

## Licencia

Este proyecto esta licenciado bajo los terminos incluidos en el archivo [LICENSE](ScoutCode/LICENSE).
