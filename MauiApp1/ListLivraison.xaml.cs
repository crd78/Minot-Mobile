using MauiApp1.Models;
using MauiApp1.Helper;
using MinotMobile.Services; // Ajout du using pour HttpClientService
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System;
using Microsoft.Maui.Storage; // Ajout du using pour SecureStorage

namespace MauiApp1
{
    public partial class ListLivraison : ContentPage, INotifyPropertyChanged
    {
        private readonly HttpClientService _httpClientService; // Remplacer ApiHelper par HttpClientService
        public ObservableCollection<Livraison> Livraisons { get; } = new();
        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ListLivraison()
        {
            InitializeComponent();
            // Instancier HttpClientService avec l'URL de base de ApiHelper
            _httpClientService = new HttpClientService(ApiHelper.BaseUrl);
            BindingContext = this;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            await LoadLivraisons();
        }

        private async Task LoadLivraisons()
        {
            if (IsRefreshing) return;

            IsRefreshing = true;
            try
            {
                // Récupérer le token et configurer le header d'autorisation
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token))
                {
                    // Gérer le cas où le token n'est pas trouvé (ex: redirection vers la page de connexion)
                    await DisplayAlert("Erreur", "Session expirée. Veuillez vous reconnecter.", "OK");
                    await Shell.Current.GoToAsync("///Connexion"); // Redirige vers la page de connexion
                    return;
                }
                _httpClientService.SetAuthorizationHeader(token);

                // Utiliser HttpClientService pour appeler l'API
                var livraisons = await _httpClientService.GetAsync<List<Livraison>>("api/livraisons");
                if (livraisons != null)
                {
                    Livraisons.Clear();
                    foreach (var livraison in livraisons)
                    {
                        Livraisons.Add(livraison);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", $"Impossible de charger les livraisons : {ex.Message}", "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("nouvelleLivraison");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}