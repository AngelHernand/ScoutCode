using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScoutCode.Models;
using ScoutCode.Pipelines;
using ScoutCode.Services;

namespace ScoutCode.ViewModels;

// ViewModel de la pantalla donde se cifra/descifra
[QueryProperty(nameof(CipherTypeValue), "CipherType")]
[QueryProperty(nameof(CipherName), "CipherName")]
public partial class CipherDetailViewModel : ObservableObject
{
    private readonly ICipherService _cipherService;
    private readonly ICameraPipeline _cameraPipeline;

    // --- Propiedades de navegacion ---

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

    // --- Camara (placeholder) ---

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
    }

    // --- Comandos de camara ---

    [RelayCommand]
    private async Task TakePhoto()
    {
        HasCameraResult = false;
        CameraResultText = string.Empty;

        try
        {
            // Placeholder: ver si el dispositivo soporta camara
            if (MediaPicker.Default.IsCaptureSupported)
            {
                CameraResultText = "Captura disponible. El procesamiento de imagen va a estar disponible mas adelante.";
            }
            else
            {
                CameraResultText = "La captura de fotos no esta soportada en este dispositivo.";
            }
        }
        catch
        {
            CameraResultText = "No se pudo acceder a la camara.";
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
            CameraResultText = "Seleccion de imagen disponible. El procesamiento va a estar disponible mas adelante.";
        }
        catch
        {
            CameraResultText = "No se pudo seleccionar la imagen.";
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
            CameraResultText = $"Error: {ex.Message}";
            HasCameraResult = true;
        }
        finally
        {
            IsProcessing = false;
        }
    }
}
