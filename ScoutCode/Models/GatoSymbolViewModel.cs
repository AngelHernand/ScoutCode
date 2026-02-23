using CommunityToolkit.Mvvm.ComponentModel;

namespace ScoutCode.Models;

// Representa un símbolo del cifrado Gato para mostrar en la grilla de selección.
public partial class GatoSymbolViewModel : ObservableObject
{
    // Clave interna: "a", "b", ..., "enie" (para la Ñ)
    public string Key { get; set; } = string.Empty;

    // Letra que representa: "A", "B", ..., "Ñ"
    public string Letter { get; set; } = string.Empty;

    // Nombre del recurso de imagen: "gato_a.png", "gato_enie.png", etc.
    public string ImageSource { get; set; } = string.Empty;

    // Indica si el símbolo fue seleccionado (para resaltado visual)
    [ObservableProperty]
    private bool _isSelected;
}
