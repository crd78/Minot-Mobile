using MauiApp1.Models;

namespace MauiApp1
{
    [QueryProperty(nameof(Livraison), "Livraison")]
    public partial class DetailLivraison : ContentPage
    {
        public Livraison Livraison
        {
            get => ViewModel.Livraison;
            set
            {
                ViewModel.Livraison = value;
            }
        }

        public DetailLivraisonViewModel ViewModel { get; } = new();

        public DetailLivraison()
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }
    }
}