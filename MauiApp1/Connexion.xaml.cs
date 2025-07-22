using MauiApp1.Models;
using MinotMobile.Services;
using Microsoft.Maui.Controls;

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
                // Vérification des entrées
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

                // Afficher un indicateur de chargement
                IsBusy = true;

                string endpoint = "api/client/connexion";
                var response = await _httpClient.PostAsync<LoginRequest, LoginResponse>(endpoint, loginRequest);

                if (response != null && !string.IsNullOrEmpty(response.Access))
                {
                    await Application.Current.MainPage.DisplayAlert("Succès", "Connexion réussie", "OK");
                }
                else
                {
                    string urlFinale = new Uri(new Uri(ApiHelper.BaseUrl), endpoint).ToString();
                    string errorMessage = $"Échec de la connexion :\nURL appelée : {urlFinale}\n";

                    // Ajoute les logs JSON envoyés/reçus
                    errorMessage += $"\nJSON envoyé : {System.Text.Json.JsonSerializer.Serialize(loginRequest)}";

                    // Pour le JSON reçu, tu peux le récupérer dans HttpClientService et le stocker dans une propriété statique temporaire
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