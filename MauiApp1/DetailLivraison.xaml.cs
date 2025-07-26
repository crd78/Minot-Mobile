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
        private Livraison _livraison;

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
            Console.WriteLine("[DOTNET] Début GetCurrentLocationAsync");
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location == null)
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
                Console.WriteLine("[DOTNET] Position récupérée : " + (location != null ? $"{location.Latitude},{location.Longitude}" : "null"));
                return location;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DOTNET] Exception dans GetCurrentLocationAsync : " + ex);
                await DisplayAlert("Erreur", $"Impossible d'obtenir la position : {ex.Message}", "OK");
                return null;
            }
        }

        public async Task OuvrirItineraireAsync()
        {
            Console.WriteLine("[DOTNET] Début OuvrirItineraireAsync");

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
            Console.WriteLine($"[DOTNET] URL Google Maps : {url}");

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
            Console.WriteLine("Bouton itinéraire GPS cliqué");
            try
            {
                await OuvrirItineraireAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DOTNET] Exception dans OnItineraireClicked : " + ex);
                await DisplayAlert("Erreur", $"Exception : {ex.Message}", "OK");
            }
        }

        public async Task CommencerLivraisonAsync()
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

                // Payload complet si besoin
                var payload = new
                {
                    IdLivraison = Livraison.IdLivraison,
                    Statut = "EN_COURS"
                    // Ajoute d'autres propriétés si l'API les exige
                };

                var response = await httpClient.PutAsync<Livraison>($"api/livraisons/{Livraison.IdLivraison}", payload);

                if (response != null)
                {
                    Livraison.Statut = response.Statut; // Met à jour le modèle local avec la réponse
                    ViewModel.OnPropertyChanged(nameof(ViewModel.Livraison));
                    ViewModel.OnPropertyChanged(nameof(ViewModel.PeutTerminer));
                    await DisplayAlert("Succès", "La livraison est maintenant en cours.", "OK");
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

        private async void OnCommencerLivraisonClicked(object sender, EventArgs e)
        {
            await CommencerLivraisonAsync();
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

                var payload = new { Statut = "LIVREE" };
                var response = await httpClient.PutAsync<object>($"api/livraisons/{Livraison.IdLivraison}", payload);

                if (response != null)
                {
                    Livraison.Statut = "LIVREE";
                    ViewModel.OnPropertyChanged(nameof(ViewModel.Livraison));
                    ViewModel.OnPropertyChanged(nameof(ViewModel.PeutLivrer));
                    await DisplayAlert("Succès", "La livraison est maintenant livrée.", "OK");
                }
                else
                {
                    await DisplayAlert("Erreur", "Impossible de mettre à jour le statut.", "OK");
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

    }
}