using MauiApp1.Models;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using MinotMobile.Services;
using MauiApp1.Helper;

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
                ViewModel.OnPropertyChanged(nameof(ViewModel.Livraison));
                ViewModel.OnPropertyChanged(nameof(ViewModel.PeutLivrer));
                ViewModel.OnPropertyChanged(nameof(ViewModel.PeutTerminer));
            }
        }

        public DetailLivraisonViewModel ViewModel { get; } = new();

        public DetailLivraison()
        {
            InitializeComponent();
            BindingContext = ViewModel;
        }

        public async Task<Location?> GetCurrentLocationAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location == null)
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
                return location;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Impossible d'obtenir la position : {ex.Message}", "OK");
                return null;
            }
        }

        public async Task OuvrirItineraireAsync()
        {
            var location = await GetCurrentLocationAsync();
            if (location == null)
            {
                await DisplayAlert("Erreur", "Impossible d'obtenir la position GPS.", "OK");
                return;
            }

            var adresse = Livraison?.Commande?.Client?.Adresse;
            if (string.IsNullOrWhiteSpace(adresse))
            {
                await DisplayAlert("Erreur", "Adresse de livraison manquante.", "OK");
                return;
            }

            var destination = Uri.EscapeDataString(adresse);
            var url = $"https://www.google.com/maps/dir/?api=1&origin={location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)},{location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&destination={destination}&travelmode=driving";

            try
            {
                await Launcher.OpenAsync(url);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Impossible d'ouvrir Google Maps : {ex.Message}", "OK");
            }
        }

        private async void OnItineraireClicked(object sender, EventArgs e)
        {
            await OuvrirItineraireAsync();
        }

        public async Task LivrerAsync()
        {
            if (Livraison == null)
                return;

            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token))
                {
                    await DisplayAlert("Erreur", "Session expirée. Veuillez vous reconnecter.", "OK");
                    await Shell.Current.GoToAsync("///Connexion");
                    return;
                }

                var httpClient = new HttpClientService(ApiHelper.BaseUrl);
                httpClient.SetAuthorizationHeader(token);

                // Payload complet
                var payload = new Dictionary<string, object>
                {
                    { "Statut", "LIVREE" }
                };

                var response = await httpClient.PutAsync<Livraison>($"api/livraisons/{Livraison.IdLivraison}", payload);

                if (response != null)
                {
                    Livraison.Statut = response.Statut;
                    ViewModel.OnPropertyChanged(nameof(ViewModel.Livraison));
                    ViewModel.OnPropertyChanged(nameof(ViewModel.PeutLivrer));
                    ViewModel.OnPropertyChanged(nameof(ViewModel.PeutTerminer));
                    await DisplayAlert("Succès", "La livraison est maintenant livrée.", "OK");
                }
                else
                {
                    await DisplayAlert("Erreur", $"Impossible de mettre à jour le statut.\nJSON reçu : {HttpClientService.LastJsonRecu ?? "null"}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Erreur lors de la mise à jour : {ex.Message}", "OK");
            }
        }

        private async void OnLivrerClicked(object sender, EventArgs e)
        {
            await LivrerAsync();
        }
        private async void OnRemarqueClicked(object sender, EventArgs e)
        {
            if (Livraison == null)
            {
                Console.WriteLine("[DEBUG] Livraison est null dans OnRemarqueClicked.");
                return;
            }

            Console.WriteLine($"[DEBUG] Navigation vers Remarque avec Livraison.IdLivraison = {Livraison.IdLivraison}");

            await Shell.Current.GoToAsync("remarque", new Dictionary<string, object>
            {
                { "Livraison", Livraison }
            });
        }

    }
}