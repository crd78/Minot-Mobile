using MauiApp1.Models;
using MinotMobile.Services;
using Microsoft.Maui.Controls;

namespace MauiApp1
{
    public partial class ListLivraison : ContentPage
    {
        private readonly HttpClientService _httpClient;
        private readonly ListLivraisonViewModel _viewModel = new();

        public ListLivraison()
        {
            InitializeComponent();
            _httpClient = new HttpClientService(ApiHelper.BaseUrl);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var token = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.SetAuthorizationHeader(token);

            await _viewModel.LoadLivraisonsAsync(_httpClient);
        }
    }
}