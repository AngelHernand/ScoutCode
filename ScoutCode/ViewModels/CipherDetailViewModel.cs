using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScoutCode.Models;
using ScoutCode.Pipelines;
using ScoutCode.Services;
using System.Collections.ObjectModel;

namespace ScoutCode.ViewModels;

// ViewModel de la pantalla donde se cifra/descifra
[QueryProperty(nameof(CipherTypeValue), "CipherType")]
[QueryProperty(nameof(CipherName), "CipherName")]
[QueryProperty(nameof(CipherIcon), "CipherIcon")]
public partial class CipherDetailViewModel : ObservableObject
{
    private readonly ICipherService _cipherService;
    private readonly ICameraPipeline _cameraPipeline;
    private readonly ITextRecognitionService _textRecognitionService;

    // --- Propiedades de navegacion ---

    [ObservableProperty]
    private string _cipherName = string.Empty;

    [ObservableProperty]
    private string _cipherIcon = string.Empty;

    private CipherType _selectedCipher;
    public CipherType SelectedCipher
    {
        get => _selectedCipher;
        set => SetProperty(ref _selectedCipher, value);
    }

    // Propiedad para recibir el query parameter como int
    public int CipherTypeValue
    {
        set
        {
            SelectedCipher = (CipherType)value;
            OnPropertyChanged(nameof(SelectedCipher));
            SupportedCharsInfo = _cipherService.GetSupportedCharacters(SelectedCipher);
            IsOcrSupported = IsOcrCompatibleCipher() && _textRecognitionService.IsAvailable;
            IsSymbolicCipher = SelectedCipher == CipherType.Gato;
            if (IsSymbolicCipher)
                LoadGatoSymbols();
        }
    }

    // --- Modo de entrada ---

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private bool _isManualMode = true;

    [ObservableProperty]
    private bool _isCameraMode;

    // --- Cifrado manual ---

    [ObservableProperty]
    private int _selectedOperationIndex;

    public OperationMode SelectedOperation =>
        SelectedOperationIndex == 0 ? OperationMode.Encrypt : OperationMode.Decrypt;

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private string _outputText = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasOutput;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _supportedCharsInfo = string.Empty;

    [ObservableProperty]
    private string _operationLabel = "Cifrar";

    // --- Camara y OCR ---

    [ObservableProperty]
    private string _cameraResultText = string.Empty;

    [ObservableProperty]
    private bool _hasCameraResult;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private ImageSource? _previewImageSource;

    [ObservableProperty]
    private bool _hasPreviewImage;

    [ObservableProperty]
    private bool _isOcrSupported;

    [ObservableProperty]
    private string _ocrStatusMessage = string.Empty;

    // --- Cifrado Gato (simbólico) ---

    [ObservableProperty]
    private bool _isSymbolicCipher;

    // Grilla completa de los 27 símbolos para que el usuario seleccione
    public ObservableCollection<GatoSymbolViewModel> GatoSymbols { get; } = new();

    // Secuencia de símbolos seleccionados por el usuario (modo descifrar)
    public ObservableCollection<GatoSymbolViewModel> SelectedGatoSymbols { get; } = new();

    // Imágenes resultado del cifrado (modo cifrar)
    public ObservableCollection<GatoSymbolViewModel> GatoEncryptedSymbols { get; } = new();

    [ObservableProperty]
    private string _gatoDecryptedText = string.Empty;

    [ObservableProperty]
    private bool _hasGatoEncryptResult;

    [ObservableProperty]
    private bool _hasGatoDecryptResult;

    public CipherDetailViewModel(
        ICipherService cipherService,
        ICameraPipeline cameraPipeline,
        ITextRecognitionService textRecognitionService)
    {
        _cipherService = cipherService;
        _cameraPipeline = cameraPipeline;
        _textRecognitionService = textRecognitionService;
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        IsManualMode = value == 0;
        IsCameraMode = value == 1;
    }

    partial void OnSelectedOperationIndexChanged(int value)
    {
        OperationLabel = value == 0 ? "Cifrar" : "Descifrar";
        // Limpiar al cambiar de operacion
        OutputText = string.Empty;
        HasOutput = false;
        StatusMessage = string.Empty;
        HasError = false;
    }

    [RelayCommand]
    private void Process()
    {
        HasError = false;
        StatusMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(InputText))
        {
            StatusMessage = "Ingresa un texto para procesar.";
            HasError = true;
            return;
        }

        try
        {
            var result = _cipherService.Process(SelectedCipher, SelectedOperation, InputText.Trim());

            if (string.IsNullOrEmpty(result))
            {
                StatusMessage = "No se pudo procesar. Verifica que los caracteres sean validos.";
                HasError = true;
                OutputText = string.Empty;
                HasOutput = false;
            }
            else
            {
                OutputText = result;
                HasOutput = true;
                StatusMessage = SelectedOperation == OperationMode.Encrypt
                    ? "Texto cifrado."
                    : "Texto descifrado.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            HasError = true;
            HasOutput = false;
        }
    }

    [RelayCommand]
    private async Task CopyResult()
    {
        if (string.IsNullOrEmpty(OutputText)) return;

        await Clipboard.Default.SetTextAsync(OutputText);
        StatusMessage = "Copiado al portapapeles.";
        HasError = false;
    }

    [RelayCommand]
    private void ClearAll()
    {
        InputText = string.Empty;
        OutputText = string.Empty;
        StatusMessage = string.Empty;
        HasOutput = false;
        HasError = false;
        // Limpiar también el estado Gato
        GatoEncryptedSymbols.Clear();
        HasGatoEncryptResult = false;
        SelectedGatoSymbols.Clear();
        GatoDecryptedText = string.Empty;
        HasGatoDecryptResult = false;
    }

    // --- Cifrado Gato: carga de símbolos ---

    private void LoadGatoSymbols()
    {
        GatoSymbols.Clear();

        foreach (var letter in ScoutCode.Ciphers.CipherUtils.SpanishAlphabet)
        {
            var key = letter == 'Ñ' ? "enie" : letter.ToString().ToLowerInvariant();
            GatoSymbols.Add(new GatoSymbolViewModel
            {
                Key = key,
                Letter = letter.ToString(),
                ImageSource = $"gato_{key}.png"
            });
        }
    }

    // --- Cifrado Gato: cifrar (texto → símbolos) ---

    [RelayCommand]
    private void ProcessGatoEncrypt()
    {
        HasError = false;
        StatusMessage = string.Empty;
        GatoEncryptedSymbols.Clear();
        HasGatoEncryptResult = false;

        if (string.IsNullOrWhiteSpace(InputText))
        {
            StatusMessage = "Ingresa un texto para cifrar.";
            HasError = true;
            return;
        }

        try
        {
            var result = _cipherService.Process(SelectedCipher, OperationMode.Encrypt, InputText.Trim());

            if (string.IsNullOrEmpty(result))
            {
                StatusMessage = "No se pudo procesar. Verifica que los caracteres sean válidos.";
                HasError = true;
                return;
            }

            // Parsear "GATO:h,o,l,a" → lista de imágenes
            if (result.StartsWith("GATO:", StringComparison.OrdinalIgnoreCase))
            {
                var payload = result["GATO:".Length..];
                var keys = payload.Split(',');
                foreach (var k in keys)
                {
                    var trimmed = k.Trim();
                    if (trimmed == " " || trimmed == "")
                    {
                        // Espacio → no agregar símbolo, se puede representar con gap
                        GatoEncryptedSymbols.Add(new GatoSymbolViewModel
                        {
                            Key = " ",
                            Letter = " ",
                            ImageSource = string.Empty
                        });
                    }
                    else
                    {
                        var letterDisplay = trimmed.Equals("enie", StringComparison.OrdinalIgnoreCase)
                            ? "Ñ" : trimmed.ToUpperInvariant();
                        GatoEncryptedSymbols.Add(new GatoSymbolViewModel
                        {
                            Key = trimmed,
                            Letter = letterDisplay,
                            ImageSource = $"gato_{trimmed}.png"
                        });
                    }
                }
                HasGatoEncryptResult = true;
                StatusMessage = "Texto cifrado en símbolos Gato.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            HasError = true;
        }
    }

    // --- Cifrado Gato: seleccionar símbolo (modo descifrar) ---

    [RelayCommand]
    private void SelectGatoSymbol(GatoSymbolViewModel symbol)
    {
        if (symbol == null) return;

        // Agregar a la secuencia seleccionada
        SelectedGatoSymbols.Add(new GatoSymbolViewModel
        {
            Key = symbol.Key,
            Letter = symbol.Letter,
            ImageSource = symbol.ImageSource
        });

        // Flash visual: marcar como seleccionado brevemente
        symbol.IsSelected = true;
        _ = Task.Run(async () =>
        {
            await Task.Delay(200);
            MainThread.BeginInvokeOnMainThread(() => symbol.IsSelected = false);
        });
    }

    [RelayCommand]
    private void AddGatoSpace()
    {
        SelectedGatoSymbols.Add(new GatoSymbolViewModel
        {
            Key = " ",
            Letter = " ",
            ImageSource = string.Empty
        });
    }

    [RelayCommand]
    private void RemoveLastGatoSymbol()
    {
        if (SelectedGatoSymbols.Count > 0)
            SelectedGatoSymbols.RemoveAt(SelectedGatoSymbols.Count - 1);
    }

    [RelayCommand]
    private void ClearGatoSymbols()
    {
        SelectedGatoSymbols.Clear();
        GatoDecryptedText = string.Empty;
        HasGatoDecryptResult = false;
        StatusMessage = string.Empty;
        HasError = false;
    }

    // --- Cifrado Gato: descifrar (símbolos → texto) ---

    [RelayCommand]
    private void ProcessGatoDecrypt()
    {
        HasError = false;
        StatusMessage = string.Empty;
        GatoDecryptedText = string.Empty;
        HasGatoDecryptResult = false;

        if (SelectedGatoSymbols.Count == 0)
        {
            StatusMessage = "Selecciona al menos un símbolo para descifrar.";
            HasError = true;
            return;
        }

        try
        {
            // Reconstruir formato "GATO:h,o,l,a"
            var keys = SelectedGatoSymbols.Select(s => s.Key);
            var gatoInput = "GATO:" + string.Join(",", keys);

            var result = _cipherService.Process(SelectedCipher, OperationMode.Decrypt, gatoInput);
            GatoDecryptedText = result;
            HasGatoDecryptResult = true;
            StatusMessage = "Símbolos descifrados correctamente.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            HasError = true;
        }
    }

    // --- Verifica si el cifrado es compatible con OCR ---

    private bool IsOcrCompatibleCipher()
    {
        // Gato usa símbolos gráficos → OCR de texto no sirve.
        return SelectedCipher != CipherType.Gato;
    }

    // --- Comandos de camara ---

    [RelayCommand]
    private async Task TakePhoto()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                OcrStatusMessage = "La camara no esta disponible en este dispositivo.";
                return;
            }

            IsProcessing = true;
            OcrStatusMessage = string.Empty;
            CameraResultText = string.Empty;
            HasCameraResult = false;

            var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Tomar foto del texto cifrado"
            });

            if (photo != null)
            {
                await ProcessPhotoAsync(photo);
            }
        }
        catch (Exception ex)
        {
            OcrStatusMessage = $"Error al tomar la foto: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand]
    private async Task PickPhoto()
    {
        try
        {
            IsProcessing = true;
            OcrStatusMessage = string.Empty;
            CameraResultText = string.Empty;
            HasCameraResult = false;

            var photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Seleccionar foto del texto cifrado"
            });

            if (photo != null)
            {
                await ProcessPhotoAsync(photo);
            }
        }
        catch (Exception ex)
        {
            OcrStatusMessage = $"Error al seleccionar la foto: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task ProcessPhotoAsync(FileResult photo)
    {
        try
        {
            // Mostrar preview de la imagen
            PreviewImageSource = ImageSource.FromFile(photo.FullPath);
            HasPreviewImage = true;

            // Leer bytes para OCR
            using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            // Ejecutar OCR
            OcrStatusMessage = "Analizando imagen...";
            var recognizedText = await _textRecognitionService.RecognizeTextAsync(imageBytes);

            if (string.IsNullOrWhiteSpace(recognizedText))
            {
                OcrStatusMessage = "No se detecto texto en la imagen. Intenta con mejor iluminacion.";
                CameraResultText = string.Empty;
                HasCameraResult = false;
            }
            else
            {
                CameraResultText = recognizedText;
                HasCameraResult = true;
                OcrStatusMessage = "Texto reconocido. Podes editarlo antes de descifrar.";
            }
        }
        catch (Exception ex)
        {
            OcrStatusMessage = $"Error al procesar la imagen: {ex.Message}";
            CameraResultText = string.Empty;
            HasCameraResult = false;
        }
    }

    [RelayCommand]
    private async Task ProcessCamera()
    {
        if (string.IsNullOrWhiteSpace(CameraResultText))
        {
            OcrStatusMessage = "No hay texto para descifrar.";
            return;
        }

        try
        {
            IsProcessing = true;
            OcrStatusMessage = "Descifrando...";

            // Usar la MISMA logica que el modo Manual
            var result = _cipherService.Process(
                SelectedCipher,
                OperationMode.Decrypt, // Siempre descifrar en modo camara
                CameraResultText.Trim());

            CameraResultText = result;
            HasCameraResult = true;
            OcrStatusMessage = "Texto descifrado correctamente.";
        }
        catch (Exception ex)
        {
            OcrStatusMessage = $"Error al descifrar: {ex.Message}";
        }
        finally
        {
            IsProcessing = false;
        }

        await Task.CompletedTask;
    }
}
