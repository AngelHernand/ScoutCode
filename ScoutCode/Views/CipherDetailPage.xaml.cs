using ScoutCode.ViewModels;

namespace ScoutCode.Views;

public partial class CipherDetailPage : ContentPage
{
    private readonly CipherDetailViewModel _viewModel;

    public CipherDetailPage(CipherDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    private void OnManualTabClicked(object? sender, EventArgs e)
    {
        _viewModel.SelectedTabIndex = 0;
        UpdateTabStyles(isManual: true);
    }

    private void OnCameraTabClicked(object? sender, EventArgs e)
    {
        _viewModel.SelectedTabIndex = 1;
        UpdateTabStyles(isManual: false);
    }

    private void OnEncryptSelected(object? sender, EventArgs e)
    {
        _viewModel.SelectedOperationIndex = 0;
        UpdateOperationStyles(isEncrypt: true);
    }

    private void OnDecryptSelected(object? sender, EventArgs e)
    {
        _viewModel.SelectedOperationIndex = 1;
        UpdateOperationStyles(isEncrypt: false);
    }

    private void OnGatoEncryptSelected(object? sender, EventArgs e)
    {
        _viewModel.SelectedOperationIndex = 0;
        GatoEncryptSection.IsVisible = true;
        GatoDecryptSection.IsVisible = false;
        UpdateGatoOperationStyles(isEncrypt: true);
    }

    private void OnGatoDecryptSelected(object? sender, EventArgs e)
    {
        _viewModel.SelectedOperationIndex = 1;
        GatoEncryptSection.IsVisible = false;
        GatoDecryptSection.IsVisible = true;
        UpdateGatoOperationStyles(isEncrypt: false);
    }

    private void UpdateGatoOperationStyles(bool isEncrypt)
    {
        var primary = (Color)Application.Current!.Resources["Primary"];
        var grayBg = Application.Current.RequestedTheme == AppTheme.Dark
            ? (Color)Application.Current.Resources["Gray600"]
            : (Color)Application.Current.Resources["Gray200"];
        var grayText = Application.Current.RequestedTheme == AppTheme.Dark
            ? (Color)Application.Current.Resources["Gray200"]
            : (Color)Application.Current.Resources["Gray600"];

        GatoEncryptBtn.BackgroundColor = isEncrypt ? primary : grayBg;
        GatoEncryptBtn.TextColor = isEncrypt ? Colors.White : grayText;
        GatoEncryptBtn.FontAttributes = isEncrypt ? FontAttributes.Bold : FontAttributes.None;

        GatoDecryptBtn.BackgroundColor = !isEncrypt ? primary : grayBg;
        GatoDecryptBtn.TextColor = !isEncrypt ? Colors.White : grayText;
        GatoDecryptBtn.FontAttributes = !isEncrypt ? FontAttributes.Bold : FontAttributes.None;
    }

    private void UpdateTabStyles(bool isManual)
    {
        var primary = (Color)Application.Current!.Resources["Primary"];
        var grayBg = Application.Current.RequestedTheme == AppTheme.Dark
            ? (Color)Application.Current.Resources["Gray600"]
            : (Color)Application.Current.Resources["Gray200"];
        var grayText = Application.Current.RequestedTheme == AppTheme.Dark
            ? (Color)Application.Current.Resources["Gray200"]
            : (Color)Application.Current.Resources["Gray600"];

        ManualTabBtn.BackgroundColor = isManual ? primary : grayBg;
        ManualTabBtn.TextColor = isManual ? Colors.White : grayText;
        ManualTabBtn.FontAttributes = isManual ? FontAttributes.Bold : FontAttributes.None;

        CameraTabBtn.BackgroundColor = !isManual ? primary : grayBg;
        CameraTabBtn.TextColor = !isManual ? Colors.White : grayText;
        CameraTabBtn.FontAttributes = !isManual ? FontAttributes.Bold : FontAttributes.None;
    }

    private void UpdateOperationStyles(bool isEncrypt)
    {
        var primary = (Color)Application.Current!.Resources["Primary"];
        var grayBg = Application.Current.RequestedTheme == AppTheme.Dark
            ? (Color)Application.Current.Resources["Gray600"]
            : (Color)Application.Current.Resources["Gray200"];
        var grayText = Application.Current.RequestedTheme == AppTheme.Dark
            ? (Color)Application.Current.Resources["Gray200"]
            : (Color)Application.Current.Resources["Gray600"];

        EncryptBtn.BackgroundColor = isEncrypt ? primary : grayBg;
        EncryptBtn.TextColor = isEncrypt ? Colors.White : grayText;
        EncryptBtn.FontAttributes = isEncrypt ? FontAttributes.Bold : FontAttributes.None;

        DecryptBtn.BackgroundColor = !isEncrypt ? primary : grayBg;
        DecryptBtn.TextColor = !isEncrypt ? Colors.White : grayText;
        DecryptBtn.FontAttributes = !isEncrypt ? FontAttributes.Bold : FontAttributes.None;
    }
}
