using ScoutCode.ViewModels;

namespace ScoutCode.Views;

public partial class CipherDetailPage : ContentPage
{
    private readonly CipherDetailViewModel _viewModel;
    private bool _cameraLoaded;

    public CipherDetailPage(CipherDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    // ── Navigation ──────────────────────────────────────────
    private async void OnBackTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    // ── Tab toggle: Manual / Cámara ─────────────────────────
    private void OnManualTabClicked(object? sender, EventArgs e)
    {
        _viewModel.SelectedTabIndex = 0;
        UpdateTabStyles(isManual: true);
    }

    private void OnCameraTabClicked(object? sender, EventArgs e)
    {
        _viewModel.SelectedTabIndex = 1;
        UpdateTabStyles(isManual: false);
        EnsureCameraLoaded();
    }

    private void EnsureCameraLoaded()
    {
        if (_cameraLoaded) return;
        _cameraLoaded = true;

        var cameraView = new CameraSectionView { BindingContext = _viewModel };
        CameraContainer.Children.Add(cameraView);
    }

    private void UpdateTabStyles(bool isManual)
    {
        var card = (Color)Application.Current!.Resources["CardColor"];
        var muted = (Color)Application.Current.Resources["MutedText"];
        var foreground = (Color)Application.Current.Resources["ForegroundText"];

        // Active tab = white card + shadow, inactive = transparent
        ManualTabBorder.BackgroundColor = isManual ? card : Colors.Transparent;
        ManualTabBorder.Shadow = isManual ? new Shadow { Brush = new SolidColorBrush(Color.FromArgb("#1A000000")), Offset = new Point(0, 1), Radius = 3 } : null;
        ManualTabLabel.TextColor = isManual ? foreground : muted;
        ManualTabLabel.FontFamily = isManual ? "InterSemiBold" : "InterMedium";

        CameraTabBorder.BackgroundColor = !isManual ? card : Colors.Transparent;
        CameraTabBorder.Shadow = !isManual ? new Shadow { Brush = new SolidColorBrush(Color.FromArgb("#1A000000")), Offset = new Point(0, 1), Radius = 3 } : null;
        CameraTabLabel.TextColor = !isManual ? foreground : muted;
        CameraTabLabel.FontFamily = !isManual ? "InterSemiBold" : "InterMedium";
    }

    // ── Operation toggle: Cifrar / Descifrar ────────────────
    private void OnEncryptSelected(object? sender, EventArgs e)
    {
        _viewModel.SelectedOperationIndex = 0;
        UpdateOperationStyles(isEncrypt: true);

        // Handle Gato sections visibility
        if (_viewModel.IsSymbolicCipher)
        {
            GatoEncryptSection.IsVisible = true;
            GatoDecryptSection.IsVisible = false;
        }
    }

    private void OnDecryptSelected(object? sender, EventArgs e)
    {
        _viewModel.SelectedOperationIndex = 1;
        UpdateOperationStyles(isEncrypt: false);

        // Handle Gato sections visibility
        if (_viewModel.IsSymbolicCipher)
        {
            GatoEncryptSection.IsVisible = false;
            GatoDecryptSection.IsVisible = true;
        }
    }

    private void UpdateOperationStyles(bool isEncrypt)
    {
        var green = (Color)Application.Current!.Resources["MountainGreen"];
        var blue = (Color)Application.Current.Resources["MountainBlue"];
        var muted = (Color)Application.Current.Resources["MutedText"];

        // Active = colored bg + white text, inactive = transparent + muted
        EncryptToggleBorder.BackgroundColor = isEncrypt ? green : Colors.Transparent;
        EncryptToggleLabel.TextColor = isEncrypt ? Colors.White : muted;
        EncryptToggleLabel.FontFamily = isEncrypt ? "InterSemiBold" : "InterMedium";

        DecryptToggleBorder.BackgroundColor = !isEncrypt ? blue : Colors.Transparent;
        DecryptToggleLabel.TextColor = !isEncrypt ? Colors.White : muted;
        DecryptToggleLabel.FontFamily = !isEncrypt ? "InterSemiBold" : "InterMedium";

        // Update header pill
        UpdateModePill(isEncrypt);

        // Update process button and accent bar colors
        UpdateProcessButtonColor(isEncrypt);
    }

    private void UpdateModePill(bool isEncrypt)
    {
        var encPillBg = (Color)Application.Current!.Resources["EncryptPillBg"];
        var encPillText = (Color)Application.Current.Resources["EncryptPillText"];
        var decPillBg = (Color)Application.Current.Resources["DecryptPillBg"];
        var decPillText = (Color)Application.Current.Resources["DecryptPillText"];

        ModePill.BackgroundColor = isEncrypt ? encPillBg : decPillBg;
        ModePillLabel.Text = isEncrypt ? "Cifrar" : "Descifrar";
        ModePillLabel.TextColor = isEncrypt ? encPillText : decPillText;
    }

    private void UpdateProcessButtonColor(bool isEncrypt)
    {
        var green = (Color)Application.Current!.Resources["MountainGreen"];
        var blue = (Color)Application.Current.Resources["MountainBlue"];

        ProcessBtn.BackgroundColor = isEncrypt ? green : blue;
        AccentBar.BackgroundColor = isEncrypt ? green : blue;
    }

    // ── Swap & Copy ─────────────────────────────────────────
    private void OnSwapTapped(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_viewModel.OutputText)) return;
        _viewModel.InputText = _viewModel.OutputText;
        _viewModel.OutputText = string.Empty;
        _viewModel.HasOutput = false;

        // Flip the operation mode
        bool wasEncrypt = _viewModel.SelectedOperationIndex == 0;
        if (wasEncrypt)
            OnDecryptSelected(null, EventArgs.Empty);
        else
            OnEncryptSelected(null, EventArgs.Empty);
    }

    private async void OnCopyTapped(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_viewModel.OutputText)) return;
        await Clipboard.Default.SetTextAsync(_viewModel.OutputText);

        // Visual feedback
        CopyLabel.Text = "¡Copiado!";
        CopyIcon.Text = "OK";
        await Task.Delay(1500);
        CopyLabel.Text = "Copiar";
        CopyIcon.Text = "";
    }
}
