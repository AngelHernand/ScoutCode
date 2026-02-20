using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScoutCode.Models;
using ScoutCode.Pipelines;
using ScoutCode.Services;

namespace ScoutCode.ViewModels;

/// <summary>
/// ViewModel para la pantalla de detalle de cifrado.
/// Maneja las operaciones de cifrado/descifrado manual y el placeholder de cámara.
/// </summary>
[QueryProperty(nameof(CipherTypeValue), "CipherType")]
[QueryProperty(nameof(CipherName), "CipherName")]
public partial class CipherDetailViewModel : ObservableObject
{
    private readonly ICipherService _cipherService;
    private readonly ICameraPipeline _cameraPipeline;

    // --- Propiedades de navegación ---

    [ObservableProperty]
    private string _cipherName = string.Empty;

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
        }
    }

    // --- Propiedades de modo ---

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private bool _isManualMode = true;

    [ObservableProperty]
    private bool _isCameraMode;

    // --- Operación manual ---

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

    // --- Cámara (placeholder) ---

    [ObservableProperty]
    private string _cameraResultText = string.Empty;

    [ObservableProperty]
    private bool _hasCameraResult;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private string _previewImageSource = string.Empty;

    [ObservableProperty]
    private bool _hasPreviewImage;

    public CipherDetailViewModel(ICipherService cipherService, ICameraPipeline cameraPipeline)
    {
        _cipherService = cipherService;
        _cameraPipeline = cameraPipeline;
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        IsManualMode = value == 0;
        IsCameraMode = value == 1;
    }

    partial void OnSelectedOperationIndexChanged(int value)
    {
        OperationLabel = value == 0 ? "Cifrar" : "Descifrar";
        // Limpiar resultado al cambiar operación
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
            StatusMessage = "⚠️ Por favor, ingresa un texto para procesar.";
            HasError = true;
            return;
        }

        try
        {
            var result = _cipherService.Process(SelectedCipher, SelectedOperation, InputText.Trim());

            if (string.IsNullOrEmpty(result))
            {
                StatusMessage = "⚠️ No se pudo procesar el texto. Verifica que los caracteres sean válidos.";
                HasError = true;
                OutputText = string.Empty;
                HasOutput = false;
            }
            else
            {
                OutputText = result;
                HasOutput = true;
                StatusMessage = SelectedOperation == OperationMode.Encrypt
                    ? "✅ Texto cifrado exitosamente."
                    : "✅ Texto descifrado exitosamente.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Error: {ex.Message}";
            HasError = true;
            HasOutput = false;
        }
    }

    [RelayCommand]
    private async Task CopyResult()
    {
        if (string.IsNullOrEmpty(OutputText)) return;

        await Clipboard.Default.SetTextAsync(OutputText);
        StatusMessage = "📋 Resultado copiado al portapapeles.";
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
    }

    // --- Comandos de cámara (placeholder) ---

    [RelayCommand]
    private async Task TakePhoto()
    {
        HasCameraResult = false;
        CameraResultText = string.Empty;

        try
        {
            // Placeholder: intentar usar la cámara del dispositivo
            if (MediaPicker.Default.IsCaptureSupported)
            {
                // En un futuro se capturará la foto real
                CameraResultText = "📸 Captura de foto disponible.\nEl procesamiento de imagen estará disponible próximamente.";
            }
            else
            {
                CameraResultText = "📸 La captura de fotos no está soportada en este dispositivo.";
            }
        }
        catch
        {
            CameraResultText = "📸 No se pudo acceder a la cámara.";
        }

        HasCameraResult = true;
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task PickPhoto()
    {
        HasCameraResult = false;
        CameraResultText = string.Empty;

        try
        {
            CameraResultText = "🖼️ Selección de imagen disponible.\nEl procesamiento de imagen estará disponible próximamente.";
        }
        catch
        {
            CameraResultText = "🖼️ No se pudo seleccionar la imagen.";
        }

        HasCameraResult = true;
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ProcessCamera()
    {
        IsProcessing = true;
        HasCameraResult = false;

        try
        {
            var result = await _cameraPipeline.ProcessImageAsync(Array.Empty<byte>());
            CameraResultText = result;
            HasCameraResult = true;
        }
        catch (Exception ex)
        {
            CameraResultText = $"❌ Error: {ex.Message}";
            HasCameraResult = true;
        }
        finally
        {
            IsProcessing = false;
        }
    }
}
