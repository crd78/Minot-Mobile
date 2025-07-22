using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Threading.Tasks;
using ZXing.Net.Maui;

namespace QrCodeScanner
{
    public partial class ScanPage : ContentPage
    {
        private readonly TaskCompletionSource<string?> _tcs = new();

        public ScanPage()
        {
            InitializeComponent();
        }

        public Task<string?> ScanAsync() => _tcs.Task;

        private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            var code = e.Results.FirstOrDefault()?.Value;
            _tcs.TrySetResult(code);
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PopModalAsync();
            });
        }
    }
}