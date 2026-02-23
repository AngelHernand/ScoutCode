using ScoutCode.ViewModels;

namespace ScoutCode.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnInstagramTapped(object? sender, EventArgs e)
    {
        var uri = "https://www.instagram.com/_angelin_angel0n_?igsh=MTBmbXF4aGR1ZWs0MA%3D%3D&utm_source=qr";
        await Launcher.Default.OpenAsync(new Uri(uri));
    }
}
