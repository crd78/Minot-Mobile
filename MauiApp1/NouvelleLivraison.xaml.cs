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

                await DisplayAlert("Code détecté", $"Code scanné : {result}", "OK");
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
                    await DisplayAlert("berk", "Code validé avec succès !", "OK");
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
                var livraison = await httpClient.GetAsync<Livraison>($"api/livraisons/{code}");

                // Affiche le JSON brut reçu
                await DisplayAlert("Debug", $"Réponse JSON brute : {MinotMobile.Services.HttpClientService.LastJsonRecu ?? "null"}", "OK");
                Console.WriteLine($"[DEBUG] JSON reçu : {MinotMobile.Services.HttpClientService.LastJsonRecu ?? "null"}");

                if (livraison == null)
                {
                    await DisplayAlert("Erreur", $"Aucune livraison trouvée pour l'ID : {code}", "OK");
                    Console.WriteLine("[DEBUG] livraison == null");
                    return;
                }

                string details = $"Livraison #{livraison.IdLivraison}\n" +
                                 $"Statut : {livraison.StatutText}\n" +
                                 $"Entreprise : {livraison.NomEntreprise}\n" +
                                 $"Date prévue : {livraison.DatePrevue:dd/MM/yyyy}";
                await DisplayAlert("Détails livraison", details, "OK");

                Console.WriteLine("[DEBUG] Fin ProcessScannedCode, navigation vers ..");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Erreur lors du traitement : {ex.Message}", "OK");
                Console.WriteLine($"[DEBUG] Exception : {ex}");
            }
        }
    }
}