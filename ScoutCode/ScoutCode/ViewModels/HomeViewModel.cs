using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScoutCode.Models;
using ScoutCode.Services;
using System.Collections.ObjectModel;

namespace ScoutCode.ViewModels;

// ViewModel de la pantalla principal, carga la lista de cifrados
public partial class HomeViewModel : ObservableObject
{
    private readonly ICipherService _cipherService;

    [ObservableProperty]
    private string _title = "ScoutCipher";

    [ObservableProperty]
    private string _subtitle = "Claves Scout - 100% Offline";

    [ObservableProperty]
    private ObservableCollection<CipherDefinition> _ciphers = new();

    [ObservableProperty]
    private bool _isLoading;

    public HomeViewModel(ICipherService cipherService)
    {
        _cipherService = cipherService;
        LoadCiphers();
    }

    private void LoadCiphers()
    {
        IsLoading = true;
        var available = _cipherService.GetAvailableCiphers();
        Ciphers = new ObservableCollection<CipherDefinition>(available);
        IsLoading = false;
    }

    [RelayCommand]
    private async Task NavigateToCipher(CipherDefinition cipher)
    {
        if (cipher == null) return;

        var parameters = new Dictionary<string, object>
        {
            { "CipherType", cipher.Type },
            { "CipherName", cipher.Name },
            { "CipherIcon", cipher.Icon }
        };

        await Shell.Current.GoToAsync("CipherDetailPage", parameters);
    }
}
