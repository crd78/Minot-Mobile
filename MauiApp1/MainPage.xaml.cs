using QrCodeScanner;
using Microsoft.Maui.ApplicationModel;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        //private async void OnScanClicked(object sender, EventArgs e)
        //{
        //    var status = await Permissions.RequestAsync<Permissions.Camera>();
        //    if (status != PermissionStatus.Granted)
        //    {
        //        await DisplayAlert("Permission refusée", "L'accès à la caméra est requis pour scanner.", "OK");
        //        return;
        //    }

        //    var result = await BarcodeScannerUtil.ScanAsync(Navigation);
        //    await DisplayAlert("Résultat du scan", result ?? "Aucun code détecté", "OK");
        //}
        private async void OnConnexionButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("connexion");
        }
    }
}