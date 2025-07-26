using System;
using System.Text.Json.Serialization;

namespace MauiApp1.Models
{
    public class Livraison
    {
        [JsonPropertyName("IdLivraison")]
        public int IdLivraison { get; set; }

        [JsonPropertyName("commande")]
        public Commande Commande { get; set; } = new();

        [JsonPropertyName("IdCommande")]
        public int IdCommande { get; set; }

        [JsonPropertyName("IdTransport")]
        public int IdTransport { get; set; }

        [JsonPropertyName("Statut")]
        public string Statut { get; set; } = string.Empty;

        [JsonPropertyName("IdEntrepot")]
        public int IdEntrepot { get; set; }

        [JsonPropertyName("IdVehicule")]
        public int IdVehicule { get; set; }

        [JsonPropertyName("DatePrevue")]
        public DateTime DatePrevue { get; set; }

        [JsonPropertyName("DateLivraison")]
        public DateTime? DateLivraison { get; set; }

        [JsonPropertyName("Commentaire")]
        public string Commentaire { get; set; } = string.Empty;

        [JsonPropertyName("DateCreation")]
        public DateTime DateCreation { get; set; }

        [JsonPropertyName("DateMiseAJour")]
        public DateTime DateMiseAJour { get; set; }

        // Propriétés d'affichage (optionnelles)
        public string NomEntreprise => $"Client #{Commande?.IdClient}";
        public string StatutText => Statut switch
        {
            "LIVREE" => "Livrée",
            "ANNULEE" => "Annulée",
            "EN_COURS" => "En cours",
            "PREPARATION" => "En préparation",
            _ => Statut
        };

        public class Client
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("nom")]
            public string Nom { get; set; } = string.Empty;

            [JsonPropertyName("prenom")]
            public string Prenom { get; set; } = string.Empty;

            [JsonPropertyName("adresse")]
            public string Adresse { get; set; } = string.Empty;
        }
    }
}