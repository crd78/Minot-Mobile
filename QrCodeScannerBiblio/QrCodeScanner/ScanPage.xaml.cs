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
            // S'assure que la cam�ra est bien arr�t�e quand la page dispara�t
            Disappearing += (s, e) =>
            {
                cameraBarcodeReaderView.Handler?.DisconnectHandler();
            };
        }

        public Task<string?> ScanAsync() => _tcs.Task;

        // M�thode pour arr�ter la cam�ra manuellement
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
                // Arr�ter la d�tection pour �viter de multiples scans
                if (cameraBarcodeReaderView.IsDetecting)
                {
                    cameraBarcodeReaderView.IsDetecting = false;
                }
                _tcs.TrySetResult(code);
            });
        }
    }
}