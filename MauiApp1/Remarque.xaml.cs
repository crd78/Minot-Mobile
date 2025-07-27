using MauiApp1.Models;
using MinotMobile.Services;
using MauiApp1.Helper;
using Microsoft.Maui.ApplicationModel;

namespace MauiApp1
{
    [QueryProperty(nameof(Livraison), "Livraison")] // Déplacez l'attribut ici
    public partial class Remarque : ContentPage
    {
        // Propriété à binder depuis la navigation ou le ViewModel
        public Livraison Livraison { get; set; }

        public Remarque()
        {
            InitializeComponent();
        }

        private async void OnAjouterRemarqueClicked(object sender, EventArgs e)
        {
            Console.WriteLine("[DEBUG] Bouton Ajouter la remarque cliqué.");

            if (Livraison == null)
            {
                Console.WriteLine("[DEBUG] Livraison est null. Vérifiez si elle est correctement passée à la page.");
                return;
            }

            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("[DEBUG] Token non disponible. L'utilisateur doit se reconnecter.");
                    await DisplayAlert("Erreur", "Session expirée. Veuillez vous reconnecter.", "OK");
                    await Shell.Current.GoToAsync("///Connexion");
                    return;
                }

                var commentaire = RemarqueEditor.Text?.Trim();
                if (string.IsNullOrEmpty(commentaire))
                {
                    Console.WriteLine("[DEBUG] Aucun commentaire saisi. L'utilisateur doit entrer un texte.");
                    await DisplayAlert("Erreur", "Veuillez saisir une remarque.", "OK");
                    return;
                }

                Console.WriteLine($"[DEBUG] Commentaire saisi : {commentaire}");

                var httpClient = new HttpClientService(ApiHelper.BaseUrl);
                httpClient.SetAuthorizationHeader(token);

               

                var payload = new Dictionary<string, object>
                {
                    { "Commentaire", commentaire }
                };

                Console.WriteLine($"[DEBUG] Payload envoyé : {System.Text.Json.JsonSerializer.Serialize(payload)}");

                var response = await httpClient.PutAsync<Livraison>($"api/livraisons/{Livraison.IdLivraison}", payload);

                if (response != null)
                {
                    Console.WriteLine("[DEBUG] Réponse API reçue. Mise à jour du commentaire dans l'objet Livraison.");
                    Livraison.Commentaire = response.Commentaire;
                    await DisplayAlert("Succès", "Remarque ajoutée.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    Console.WriteLine("[DEBUG] Échec de l'ajout de la remarque. Vérifiez la réponse API.");
                    await DisplayAlert("Erreur", $"Impossible d'ajouter la remarque.\nJSON reçu : {HttpClientService.LastJsonRecu ?? "null"}", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Exception : {ex.Message}");
                await DisplayAlert("Erreur", $"Erreur lors de l'ajout : {ex.Message}", "OK");
            }
        }
    }
}