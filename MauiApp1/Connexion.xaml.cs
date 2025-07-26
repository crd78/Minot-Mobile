using MauiApp1.Models;
using MinotMobile.Services;
using MauiApp1.Helper; // Assurez-vous que le bon namespace est utilisé
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace MauiApp1
{
    public partial class Connexion : ContentPage // <-- doit être ContentPage
    {
        private readonly HttpClientService _httpClient;
        public static string? LastJsonRecu;
        public Connexion()
        {
            InitializeComponent(); // sera reconnu
            _httpClient = new HttpClientService(ApiHelper.BaseUrl);
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(EmailEntry.Text) || string.IsNullOrEmpty(PasswordEntry.Text))
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", "Veuillez remplir tous les champs", "OK");
                    return;
                }

                var email = EmailEntry.Text;
                var password = PasswordEntry.Text;

                var loginRequest = new LoginRequest
                {
                    Email = email,
                    Password = password
                };

                IsBusy = true;

                string endpoint = "api/client/connexion";
                var response = await _httpClient.PostAsync<LoginRequest, LoginResponse>(endpoint, loginRequest);

                // Vérification du rôle
                if (response != null && !string.IsNullOrEmpty(response.Access))
                {
                    // Vérifie le rôle (adapte le nom de la propriété si besoin)
                    if (response.Role == 4)
                    {
                        await Application.Current.MainPage.DisplayAlert("Permission refusée", "Vous n'avez pas les droits pour accéder à cette application.", "OK");
                        return;
                    }

                    await SecureStorage.SetAsync("auth_token", response.Access);

                  
                    await Shell.Current.GoToAsync("listLivraison");
                }
                else
                {
                    string urlFinale = new Uri(new Uri(ApiHelper.BaseUrl), endpoint).ToString();
                    string errorMessage = $"Échec de la connexion :\nURL appelée : {urlFinale}\n";
                    errorMessage += $"\nJSON envoyé : {System.Text.Json.JsonSerializer.Serialize(loginRequest)}";
                    errorMessage += $"\nJSON reçu : {MinotMobile.Services.HttpClientService.LastJsonRecu ?? "Non disponible"}";
                    if (response == null)
                        errorMessage += "\nAucune réponse du serveur";

                    await Application.Current.MainPage.DisplayAlert("Erreur", errorMessage, "OK");
                    Console.WriteLine($"Erreur de connexion: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur",
                    $"Une erreur s'est produite: {ex.Message}\n\nDétails: {ex.StackTrace}", "OK");

                Console.WriteLine($"Exception dans OnLoginClicked: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnClearEmailClicked(object sender, EventArgs e)
        {
            EmailEntry.Text = string.Empty;
        }

        private void OnClearPasswordClicked(object sender, EventArgs e)
        {
            PasswordEntry.Text = string.Empty;
        }
    }
}