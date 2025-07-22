namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("connexion", typeof(Connexion));
            Routing.RegisterRoute("listLivraison", typeof(ListLivraison));
        }
    }
}
