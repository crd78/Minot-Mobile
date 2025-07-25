using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Threading.Tasks;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace QrCodeScanner
{
    public partial class ScanPage : ContentPage
    {
        private readonly TaskCompletionSource<string?> _tcs = new();

        public ScanPage()
        {
            InitializeComponent();
            // S'assure que la caméra est bien arrêtée quand la page disparaît
            Disappearing += (s, e) =>
            {
                cameraBarcodeReaderView.Handler?.DisconnectHandler();
            };
        }

        public Task<string?> ScanAsync() => _tcs.Task;

        // Méthode pour arrêter la caméra manuellement
        public void StopCamera()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (cameraBarcodeReaderView.IsDetecting)
                {
                    cameraBarcodeReaderView.IsDetecting = false;
                }
                if (cameraBarcodeReaderView.IsTorchOn)
                {
                    cameraBarcodeReaderView.IsTorchOn = false;
                }
            });
        }

        private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            var code = e.Results.FirstOrDefault()?.Value;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Arrêter la détection pour éviter de multiples scans
                if (cameraBarcodeReaderView.IsDetecting)
                {
                    cameraBarcodeReaderView.IsDetecting = false;
                }
                _tcs.TrySetResult(code);
            });
        }
    }
}