using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace QrCodeScanner
{
    public static class BarcodeScannerUtil
    {
        public static async Task<string?> ScanAsync(INavigation navigation)
        {
            var scanPage = new ScanPage();
            await navigation.PushModalAsync(scanPage);
            var result = await scanPage.ScanAsync();

            // Étape 1: Arrêter explicitement la caméra
            scanPage.StopCamera();

            // Étape 2: Attendre un court instant pour que les ressources natives soient libérées
            await Task.Delay(100);

            // Étape 3: Fermer la page modale
            await navigation.PopModalAsync();

            return result;
        }
    }
}