using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using MauiApp1.Models;
using MauiApp1.Helper;
using MinotMobile.Services; // Assurez-vous que le bon namespace est utilisé
using ZXing.Net.Maui;

namespace MauiApp1
{
    public partial class NouvelleLivraison : ContentPage
    {
        private string? _scannedCode;

        public NouvelleLivraison()
        {
            InitializeComponent();
            cameraView.Options = new BarcodeReaderOptions
            {
                Formats = BarcodeFormat.QrCode, // On se concentre sur les QR Codes
                AutoRotate = true,
                Multiple = false
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await StartScan();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // S'assurer que la caméra est bien arrêtée
            if (cameraView.IsDetecting)
            {
                cameraView.IsDetecting = false;
            }
        }

        private async Task StartScan()
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission refusée", "L'accès à la caméra est requis pour scanner.", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }

            // Ne touche pas à MainContent.IsVisible !
            cameraView.IsVisible = true;
            cameraView.IsDetecting = true;
        }

        private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                cameraView.IsDetecting = false;
                cameraView.IsVisible = false;

                var result = e.Results.FirstOrDefault()?.Value;
                if (string.IsNullOrEmpty(result))
                {
                    return;
                }

                _scannedCode = result;
                ResultLabel.Text = result;
                ResultFrame.IsVisible = true;
                ValidateButton.IsEnabled = true;

              
            });
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnRestartClicked(object sender, EventArgs e)
        {
            _scannedCode = null;
            ResultFrame.IsVisible = false;
            ValidateButton.IsEnabled = false;
            await StartScan();
        }

        private async void OnValidateClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_scannedCode))
            {
                var result = await DisplayAlert("Validation",
                    $"Voulez-vous valider ce code ?\n{_scannedCode}",
                    "Oui", "Non");

                if (result)
                {
                    await ProcessScannedCode(_scannedCode);
                    
                }
                else {                    await DisplayAlert("Annulé", "Validation annulée.", "OK"); }
            }
        }

        private async Task ProcessScannedCode(string code)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Début ProcessScannedCode avec code : {code}");

                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token))
                {
                    await DisplayAlert("Erreur", "Session expirée. Veuillez vous reconnecter.", "OK");
                    await Shell.Current.GoToAsync("///Connexion");
                    return;
                }

                var httpClient = new HttpClientService(ApiHelper.BaseUrl);
                httpClient.SetAuthorizationHeader(token);

                Console.WriteLine($"[DEBUG] Appel API GET /api/livraisons/{code}");
                var livraison = await httpClient.GetAsync<Livraison>($"api/livraisons/{code}/enriched");

                Console.WriteLine($"[DEBUG] JSON reçu : {MinotMobile.Services.HttpClientService.LastJsonRecu ?? "null"}");

                if (livraison == null)
                {
                    await DisplayAlert("Erreur", $"Aucune livraison trouvée pour l'ID : {code}", "OK");
                    Console.WriteLine("[DEBUG] livraison == null");
                    return;
                }

                // Met à jour le statut en base
                var payload = new Dictionary<string, object>
                {
                    { "Statut", "EN_COURS" }
                };
                var updateResponse = await httpClient.PutAsync<Livraison>($"api/livraisons/{livraison.IdLivraison}", payload);
                Console.WriteLine($"[DEBUG] Réponse PUT : {MinotMobile.Services.HttpClientService.LastJsonRecu ?? "null"}");
                if (updateResponse != null)
                {
                    livraison.Statut = updateResponse.Statut;
                }
                else
                {
                    await DisplayAlert("Attention", "Impossible de passer la livraison à EN_COURS.", "OK");
                }

                // Navigation vers la page de détail
                await Shell.Current.GoToAsync("detailLivraison", new Dictionary<string, object>
        {
            { "Livraison", livraison }
        });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Erreur lors du traitement : {ex.Message}", "OK");
                Console.WriteLine($"[DEBUG] Exception : {ex}");
            }
        }
    }
}