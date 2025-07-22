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
            return await scanPage.ScanAsync();
        }
    }
}