using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using QrCodeScanner; // Import de votre bibliothèque

namespace MauiApp1
{
    public partial class NouvelleLivraison : ContentPage
    {
        private string? _scannedCode;

        public NouvelleLivraison()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Lancer automatiquement le scan quand la page apparaît
            await StartScan();
        }

        private async Task StartScan()
        {
            try
            {
                // Vérifier les permissions caméra
                var status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission refusée",
                        "L'accès à la caméra est requis pour scanner les codes-barres.", "OK");
                    await Shell.Current.GoToAsync("..");
                    return;
                }

                // Utiliser votre bibliothèque de scan
                var result = await BarcodeScannerUtil.ScanAsync(Navigation);

                if (!string.IsNullOrEmpty(result))
                {
                    _scannedCode = result;

                    // Afficher le résultat
                    ResultLabel.Text = result;
                    ResultFrame.IsVisible = true;
                    ValidateButton.IsEnabled = true;

                    await DisplayAlert("Code détecté", $"Code scanné : {result}", "OK");
                }
                else
                {
                    await DisplayAlert("Scan annulé", "Aucun code détecté", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur",
                    $"Erreur lors du scan : {ex.Message}", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnRestartClicked(object sender, EventArgs e)
        {
            // Recommencer le scan
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