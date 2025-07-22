using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Linq;
using ZXing.Net.Maui;

namespace MauiApp1
{
    public partial class NouvelleLivraison : ContentPage
    {
        private string? _scannedCode;

        public NouvelleLivraison()
        {
            InitializeComponent();
            RequestCameraPermission();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Démarrer la caméra quand la page apparaît
            if (barcodeReader != null)
            {
                barcodeReader.IsDetecting = true;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Arrêter la caméra quand la page disparaît
            if (barcodeReader != null)
            {
                barcodeReader.IsDetecting = false;
            }
        }

        private async void RequestCameraPermission()
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permission refusée",
                    "L'accès à la caméra est requis pour scanner les codes-barres.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }

        private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            var code = e.Results.FirstOrDefault()?.Value;
            if (!string.IsNullOrEmpty(code))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _scannedCode = code;

                    // Arrêter la détection
                    barcodeReader.IsDetecting = false;

                    // Afficher le résultat
                    ResultLabel.Text = code;
                    ResultFrame.IsVisible = true;
                    ValidateButton.IsEnabled = true;

                    // Feedback visuel/sonore optionnel
                    DisplayAlert("Code détecté", $"Code scanné : {code}", "OK");
                });
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private void OnRestartClicked(object sender, EventArgs e)
        {
            // Recommencer le scan
            _scannedCode = null;
            ResultFrame.IsVisible = false;
            ValidateButton.IsEnabled = false;
            barcodeReader.IsDetecting = true;
        }

        private async void OnValidateClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_scannedCode))
            {
                // Ici tu peux traiter le code scanné
                // Par exemple, l'envoyer à une API ou naviguer vers une autre page

                var result = await DisplayAlert("Validation",
                    $"Voulez-vous valider ce code ?\n{_scannedCode}",
                    "Oui", "Non");

                if (result)
                {
                    // Traitement du code validé
                    await ProcessScannedCode(_scannedCode);
                }
            }
        }

        private async Task ProcessScannedCode(string code)
        {
            try
            {
                // Ici tu peux ajouter la logique pour traiter le code scanné
                // Par exemple : appel API, sauvegarde, etc.

                await DisplayAlert("Succès",
                    $"Code traité avec succès : {code}", "OK");

                // Retourner à la page précédente
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur",
                    $"Erreur lors du traitement : {ex.Message}", "OK");
            }
        }
    }
}