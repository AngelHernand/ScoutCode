using ScoutCode.ViewModels;

namespace ScoutCode.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Animate card entries with stagger on first appear
        await AnimatePageEntry();
    }

    private async Task AnimatePageEntry()
    {
        // Fade-up the whole content area
        var content = this.Content;
        if (content != null)
        {
            content.Opacity = 0;
            content.TranslationY = 16;
            await Task.WhenAll(
                content.FadeTo(1, 400, Easing.CubicOut),
                content.TranslateTo(0, 0, 400, Easing.CubicOut)
            );
        }
    }

    private async void OnInstagramTapped(object? sender, EventArgs e)
    {
        var uri = "https://www.instagram.com/_angelin_angel0n_?igsh=MTBmbXF4aGR1ZWs0MA%3D%3D&utm_source=qr";
        await Launcher.Default.OpenAsync(new Uri(uri));
    }
}
