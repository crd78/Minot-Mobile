using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static MauiApp1.Models.Livraison;

namespace MauiApp1.Models
{
    public class Commande
    {
        [JsonPropertyName("IdCommande")]
        public int IdCommande { get; set; }

        [JsonPropertyName("client")]
        public Client Client { get; set; } = new();

        [JsonPropertyName("IdClient")]
        public int IdClient { get; set; }

        [JsonPropertyName("DateCommande")]
        public DateTime DateCommande { get; set; }

        [JsonPropertyName("Statut")]
        public string Statut { get; set; } = string.Empty;

        [JsonPropertyName("MontantTotalHT")]
        public string MontantTotalHT { get; set; }

        [JsonPropertyName("MontantTotalTTC")]
        public string MontantTotalTTC { get; set; }

        [JsonPropertyName("DateMiseAJour")]
        public DateTime DateMiseAJour { get; set; }

        [JsonPropertyName("produits")]
        public List<ProduitCommande> Produits { get; set; } = new();
    }
}