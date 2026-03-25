using ScoutCode.Views;

namespace ScoutCode
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Rutas de navegacion
            Routing.RegisterRoute("CipherDetailPage", typeof(CipherDetailPage));
        }
    }
}
